using System;
using Unity.Entities;

public struct BrainHealth : IComponentData
{
    public float MaxHealth;
    public float CurrentHealth;
}

