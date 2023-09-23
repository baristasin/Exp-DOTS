using System;
using Unity.Entities;
using Unity.Mathematics;

public struct EnemyMovementData : IComponentData
{
    public float MoveSpeed;
    public float2 EnemyPositionXZ;
}

