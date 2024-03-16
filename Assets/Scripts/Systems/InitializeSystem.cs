using Components;
using Jobs;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace Systems
{
    [UpdateAfter(typeof(InputSystem))]
    public partial struct InitializeSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<StartSimulation>();
            state.RequireForUpdate<Config>();
            state.RequireForUpdate<CellPrefab>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var startSimulationEntity = SystemAPI.GetSingletonEntity<StartSimulation>();
            var config = SystemAPI.GetSingleton<Config>();
            var prefab = SystemAPI.GetSingleton<CellPrefab>();
            var prefabEntity = prefab.Prefab;
            var manager = state.EntityManager;
            var viewSize = config.ViewSize;
            var entities = new NativeArray<Entity>(viewSize * viewSize, Allocator.TempJob);
            manager.Instantiate(prefabEntity, entities);

            var setPositionsJob = new UpdateCellPositionsJob();
            setPositionsJob.ViewSize = viewSize;
            state.Dependency = setPositionsJob.ScheduleParallel(state.Dependency);

            entities.Dispose(state.Dependency);
   
            
            state.EntityManager.DestroyEntity(startSimulationEntity);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
        
        
    }
}