using Unity.Entities;
using UnityEngine;

namespace Components
{
    public class CellPrefabAuthoring : MonoBehaviour
    {
        [SerializeField] private GameObject prefab;

        public class CellPrefabBaker : Baker<CellPrefabAuthoring>
        {
            public override void Bake(CellPrefabAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                var prefab = GetEntity(authoring.prefab, TransformUsageFlags.Dynamic);
                AddComponent<CellPrefab>(entity);
                SetComponent(entity, new CellPrefab()
                {
                    Prefab = prefab
                });
            }
        }
    }

    public struct CellPrefab : IComponentData
    {
        public Entity Prefab;
    }

    
}