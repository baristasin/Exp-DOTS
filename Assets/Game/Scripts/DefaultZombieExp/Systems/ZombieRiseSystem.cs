using System;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

namespace Game.Scripts
{
    [BurstCompile]
    [DisableAutoCreation]

    [UpdateAfter(typeof(SpawnZombieSystem))]
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial struct ZombieRiseSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {

        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var deltaTime = SystemAPI.Time.DeltaTime;
            var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
         
            new ZombieRiseJob
            {
                DeltaTime = deltaTime,
                EntityCommandBuffer = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter()
            }.ScheduleParallel();
        }

        public partial struct ZombieRiseJob : IJobEntity
        {
            public float DeltaTime;
            public EntityCommandBuffer.ParallelWriter EntityCommandBuffer;

            [BurstCompile]
            public void Execute(ZombieRiseAspect zombieRiseAspect, [ChunkIndexInQuery] int sortKey)
            {
                zombieRiseAspect.Rise(DeltaTime);
                if (!zombieRiseAspect.IsAboveGround) return;

                zombieRiseAspect.SetAtGroundLevel();
                EntityCommandBuffer.RemoveComponent<ZombieRiseRate>(sortKey,zombieRiseAspect.Entity);
                EntityCommandBuffer.SetComponentEnabled<ZombieWalkProperties>(sortKey, zombieRiseAspect.Entity,true);
            }
        }

    }

}