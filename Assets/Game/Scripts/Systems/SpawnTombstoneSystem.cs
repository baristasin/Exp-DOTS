using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
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

            var ecb = new EntityCommandBuffer(Allocator.Temp);

            using var builder = new BlobBuilder(Allocator.Temp);
            ref var zombieSpawnPointBlobAsset = ref builder.ConstructRoot<ZombieSpawnPointBlobAsset>();
            var spawnPointsArray = builder.Allocate(ref zombieSpawnPointBlobAsset.ZombieSpawnPointBlobArray, graveyardAspect.GraveyardProperties.ValueRO.NumberTombstonesToSpawn);           

            for (int i = 0; i < graveyardAspect.GraveyardProperties.ValueRO.NumberTombstonesToSpawn; i++)
            {
                var tombstone = ecb.Instantiate(graveyardAspect.GraveyardProperties.ValueRO.TombstonePrefab);
                var tombstoneTransform = graveyardAspect.GetRandomTombstoneTransform();
                ecb.SetComponent(tombstone, tombstoneTransform);
                spawnPointsArray[i] = tombstoneTransform.Position;
            }

            graveyardAspect.ZombieSpawnPoints.ValueRW.SpawnPoints = builder.CreateBlobAssetReference<ZombieSpawnPointBlobAsset>(Allocator.Persistent);

            ecb.Playback(state.EntityManager);

            state.Enabled = false;
        }
    }   
}

