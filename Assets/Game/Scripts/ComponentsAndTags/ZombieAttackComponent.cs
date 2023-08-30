using System;
using Unity.Entities;

public struct ZombieAttackComponent : IComponentData,IEnableableComponent
{
    public float AttackDamage;
    public float AttackInterval;
}

