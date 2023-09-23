using System;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct GunShootingSystem : ISystem
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

        new ShootingJob
        {
            DeltaTime = deltaTime,
            ParallelEntityCommandBuffer = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter()
        }.ScheduleParallel();
    }
}

[BurstCompile]
public partial struct ShootingJob : IJobEntity
{
    public EntityCommandBuffer.ParallelWriter ParallelEntityCommandBuffer;
    public float DeltaTime;

    public void Execute(GunAspect gunAspect, [EntityIndexInQuery] int queryIndex)
    {
        if (gunAspect.CurrentTimerValue <= 0)
        {
            gunAspect.CurrentTimerValue = gunAspect.ShootInterval;
            var bulletEntity = ParallelEntityCommandBuffer.Instantiate(queryIndex, gunAspect.GunBulletEntity);

            LocalTransform newTransform = new LocalTransform
            {
                Position = gunAspect.LocalTransform.ValueRO.Position + gunAspect.LocalTransform.ValueRO.Forward(),
                Rotation = gunAspect.LocalTransform.ValueRO.Rotation,
                Scale = 0.2f
            };

            ParallelEntityCommandBuffer.SetComponent(queryIndex,bulletEntity, newTransform);

            switch (gunAspect.GunData.ValueRO.GunType)
            {
                case GunType.Minigun:
                    {
                        MinigunBullet minigunBullet = new MinigunBullet { LifeTimeValue = 2f };
                        MinigunBulletSpeedData minigunBulletSpeedData = new MinigunBulletSpeedData { BulletSpeed = 50f };
                        ParallelEntityCommandBuffer.AddComponent(queryIndex, bulletEntity,minigunBullet);
                        ParallelEntityCommandBuffer.AddSharedComponent(queryIndex, bulletEntity, minigunBulletSpeedData);
                        break;
                    }
                case GunType.RocketLauncher:
                    {
                        break;
                    }
            }
        }

        else
        {
            gunAspect.CurrentTimerValue -= DeltaTime;
        }

    }
}

