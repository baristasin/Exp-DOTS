using System;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public readonly partial struct GunAspect : IAspect
{
    public readonly Entity Entity;

    public readonly RefRO<GunData> GunData => _gunData;
    public readonly RefRW<GunTimer> GunTimer => _gunTimer;
    public readonly RefRO<LocalTransform> LocalTransform => _localTransform;
    public readonly Entity GunBulletEntity => _gunData.ValueRO.GunBulletPrefab;

    public float CurrentTimerValue
    {
        get => _gunTimer.ValueRW.CurrentTimerValue;
        set => _gunTimer.ValueRW.CurrentTimerValue = value;
    }

    public float ShootInterval => _gunTimer.ValueRO.ShootInterval;
              
    private readonly RefRO<GunData> _gunData;
    private readonly RefRW<GunTimer> _gunTimer;
    private readonly RefRO<LocalTransform> _localTransform;

}

