using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

//[DisableAutoCreation]
[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(SoldierEnemyMatchSystem))]
[BurstCompile]
public partial struct SoldierEnemyMoveSystem : ISystem
{
    private EntityQuery _chaseSoldiersQuery;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        _chaseSoldiersQuery = new EntityQueryBuilder(state.WorldUpdateAllocator).WithAll<SoldierChaseData>().WithAspect<SoldierAspect>().Build(ref state);
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var deltaTime = SystemAPI.Time.DeltaTime;

        new ChaseEnemyJob
        {
            DeltaTime = deltaTime
        }.ScheduleParallel(_chaseSoldiersQuery);
    }
}

public partial struct ChaseEnemyJob : IJobEntity
{
    public float DeltaTime;

    public void Execute(SoldierAspect soldierAspect,SoldierChaseData soldierChaseData)
    {
        soldierAspect.SoldierMovementData.ValueRW.TargetPosition = soldierChaseData.EnemyLocalTransform.Position;
    }
}
