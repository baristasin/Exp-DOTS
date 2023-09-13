using System;
using Unity.Burst;
using Unity.Entities;

namespace Game.Scripts
{
    [BurstCompile]
    [DisableAutoCreation]

    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [UpdateAfter(typeof(ZombieWalkSystem))]
    public partial struct ZombieAttackSystem : ISystem
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
            new ZombieAttackJob
            {
                DeltaTime = deltaTime,
                EntityCommandBuffer = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter(),
                BrainEntity = brainEntity
            }.ScheduleParallel();
        }
    }

    public partial struct ZombieAttackJob : IJobEntity
    {
        public float DeltaTime;
        public EntityCommandBuffer.ParallelWriter EntityCommandBuffer;
        public Entity BrainEntity;

        public void Execute(ZombieAttackAspect zombieAttackAspect, [EntityIndexInQuery] int sortIndex)
        {
            zombieAttackAspect.Attack(DeltaTime,EntityCommandBuffer,sortIndex, BrainEntity);            
        }
    }

}
