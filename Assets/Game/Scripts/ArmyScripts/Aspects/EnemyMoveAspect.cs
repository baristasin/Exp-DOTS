using System;
using Game.Scripts;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public readonly partial struct EnemyMoveAspect : IAspect
{
    private readonly Entity _entity;

    private readonly RefRW<LocalTransform> _localTransform;
    private readonly RefRO<EnemyMovementData> _enemyMovementData;

    public void Move(float deltaTime)
    {
        _localTransform.ValueRW.Position += _localTransform.ValueRW.Forward() * _enemyMovementData.ValueRO.MoveSpeed * deltaTime;
    }
}

