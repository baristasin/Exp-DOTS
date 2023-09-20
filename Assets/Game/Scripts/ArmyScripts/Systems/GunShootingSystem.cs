using System;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct GunShootingSystem : ISystem
{
    public int Counter;

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
            Counter = Counter,
            ParallelEntityCommandBuffer = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter()
        }.ScheduleParallel();

        Counter++;
    }
}

[BurstCompile]
public partial struct ShootingJob : IJobEntity
{
    public EntityCommandBuffer.ParallelWriter ParallelEntityCommandBuffer;
    public int Counter;
    public float DeltaTime;

    public void Execute(GunAspect gunAspect, [EntityIndexInQuery] int queryIndex)
    {
        if (gunAspect.CurrentTimerValue <= 0)
        {
            gunAspect.CurrentTimerValue = gunAspect.ShootInterval;
            var bulletEntity = ParallelEntityCommandBuffer.Instantiate(queryIndex, gunAspect.GunBulletEntity);

            var value = math.sin(Counter*100f);

            LocalTransform newTransform = new LocalTransform
            {
                Position = new float3(0, 1f, 0),
                Rotation = quaternion.Euler(0,value / 2f, 0),
                Scale = 1f
            };

            ParallelEntityCommandBuffer.SetComponent(queryIndex,bulletEntity, newTransform);

            switch (gunAspect.GunData.ValueRO.GunType)
            {
                case GunType.Minigun:
                    {
                        MinigunBullet minigunBullet = new MinigunBullet { BulletSpeed = 10f,MaxLifeTimeValue = 4f };
                        ParallelEntityCommandBuffer.AddComponent(queryIndex, bulletEntity,minigunBullet);
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

