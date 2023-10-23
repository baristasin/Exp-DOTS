using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

//[DisableAutoCreation]
[UpdateInGroup(typeof(SimulationSystemGroup))]
[BurstCompile]
public partial struct SoldierEnemyMatchSystem : ISystem
{
    private NativeArray<LocalTransform> _localTransforms;
    private int _currentEnemyBattalionId;
    private int _soldierCount;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        _currentEnemyBattalionId = -1;
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (soldierLocalTransform, soldierMovementData, soldierChaseData, soldierEntity) in SystemAPI.Query<RefRW<LocalTransform>, SoldierMovementData, RefRW<SoldierChaseData>>().WithEntityAccess())
        {
            if (_currentEnemyBattalionId != soldierChaseData.ValueRO.EnemyBattalionId)
            {
                _currentEnemyBattalionId = soldierChaseData.ValueRO.EnemyBattalionId;
            }

            if (_soldierCount == 0)
            {
                foreach (var (enemyLocalTransform, enemySoldierMovementData) in SystemAPI.Query<LocalTransform, SoldierMovementData>()
                    .WithSharedComponentFilter<SoldierBattalionIdData>(new SoldierBattalionIdData { BattalionId = soldierChaseData.ValueRO.EnemyBattalionId }))
                {
                    _soldierCount++;
                }

                _localTransforms = new NativeArray<LocalTransform>(_soldierCount, Allocator.Persistent);
                int counter = 0;

                foreach (var (enemyLocalTransform, enemySoldierMovementData) in SystemAPI.Query<LocalTransform, SoldierMovementData>()
    .WithSharedComponentFilter<SoldierBattalionIdData>(new SoldierBattalionIdData { BattalionId = soldierChaseData.ValueRO.EnemyBattalionId }))
                {
                    _localTransforms[counter] = enemyLocalTransform;
                    counter++;
                }
            }
            else
            {
                var currentEnemyLocalTransform = soldierChaseData.ValueRO.EnemyLocalTransform;

                for (int i = 0; i < _localTransforms.Length; i++)
                {
                    if (math.distance(soldierLocalTransform.ValueRO.Position, currentEnemyLocalTransform.Position)
                        > math.distance(soldierLocalTransform.ValueRO.Position, _localTransforms[i].Position))
                    {
                        soldierChaseData.ValueRW.EnemyLocalTransform = _localTransforms[i];
                    }
                }
            }
        }
        _soldierCount = 0;
    }
}
