using System;
using Game.Scripts;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Game.Scripts
{
    [BurstCompile]
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [UpdateAfter(typeof(ZombieInitializationSystem))]
    public partial struct ZombieWalkSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BrainTag>();
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
            var brainEntity = SystemAPI.GetSingletonEntity<BrainTag>();
            var brainScale = SystemAPI.GetComponent<LocalTransform>(brainEntity).Scale;

            new ZombieWalkJob
            {
                DeltaTime = deltaTime,
                BrainRadiusSq = brainScale * brainScale,
                EntityCommandBuffer = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter()
            }.ScheduleParallel();
        }

        public partial struct ZombieWalkJob : IJobEntity
        {
            public float DeltaTime;
            public float BrainRadiusSq;
            public EntityCommandBuffer.ParallelWriter EntityCommandBuffer;

            [BurstCompile]
            private void Execute(ZombieWalkAspect zombieWalkAspect, [ChunkIndexInQuery] int queryIndex)
            {
                zombieWalkAspect.Walk(DeltaTime);
                if (zombieWalkAspect.IsInStoppingRange(float3.zero, BrainRadiusSq))
                {
                    EntityCommandBuffer.SetComponentEnabled<ZombieWalkProperties>(queryIndex,zombieWalkAspect.Entity, false);
                    EntityCommandBuffer.SetComponentEnabled<ZombieAttackComponent>(queryIndex, zombieWalkAspect.Entity, true);                    
                }
            }
        }

    }

}