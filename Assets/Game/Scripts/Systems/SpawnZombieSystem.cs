using System;
using Unity.Burst;
using Unity.Entities;

namespace Game.Scripts
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial struct SpawnZombieSystem : ISystem
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
            var ecbSingleton = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>();

            new SpawnZombieJob
            {
                DeltaTime = deltaTime,
                EntityCommandBuffer = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged)
            }.Run();
        }
    }

    public partial struct SpawnZombieJob : IJobEntity
    {
        public float DeltaTime;
        public EntityCommandBuffer EntityCommandBuffer;

        public void Execute(GraveyardAspect graveyardAspect)
        {
            graveyardAspect.ZombieSpawnTimer -= DeltaTime;
            if (!graveyardAspect.IsRightTimeToSpawnZombie) return;

            //Spawn Zombie

            var zombie = EntityCommandBuffer.Instantiate(graveyardAspect.GraveyardProperties.ValueRO.ZombiePrefab);
            var zombieTransform = graveyardAspect.GetRandomZombieSpawnTransform();
            EntityCommandBuffer.SetComponent(zombie, zombieTransform);

            graveyardAspect.ZombieSpawnTimer = graveyardAspect.GraveyardProperties.ValueRO.ZombieSpawnRate;
        }
    }
}