using Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Jobs
{
    [BurstCompile]
    public partial struct UpdateCellPositionsJob : IJobEntity
    {
        [ReadOnly] public int viewSize;
        public void Execute([EntityIndexInQuery] int index, ref LocalTransform transform, ref Position position)
        {
            var x = index % viewSize;
            var y = index / viewSize;
            position.Value = new int2(x, y);
            transform.Position = new float3(x, y, 0);
        }
    }
}