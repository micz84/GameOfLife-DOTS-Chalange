using Components;
using MonoBehaviours;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Systems
{
    public partial class InputSystem : SystemBase
    {
        private EntityQuery _cellsQuery;
        private Manager _manager;
        [BurstCompile]
        protected override void OnCreate()
        {
            RequireForUpdate<Config>();
            RequireForUpdate<ViewPosition>();
            _cellsQuery = GetEntityQuery(ComponentType.ReadWrite<Position>());
        }

        protected override void OnUpdate()
        {
            var configEntity = SystemAPI.GetSingletonEntity<Config>();
            var config = SystemAPI.GetSingleton<Config>();
            var size = config.SimulationSize;
            var viewPosition = SystemAPI.GetSingleton<ViewPosition>();
            var viewPositionEntity = SystemAPI.GetSingletonEntity<ViewPosition>();
            var speed = 5;
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                speed *= 5;
            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
                speed = 1;
            
            var pos = viewPosition.Center;
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
                pos += new int2(0, 1) * speed;
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
                pos += new int2(0, -1) * speed;
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
                pos += new int2(-1, 0) * speed;
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
                pos += new int2(1, 0) * speed;

            if ((Input.GetKeyDown(KeyCode.Q) && config.ViewSize > 8)  || (config.ViewSize < config.SimulationSize && Input.GetKeyDown(KeyCode.E)))
            {
                if (Input.GetKeyDown(KeyCode.Q))
                    config.ViewSize /= 2;
                else
                    config.ViewSize *= 2;
                EntityManager.DestroyEntity(_cellsQuery);
                EntityManager.SetComponentData(configEntity, config);
                EntityManager.CreateEntity(typeof(StartSimulation));
                if (_manager == null)
                    _manager = GameObject.FindObjectOfType<Manager>();
                _manager.UpdateCamera(config.ViewSize);
            }
            var x = (size + pos.x % size) % size;
            var y = (size + pos.y % size) % size;
            viewPosition.Center = new int2(x, y); 
            EntityManager.SetComponentData(viewPositionEntity, viewPosition);
            
        }
    }
}