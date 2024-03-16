using Unity.Entities;
using Unity.Mathematics;

namespace Components
{
    public struct ViewPosition : IComponentData
    {
        public int2 Center;
    }
}