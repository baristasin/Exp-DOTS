using System;
using Unity.Entities;

public struct GunTimer : IComponentData
{
    public float ShootInterval;
    public float CurrentTimerValue;
}

