using System;
using Unity.Entities;

public struct MinigunBullet : IComponentData
{
    public float BulletSpeed;
    public float MaxLifeTimeValue;
    public float CurrentLifeTimeValue;
}

