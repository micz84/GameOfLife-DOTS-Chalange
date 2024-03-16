using Unity.Entities;

namespace Components
{
    public struct Config:IComponentData
    {
        public int SimulationSize;
        public int ViewSize;
        public uint Seed;
    }
}