using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

//[DisableAutoCreation]
[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(SoldierEnemyMatchSystem))]
[BurstCompile]
public partial struct SoldierEnemyBattleSystem : ISystem
{
    //private EntityQuery _chaseSoldiersQuery;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        //_chaseSoldiersQuery = new EntityQueryBuilder(state.WorldUpdateAllocator).WithAll<SoldierChaseData>().WithAspect<SoldierAspect>().Build(ref state);
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        state.CompleteDependency();

        var deltaTime = SystemAPI.Time.DeltaTime;
        var ecb = new EntityCommandBuffer(Allocator.TempJob);

        new ChaseEnemyJob
        {
            EntityManager = state.EntityManager,
            EntityCommandBuffer = ecb,
            DeltaTime = deltaTime
        }.Run();

        ecb.Playback(state.EntityManager);

    }
}

public partial struct ChaseEnemyJob : IJobEntity
{
    public EntityManager EntityManager;
    public EntityCommandBuffer EntityCommandBuffer;
    public float DeltaTime;

    public void Execute(SoldierAspect soldierAspect, SoldierChaseData soldierChaseData)
    {
        if (math.distance(soldierAspect.SoldierTransform.ValueRO.Position, soldierChaseData.EnemyLocalTransform.Position) > 1f) // Chase
        {
            soldierAspect.SoldierMovementData.ValueRW.TargetPosition = soldierChaseData.EnemyLocalTransform.Position;
        }
        else // Attack
        {
            if (EntityManager.HasBuffer<SoldierDamageBufferElement>(soldierChaseData.EnemyEntity))
            {
                EntityCommandBuffer.AppendToBuffer(soldierChaseData.EnemyEntity, new SoldierDamageBufferElement { DamageAmount = 1 });
            }
        }
    }
}
