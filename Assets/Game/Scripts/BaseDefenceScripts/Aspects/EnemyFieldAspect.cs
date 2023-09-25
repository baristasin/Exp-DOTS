using System;
using Game.Scripts;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public readonly partial struct EnemyFieldAspect : IAspect
{
    public readonly Entity Entity;
    public readonly RefRW<EnemyFieldRandom> EnemyFieldRandom => _enemyFieldRandom;

    public readonly RefRW<EnemyFieldPositionDatas> EnemyFieldPositionDatas => _enemyFieldPositionDatas;
    public readonly RefRW<EnemyFieldEnemyPrefabsData> EnemyFieldEnemyPrefabsData => _enemyFieldEnemyPrefabsData;
    public readonly RefRW<EnemyFieldSpawnDatas> EnemyFieldSpawnDatas => _enemyFieldSpawnDatas;


    private readonly RefRO<LocalTransform> _transform;
    private LocalTransform Transform => _transform.ValueRO;

    private readonly RefRW<EnemyFieldPositionDatas> _enemyFieldPositionDatas;
    private readonly RefRW<EnemyFieldEnemyPrefabsData> _enemyFieldEnemyPrefabsData;
    private readonly RefRW<EnemyFieldSpawnDatas> _enemyFieldSpawnDatas;

    private readonly RefRW<EnemyFieldRandom> _enemyFieldRandom;

    public Entity GetRandomEnemy()
    {
        var randNum = _enemyFieldRandom.ValueRW.Value.NextInt(0, 3);

        switch (randNum)
        {
            case 0:
                {
                    return _enemyFieldEnemyPrefabsData.ValueRO.CapsuleEnemyPrefab;                    
                }
            case 1:
                {
                    return _enemyFieldEnemyPrefabsData.ValueRO.CapsuleEnemyPrefab;
                }
            case 2:
                {
                    return _enemyFieldEnemyPrefabsData.ValueRO.CapsuleEnemyPrefab;
                }
            default:
                return _enemyFieldEnemyPrefabsData.ValueRO.CubeEnemyPrefab;
        }
    }

    public LocalTransform GetEnemyTransform(int instantiateIndex)
    {
        var spaceBetweenenemies = _enemyFieldSpawnDatas.ValueRO.WaveEnemyCount * 1.4f;

        var pos = new float3((1.4f * instantiateIndex) - ((float)spaceBetweenenemies / 2f), 2.6f, _transform.ValueRO.Position.z + 50f);

        float forwardStepValue = instantiateIndex % 2 == 0 ? 1f : 0;

        return new LocalTransform
        {
            Position = new float3(pos.x,pos.y,pos.z),
            Rotation = quaternion.RotateY(RotateTowards(pos, pos)),
            //Rotation = quaternion.Euler(new float3(0,0,0)),
            Scale = 1f
        };
    }

    private float RotateTowards(float3 objectsPosition, float3 targetPosition)
    {
        var x = objectsPosition.x - targetPosition.x;
        var y = objectsPosition.z - targetPosition.z;

        return math.atan2(x, y) + math.PI;
    }

    private float3 GetRandomPosition()
    {
        float3 randomPosition;

        do
        {
            randomPosition = new float3
            {
                x = _enemyFieldRandom.ValueRW.Value.NextFloat(-_enemyFieldPositionDatas.ValueRO.FieldDimensions.x / 2,
                    _enemyFieldPositionDatas.ValueRO.FieldDimensions.x / 2),
                y = 0.5f,
                z = _enemyFieldRandom.ValueRW.Value.NextFloat(-_enemyFieldPositionDatas.ValueRO.FieldDimensions.y / 2,
                0)
            };
        } while (math.distancesq(Transform.Position, randomPosition) <= 600);

        return randomPosition;
    }
}

