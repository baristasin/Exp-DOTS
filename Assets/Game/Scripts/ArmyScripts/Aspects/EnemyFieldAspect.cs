using System;
using Game.Scripts;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public readonly partial struct EnemyFieldAspect : IAspect
{
    public readonly Entity Entity;
    public readonly RefRO<EnemyFieldProperties> EnemyFieldProperties => _enemyFieldProperties;
    public readonly RefRW<EnemyFieldRandom> EnemyFieldRandom => _enemyFieldRandom;

    private readonly RefRO<LocalTransform> _transform;
    private LocalTransform Transform => _transform.ValueRO;

    private readonly RefRO<EnemyFieldProperties> _enemyFieldProperties;
    private readonly RefRW<EnemyFieldRandom> _enemyFieldRandom;

    public LocalTransform GetRandomEnemyTransform()
    {
        var position = GetRandomPosition();

        return new LocalTransform
        {
            Position = position,
            Rotation = quaternion.RotateY(RotateTowards(position,Transform.Position)),
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
                x = _enemyFieldRandom.ValueRW.Value.NextFloat(-_enemyFieldProperties.ValueRO.FieldDimensions.x / 2,
                    _enemyFieldProperties.ValueRO.FieldDimensions.x / 2),
                y = 0.5f,
                z = _enemyFieldRandom.ValueRW.Value.NextFloat(-_enemyFieldProperties.ValueRO.FieldDimensions.y / 2,
                    _enemyFieldProperties.ValueRO.FieldDimensions.y / 2)
            };
        } while (math.distancesq(Transform.Position, randomPosition) <= 50);

        return randomPosition;
    }

    private quaternion GetRandomRotation()
    {
        return quaternion.RotateY(_enemyFieldRandom.ValueRW.Value.NextFloat(-0.25f, 0.25f));
    }

    private float GetRandomScale(float baseNumber)
    {
        return _enemyFieldRandom.ValueRW.Value.NextFloat(baseNumber, 1f);
    }
}

