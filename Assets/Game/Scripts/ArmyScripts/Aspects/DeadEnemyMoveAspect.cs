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
            _localTransform.ValueRW.Rotation = quaternion.RotateY(RotateTowards(_localTransform.ValueRO.Position, new float3(_enemyDeadTag.ValueRO.HitDirection.x
    , _localTransform.ValueRO.Position.y, _enemyDeadTag.ValueRO.HitDirection.y)));
        }

        _enemyDeadTag.ValueRW.TimeElapsed += deltaTime;

        _localTransform.ValueRW.Position.x += _enemyDeadTag.ValueRO.HitDirection.x * deltaTime * 10f;

        if (_enemyDeadTag.ValueRO.TimeElapsed <= 2.5f)
        {
            _localTransform.ValueRW.Position.y += 3f * deltaTime;
        }
        else
        {
            _localTransform.ValueRW.Position.y -= 3f * deltaTime;

        }

        _localTransform.ValueRW.Position.z += _enemyDeadTag.ValueRO.HitDirection.y * deltaTime * 14f;

        _localTransform.ValueRW = _localTransform.ValueRO.RotateX(Random.CreateFromIndex((uint)_entity.Index).NextFloat(-12f, -3f) * deltaTime);


        if (_enemyDeadTag.ValueRO.TimeElapsed >= 5f)
        {
            ecb.DestroyEntity(_entity);
        }
    }

    private float RotateTowards(float3 objectsPosition, float3 targetPosition)
    {
        var x = objectsPosition.x - targetPosition.x;
        var y = objectsPosition.z - targetPosition.z;

        return math.atan2(x, y) + math.PI;
    }
}

