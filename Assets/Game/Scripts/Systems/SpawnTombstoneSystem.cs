using Unity.Burst;
using Unity.Entities;
using UnityEngine.Scripting;

namespace Game.Scripts
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial struct SpawnTombstoneSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<GraveyardProperties>(); 
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }

        [BurstCompile]       
        public void OnUpdate(ref SystemState state)
        {
            var graveyardEntity = SystemAPI.GetSingletonEntity<GraveyardProperties>();
            var graveyardAspect = SystemAPI.GetAspect<GraveyardAspect>(graveyardEntity);

            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            for (int i = 0; i < graveyardAspect.GraveyardProperties.ValueRO.NumberTombstonesToSpawn; i++)
            {
                var tombstone = ecb.Instantiate(graveyardAspect.GraveyardProperties.ValueRO.TombstonePrefab);
                var tombstoneTransform = graveyardAspect.GetRandomTombstoneTransform();
                ecb.SetComponent(tombstone, tombstoneTransform);
            }

            ecb.Playback(state.EntityManager);

            state.Enabled = false;
        }
    }   
}

