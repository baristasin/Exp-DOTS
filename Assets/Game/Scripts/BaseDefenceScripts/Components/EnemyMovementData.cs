using System;
using Unity.Entities;
using Unity.Mathematics;

public struct EnemyMovementData : IComponentData,IEnableableComponent
{
    public float MoveSpeed;
    public float2 EnemyPositionXZ;
}

