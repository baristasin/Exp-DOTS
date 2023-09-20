using System;
using Unity.Entities;

public enum GunType
{
    Minigun,
    RocketLauncher
}

public struct GunData : IComponentData
{
    public Entity GunBulletPrefab;
    public GunType GunType;
    public float GunShootInterval;
}

