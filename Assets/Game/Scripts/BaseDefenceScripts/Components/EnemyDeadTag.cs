using System;
using Unity.Entities;
using Unity.Mathematics;

public struct EnemyDeadTag : IComponentData,IEnableableComponent
{
    public float TimeElapsed;
    public float2 HitDirection;
}

