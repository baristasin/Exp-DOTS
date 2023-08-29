using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using Unity.Mathematics;
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
            //Job inc. //var ecb = new EntityCommandBuffer(Allocator.TempJob); 

            using var builder = new BlobBuilder(Allocator.Temp);
            ref var zombieSpawnPointBlobAsset = ref builder.ConstructRoot<ZombieSpawnPointBlobAsset>();
            var spawnPointsArray = builder.Allocate(ref zombieSpawnPointBlobAsset.ZombieSpawnPointBlobArray, graveyardAspect.GraveyardProperties.ValueRO.NumberTombstonesToSpawn);

            for (int i = 0; i < graveyardAspect.GraveyardProperties.ValueRO.NumberTombstonesToSpawn; i++)
            {
                //Job inc. //new CreateTombstoneJob
                //Job inc. //{
                //Job inc. //    EntityCommandBuffer = ecb,
                //Job inc. //    JobIndex = i,
                //Job inc. //}.Run();
                //Job inc. //spawnPointsArray[i] = graveyardAspect.GraveyardProperties.ValueRW.TempSpawnPointPosition;

                var tombstone = ecb.Instantiate(graveyardAspect.GraveyardProperties.ValueRO.TombstonePrefab);
                var tombstoneTransform = graveyardAspect.GetRandomTombstoneTransform();
                ecb.SetComponent(tombstone, tombstoneTransform);
                spawnPointsArray[i] = tombstoneTransform.Position;
            }

            graveyardAspect.ZombieSpawnPoints.ValueRW.SpawnPoints = builder.CreateBlobAssetReference<ZombieSpawnPointBlobAsset>(Allocator.Persistent);

            ecb.Playback(state.EntityManager);

            state.Enabled = false;
        }

        //Job inc. //public partial struct CreateTombstoneJob : IJobEntity
        //Job inc. //{
        //Job inc. //    public EntityCommandBuffer EntityCommandBuffer;
        //Job inc. //    public int JobIndex;
        //Job inc. 
        //Job inc. //    public void Execute(GraveyardAspect graveyardAspect)
        //Job inc. //    {
        //Job inc. //        var tombstone = EntityCommandBuffer.Instantiate(graveyardAspect.GraveyardProperties.ValueRO.TombstonePrefab);
        //Job inc. //        var tombstoneTransform = graveyardAspect.GetRandomTombstoneTransform();
        //Job inc. //        EntityCommandBuffer.SetComponent(tombstone, tombstoneTransform);
        //Job inc. //        graveyardAspect.GraveyardProperties.ValueRW.TempSpawnPointPosition = tombstoneTransform.Position;
        //Job inc. //    }
        //Job inc. //}
    }
}

