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
        [ReadOnly] public int2 HalfViewSize;
        [ReadOnly] public int Size;

        public void Execute(in Position cell, ref URPMaterialPropertyBaseColor materialPropertyBaseColor)
        {
            var position = cell.Value + Center - HalfViewSize;
            position = new int2((Size + position.x % Size) % Size, (Size + position.y % Size) % Size);
            var index = position.x + position.y * Size;
            var state = Buffer[index];
            var color = new float4(state, state, state, 1);
            materialPropertyBaseColor.Value = color;
        }
    }
}