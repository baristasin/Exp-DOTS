using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Scripts
{
    public struct GraveyardProperties : IComponentData
    {
        public float2 FieldDimensions;
        public int NumberTombstonesToSpawn;
        public Entity TombstonePrefab;
        public Entity ZombiePrefab;
        public float ZombieSpawnRate;
        public float3 TempSpawnPointPosition;
    }
}
