using System;
using Game.Scripts;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace Game.Scripts
{
    [BurstCompile]
    [DisableAutoCreation]

    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [UpdateAfter(typeof(ZombieRiseSystem))]
    public partial struct ZombieInitializationSystem : ISystem
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
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            foreach (var zombie in SystemAPI.Query<ZombieWalkAspect>().WithAll<ZombieMoveTag>())
            {
                ecb.RemoveComponent<ZombieMoveTag>(zombie.Entity);
                ecb.SetComponentEnabled<ZombieWalkProperties>(zombie.Entity, false);
            }
            foreach (var zombie in SystemAPI.Query<ZombieAttackAspect>().WithAll<ZombieAttackTag>())
            {
                ecb.RemoveComponent<ZombieAttackTag>(zombie.Entity);
                ecb.SetComponentEnabled<ZombieAttackComponent>(zombie.Entity, false);
            }
            ecb.Playback(state.EntityManager);
        }
    }

}