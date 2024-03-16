using Components;
using Unity.Burst;
using Unity.Entities;

namespace Systems
{
    [UpdateAfter(typeof(GameOfLifeSystem))]
    public partial struct StopSystem : ISystem
    {
        private EntityQuery _cellsQuery;
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<StopSimulation>();
            state.RequireForUpdate<Config>();
            state.RequireForUpdate<ViewPosition>();
            _cellsQuery = state.GetEntityQuery(typeof(Position));
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var configEntity = SystemAPI.GetSingletonEntity<Config>();
            var viewEntity = SystemAPI.GetSingletonEntity<ViewPosition>();
            var stopEntity = SystemAPI.GetSingletonEntity<StopSimulation>();
            
            state.EntityManager.DestroyEntity(configEntity);
            state.EntityManager.DestroyEntity(viewEntity);
            state.EntityManager.DestroyEntity(stopEntity);
            state.EntityManager.DestroyEntity(_cellsQuery);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}