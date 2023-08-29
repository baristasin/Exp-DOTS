using System;
using Unity.Collections;
using Unity.Entities;

public partial struct ZombieInitializationSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        foreach (var zombie in SystemAPI.Query<ZombieWalkAspect>().WithAll<ZombieMoveTag>())
        {
            ecb.RemoveComponent<ZombieMoveTag>(zombie.Entity);
            ecb.SetComponentEnabled<ZombieWalkProperties>(zombie.Entity, false);
        }
    }
}

