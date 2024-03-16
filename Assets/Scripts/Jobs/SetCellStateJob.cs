using Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;

namespace Jobs
{
    [BurstCompile]
    [RequireMatchingQueriesForUpdate]
    public partial struct SetCellStateJob : IJobEntity
    {
        [ReadOnly] public NativeArray<byte> Buffer;
        [ReadOnly] public int2 Center;
        [ReadOnly] public int2 halfViewSize;
        [ReadOnly] public int size;

        public void Execute(in Position cell, ref URPMaterialPropertyBaseColor materialPropertyBaseColor)
        {
            var position = cell.Value + Center - halfViewSize;
            position = new int2((size + position.x % size) % size, (size + position.y % size) % size);
            var index = position.x + position.y * size;
            var state = Buffer[index];
            var color = new float4(state, state, state, 1);
            materialPropertyBaseColor.Value = color;
        }
    }
}