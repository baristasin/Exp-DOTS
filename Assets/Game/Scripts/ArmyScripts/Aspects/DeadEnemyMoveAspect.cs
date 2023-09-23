using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public readonly partial struct DeadEnemyMoveAspect : IAspect
{
    public readonly Entity Entity => _entity;

    private readonly Entity _entity;

    private readonly RefRW<LocalTransform> _localTransform;
    private readonly RefRW<EnemyDeadTag> _enemyDeadTag;

    public void AnimateAndKillEnemy(EntityCommandBuffer ecb, float deltaTime)
    {
        _enemyDeadTag.ValueRW.TimeElapsed += deltaTime;

        _localTransform.ValueRW.Position.y += _enemyDeadTag.ValueRO.TimeElapsed * deltaTime * 3f;
        _localTransform.ValueRW.Position.z += _enemyDeadTag.ValueRO.TimeElapsed * deltaTime * 5f;

        _localTransform.ValueRW = _localTransform.ValueRW.RotateX(-_enemyDeadTag.ValueRO.TimeElapsed * deltaTime * 3f);


        if (_enemyDeadTag.ValueRO.TimeElapsed >= 5f)
        {
            ecb.DestroyEntity(_entity);
        }
    }
}

