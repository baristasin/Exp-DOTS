using System;
using Game.Scripts;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial struct EnemySpawnSystem : ISystem
{
    public float EnemySpawnTimer;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<EnemyFieldProperties>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (EnemySpawnTimer <= 0)
        {
            var enemyFieldEntity = SystemAPI.GetSingletonEntity<EnemyFieldProperties>();
            var enemyFieldAspect = SystemAPI.GetAspect<EnemyFieldAspect>(enemyFieldEntity);

            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();

            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
            var chosenEnemy = ecb.Instantiate(enemyFieldAspect.EnemyFieldProperties.ValueRO.CubeEnemyPrefab);
            var randomTransform = enemyFieldAspect.GetRandomEnemyTransform();

            EnemySpawnTimer += enemyFieldAspect.EnemyFieldProperties.ValueRO.SpawnInterval;
            ecb.AddComponent(chosenEnemy, randomTransform);
        }

        EnemySpawnTimer -= SystemAPI.Time.DeltaTime;
    }
}

