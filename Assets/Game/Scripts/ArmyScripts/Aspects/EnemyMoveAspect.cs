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
    private readonly RefRW<EnemyMovementData> _enemyMovementData;

    public void Move(float deltaTime)
    {
        _localTransform.ValueRW.Position += _localTransform.ValueRW.Forward() * _enemyMovementData.ValueRO.MoveSpeed * deltaTime;
        _enemyMovementData.ValueRW.EnemyPositionXZ = new float2(_localTransform.ValueRO.Position.x, _localTransform.ValueRO.Position.z);
    }
}

