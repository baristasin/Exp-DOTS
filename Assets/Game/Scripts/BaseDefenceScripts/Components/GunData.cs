using System;
using Unity.Entities;
using Unity.Mathematics;

public enum GunType
{
    Minigun,
    RocketLauncher
}

public struct GunData : IComponentData
{
    public Entity GunBulletPrefab;
    public Entity GunRenderer;
    public GunType GunType;
    public float GunShootInterval;
    public float2 GunPositionXZ;
}

