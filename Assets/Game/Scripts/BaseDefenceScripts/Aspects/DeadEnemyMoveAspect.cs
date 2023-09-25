using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using static UnityEditor.FilePathAttribute;

public readonly partial struct DeadEnemyMoveAspect : IAspect
{
    public readonly Entity Entity => _entity;

    private readonly Entity _entity;

    private readonly RefRW<LocalTransform> _localTransform;
    private readonly RefRW<EnemyDeadTag> _enemyDeadTag;

    public void AnimateAndKillEnemy(EntityCommandBuffer ecb, float deltaTime)
    {
        if (_enemyDeadTag.ValueRO.TimeElapsed == 0)
        {
        }

        _enemyDeadTag.ValueRW.TimeElapsed += deltaTime;

        if (_enemyDeadTag.ValueRO.TimeElapsed <= 0.75f)
        {
            _localTransform.ValueRW.Position.y += 15 * deltaTime;
            _localTransform.ValueRW = _localTransform.ValueRO.RotateX(deltaTime * -4f);
        }
        else
        {
            _localTransform.ValueRW.Position.y -= 10 * deltaTime;
        }

        _localTransform.ValueRW.Position.z += deltaTime * 15;

        //_localTransform.ValueRW.Rotation = quaternion.RotateX(RotateTowards(_localTransform.ValueRO.Position, new float3(_localTransform.ValueRO.Position.x, _localTransform.ValueRO.Position.y + 20f, _localTransform.ValueRO.Position.z)));

        if (_enemyDeadTag.ValueRO.TimeElapsed >= 5f)
        {
            ecb.DestroyEntity(_entity);
        }
    }
}

