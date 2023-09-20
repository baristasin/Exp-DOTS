using System;
using Game.Scripts;
using Unity.Burst;
using Unity.Entities;

[DisableAutoCreation]
[UpdateInGroup(typeof(SimulationSystemGroup),OrderLast = true)]
[UpdateAfter(typeof(EndSimulationEntityCommandBufferSystem))]
public partial struct ApplyBrainDamageSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<BrainTag>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var brainEntity = SystemAPI.GetSingletonEntity<BrainTag>();
        var brainAspect = SystemAPI.GetAspect<BrainAspect>(brainEntity);

        brainAspect.DamageBrain();
    }
}

