using System;
using Unity.Entities;
using UnityEngine;

public readonly partial struct ZombieAttackAspect : IAspect
{
    public readonly Entity Entity;

    private readonly RefRO<ZombieAttackComponent> _zombieAttackComponent;

    public float AttackDamage => _zombieAttackComponent.ValueRO.AttackDamage;

    public void Attack(float deltaTime,EntityCommandBuffer.ParallelWriter parallelWriter,int sortKey,Entity brainEntity)
    {
        var eatDamage = AttackDamage * deltaTime;
        var brainDamage = new BrainDamageBufferElement { Value = eatDamage };
        parallelWriter.AppendToBuffer(sortKey, brainEntity, brainDamage);
    }

}

