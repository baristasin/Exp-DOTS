using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public readonly partial struct ZombieWalkAspect : IAspect
{
    public readonly Entity Entity;

    private readonly RefRW<LocalTransform> _transform;

    private readonly RefRO<ZombieWalkProperties> _zombieWalkProperties;

    private readonly RefRW<ZombieTimer> _zombieTimer;

    public float WalkTimer
    {
        get => _zombieTimer.ValueRO.TimerValue;
        set => _zombieTimer.ValueRW.TimerValue = value;
    }

    public void Walk(float deltaTime)
    {
        WalkTimer += deltaTime;
        _transform.ValueRW.Position += _transform.ValueRO.Forward() * _zombieWalkProperties.ValueRO.WalkSpeed * deltaTime;
    }

    public bool IsInStoppingRange(float3 brainPosition,float brainRadiusSq)
    {
        return math.distancesq(brainPosition, _transform.ValueRO.Position) <= brainRadiusSq;
    }
}

