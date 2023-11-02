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

    //[BurstCompile]
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
        float3 enemyPositionAligned = new float3(soldierChaseData.EnemyLocalTransform.Position.x, soldierAspect.SoldierTransform.ValueRO.Position.y, soldierChaseData.EnemyLocalTransform.Position.z);

        if (math.distance(soldierAspect.SoldierTransform.ValueRO.Position, enemyPositionAligned) > 2f) // Chase
        {
            soldierAspect.SoldierMovementData.ValueRW.TargetPosition = enemyPositionAligned;
            soldierAspect.SoldierMovementData.ValueRW.TargetRotation = quaternion.RotateY(RotateTowards(soldierAspect.SoldierTransform.ValueRO.Position, enemyPositionAligned));
        }
        else // Attack
        {
            if (EntityManager.HasBuffer<SoldierDamageBufferElement>(soldierChaseData.EnemyEntity))
            {
                soldierAspect.SoldierMovementData.ValueRW.TargetRotation = quaternion.RotateY(RotateTowards(soldierAspect.SoldierTransform.ValueRO.Position, enemyPositionAligned));

                EntityCommandBuffer.AppendToBuffer(soldierChaseData.EnemyEntity, new SoldierDamageBufferElement { DamageAmount = 1 });

                if (EntityManager.HasComponent<SoldierChaseData>(soldierChaseData.EnemyEntity) == false)
                {
                    EntityCommandBuffer.AddComponent(soldierChaseData.EnemyEntity, new SoldierChaseData
                    {
                        EnemyBattalionId = soldierAspect.SoldierBattalionIdData.BattalionId,
                        EnemyEntity = soldierAspect.Entity,
                        EnemyLocalTransform = soldierAspect.SoldierTransform.ValueRW
                    });
                }
                soldierAspect.SoldierAnimationData.ValueRW.AnimationType = AnimationType.Attacking;
            }
        }
    }

    private float RotateTowards(float3 objectsPosition, float3 targetPosition)
    {
        var x = objectsPosition.x - targetPosition.x;
        var y = objectsPosition.z - targetPosition.z;

        return math.atan2(x, y) + math.PI;
    }
}

