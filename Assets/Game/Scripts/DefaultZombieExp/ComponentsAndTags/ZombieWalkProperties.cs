using System;
using Unity.Entities;

public struct ZombieWalkProperties : IComponentData,IEnableableComponent
{
    public float WalkSpeed;
    public float WalkAmplitude;
    public float WalkFrequency;
}

