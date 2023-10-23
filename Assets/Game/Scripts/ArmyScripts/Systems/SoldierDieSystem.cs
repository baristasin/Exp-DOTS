using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;


//[DisableAutoCreation]
[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(SoldierEnemyBattleSystem))]
[BurstCompile]
public partial struct SoldierDieSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        state.CompleteDependency();

        new GetDamageJob().Run();

        var ecb = new EntityCommandBuffer(Allocator.Temp);
        foreach (var (soldierChaseData, soldierEntity) in SystemAPI.Query<RefRW<SoldierChaseData>>().WithEntityAccess())
        {
            if (soldierChaseData.ValueRO.EnemyEntity != Entity.Null)
            {
                var healthData = state.EntityManager.GetComponentData<SoldierHealthData>(soldierChaseData.ValueRO.EnemyEntity);
                if (healthData.Health < 0)
                {
                    ecb.DestroyEntity(soldierChaseData.ValueRO.EnemyEntity);
                    soldierChaseData.ValueRW.EnemyEntity = Entity.Null;
                }
            }
        }

        ecb.Playback(state.EntityManager);
    }
}

[BurstCompile]
public partial struct GetDamageJob : IJobEntity
{
    public void Execute(SoldierAspect soldierAspect)
    {
        soldierAspect.GetDamage();
    }
}
