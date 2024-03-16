using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Components
{
    public class CellAuthoring : MonoBehaviour
    {
        public class CellBaker:Baker<CellAuthoring>
        {
            public override void Bake(CellAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<Position>(entity);
                
            }
        }
    }
    
    public struct Position : IComponentData
    {
        public int2 Value;
    }

}