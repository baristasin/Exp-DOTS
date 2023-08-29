using System;
using Unity.Burst;
using Unity.Entities;

namespace Game.Scripts
{
    [BurstCompile]
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
            public void Execute(ZombieRiseAspect zombieRiseAspect, [EntityIndexInQuery] int sortKey)
            {
                zombieRiseAspect.Rise(DeltaTime);
                if (!zombieRiseAspect.IsAboveGround) return;

                zombieRiseAspect.SetAtGroundLevel();
                EntityCommandBuffer.SetComponentEnabled<ZombieWalkProperties>(sortKey, zombieRiseAspect.Entity,true);
                EntityCommandBuffer.RemoveComponent<ZombieRiseRate>(sortKey,zombieRiseAspect.Entity);
            }
        }

    }

}