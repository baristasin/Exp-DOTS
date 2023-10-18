using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

//[DisableAutoCreation]
[UpdateInGroup(typeof(SimulationSystemGroup))]
[BurstCompile]
public partial struct SoldierEnemyMatchSystem : ISystem
{
    private NativeArray<LocalTransform> _localTransforms;
    private int _currentEnemyBattalionId;

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
                _localTransforms = new NativeArray<LocalTransform>(100,Allocator.Persistent);                
                _currentEnemyBattalionId = soldierChaseData.ValueRO.EnemyBattalionId;
            }

            int counter = 0;

            if (math.Equals(_localTransforms[0].Position,new float3(0,0,0)))
            {
                foreach (var (enemyLocalTransform, enemySoldierMovementData) in SystemAPI.Query<LocalTransform, SoldierMovementData>()
                    .WithSharedComponentFilter<SoldierBattalionIdData>(new SoldierBattalionIdData { BattalionId = soldierChaseData.ValueRO.EnemyBattalionId }))
                {
                    _localTransforms[counter] = enemyLocalTransform;
                    counter++;
                }
            }
            else
            {
                //if (_localTransforms.Length > soldierEntity.Index)
                //{
                //    soldierChaseData.ValueRW.EnemyLocalTransform = _localTransforms[soldierEntity.Index];
                //}
                //else
                //{
                //    soldierChaseData.ValueRW.EnemyLocalTransform = _localTransforms[Random.CreateFromIndex((uint)soldierEntity.Index).NextInt(0, _localTransforms.Length)];
                //}

                var currentEnemyLocalTransform = soldierChaseData.ValueRO.EnemyLocalTransform;

                for (int i = 0; i < _localTransforms.Length; i++)
                {
                    if (math.distance(soldierLocalTransform.ValueRO.Position,currentEnemyLocalTransform.Position)
                        > math.distance(soldierLocalTransform.ValueRO.Position, _localTransforms[i].Position))
                    {
                        soldierChaseData.ValueRW.EnemyLocalTransform = _localTransforms[i];
                    }
                }
            }
        }
    }
}
