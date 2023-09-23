using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct GunTargetingSystem : ISystem
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
        var entityCommandBuffer = new EntityCommandBuffer(Allocator.Temp);

        foreach(var (gunTransform,gunData,gunEntity) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<GunData>>().WithEntityAccess())
        {
            float3 desiredTargetingPosition = new float3(0,200,0);
            float currentDistance = 999;

            foreach(var (enemyTransform,enemyMovementData) in SystemAPI.Query<RefRO<LocalTransform>, RefRO<EnemyMovementData>>())
            {
                var dist = math.distance(gunData.ValueRO.GunPositionXZ, enemyMovementData.ValueRO.EnemyPositionXZ);
                if (dist < currentDistance)
                {
                    desiredTargetingPosition = enemyTransform.ValueRO.Position;
                    currentDistance = dist;
                }
            }

            LocalTransform newGunEntityTransform = new LocalTransform
            {
                Position = gunTransform.ValueRO.Position,
                Rotation = quaternion.RotateY(RotateTowards(gunTransform.ValueRO.Position, desiredTargetingPosition)),
                Scale = gunTransform.ValueRO.Scale
            };

            entityCommandBuffer.SetComponent(gunEntity, newGunEntityTransform);
            entityCommandBuffer.SetComponent(gunData.ValueRO.GunRenderer, new LocalTransform { Position = new float3(0,0,0),Rotation = quaternion.Euler(22f,0,0),Scale = 1f });
        }

        entityCommandBuffer.Playback(state.EntityManager);
    }

    private float RotateTowards(float3 objectsPosition, float3 targetPosition)
    {
        var x = objectsPosition.x - targetPosition.x;
        var y = objectsPosition.z - targetPosition.z;

        return math.atan2(x, y) + math.PI;
    }
}

