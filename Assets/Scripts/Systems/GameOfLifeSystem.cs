using Components;
using Data;
using Jobs;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace Systems
{
    [UpdateAfter(typeof(InitializeSystem))]
    public partial struct GameOfLifeSystem : ISystem
    {
        private NativeArray<CellData> m_cells;
        private NativeArray<byte> m_buffer;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<Config>();
            state.RequireForUpdate<ViewPosition>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var config = SystemAPI.GetSingleton<Config>();
            var viewPosition = SystemAPI.GetSingleton<ViewPosition>();
            var size = config.SimulationSize;
            var viewSize = config.ViewSize;
            if (!m_cells.IsCreated || m_cells.Length != size * size)
            {
                if (m_cells.IsCreated)
                    m_cells.Dispose();
                
                m_cells = new NativeArray<CellData>(size * size, Allocator.Persistent);
                m_buffer = new NativeArray<byte>(size * size, Allocator.Persistent);
                var tempOffsets = new NativeArray<int2>(8, Allocator.Temp);
                var random = new Random(config.Seed);

                tempOffsets[0] = new int2(0, 1);
                tempOffsets[1] = new int2(1, 1);
                tempOffsets[2] = new int2(1, 0);
                tempOffsets[3] = new int2(1, -1);
                tempOffsets[4] = new int2(0, -1);
                tempOffsets[5] = new int2(-1, -1);
                tempOffsets[6] = new int2(-1, 0);
                tempOffsets[7] = new int2(-1, 1);

                for (var i = 0; i < m_cells.Length; i++)
                {
                    var data = m_cells[i];
                    var x = i % size;
                    var y = i / size;
                    int4x2 neighbours = new int4x2();
                    for (var n = 0; n < 8; n++)
                    {
                        var temp = neighbours[n / 4];
                        var tempOffset = tempOffsets[n];
                        var nX = (size + (x + tempOffset.x) % size) % size;
                        var nY = (size + (y + tempOffset.y) % size) % size;
                        var nIndex = nX + nY * size;
                        temp[n % 4] = nIndex;
                        neighbours[n / 4] = temp;
                    }

                    data.NeighboursIndex = neighbours;
                    data.State = (byte)random.NextInt(2);
                    m_cells[i] = data;
                }
            }

            var golJob = new GameOfLiveJob()
            {
                Cells = m_cells,
                Data = m_buffer
            };
            state.Dependency = golJob.Schedule(m_cells.Length, 64, state.Dependency);
            var copyJob = new CopyJob()
            {
                Cells = m_cells,
                Data = m_buffer
            };

            var jobCopyHandle = copyJob.Schedule(m_cells.Length, 64, state.Dependency);
            var setCellStateJob = new SetCellStateJob()
            {
                Center = viewPosition.Center,
                Size = size,
                HalfViewSize = new int2(viewSize/2,viewSize/2),
                Buffer = m_buffer
            };
            var updateMaterialHandle = setCellStateJob.ScheduleParallel(state.Dependency);
            state.Dependency = JobHandle.CombineDependencies(jobCopyHandle, updateMaterialHandle);
        }

        [BurstCompile]
        public struct GameOfLiveJob : IJobParallelFor
        {
            [ReadOnly]
            public NativeArray<CellData> Cells;
            [WriteOnly]
            public NativeArray<byte> Data;

            public void Execute(int index)
            {
                var currentCell = Cells[index];

                var neighboursCount = 0;
                for (var i = 0; i < 8; i++)
                {
                    var temp = currentCell.NeighboursIndex[i / 4];
                    var neighbourIndex = temp[i % 4];
                    var neighbourCell = Cells[neighbourIndex];
                    neighboursCount += neighbourCell.State;
                }

                var state = (byte)math.select(0, 1, (currentCell.State == 1 && neighboursCount == 2) || neighboursCount == 3);
                Data[index] = state;
            }
        }

        [BurstCompile]
        public struct CopyJob : IJobParallelFor
        {
            public NativeArray<CellData> Cells;
            [ReadOnly] public NativeArray<byte> Data;

            public void Execute(int index)
            {
                var currentCell = Cells[index];
                currentCell.State = Data[index];
                Cells[index] = currentCell;
            }
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
            if (m_cells.IsCreated)
                m_cells.Dispose();
            if (m_buffer.IsCreated)
                m_buffer.Dispose();
        }
    }
}