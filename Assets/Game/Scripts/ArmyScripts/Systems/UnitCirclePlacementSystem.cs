using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

//[DisableAutoCreation]
[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial struct UnitCirclePlacementSystem : ISystem
{
    public int IsCircleUnitsInstantiated;

    private EntityQuery _unitCircleQuery;

    public BufferLookup<UnitCircleSelectedBattalionAndCountBufferElementData> _selectedBattalionAndCountBufferLookup;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<UnitCirclePropertiesData>();

        _unitCircleQuery = new EntityQueryBuilder(state.WorldUpdateAllocator).WithAll<UnitCircleData>().WithAll<LocalTransform>().Build(ref state);

        _selectedBattalionAndCountBufferLookup = state.GetBufferLookup<UnitCircleSelectedBattalionAndCountBufferElementData>(true);
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var unitCirclePropertiesEntity = SystemAPI.GetSingletonEntity<UnitCirclePropertiesData>();
        var unitCirclePropsAspect = SystemAPI.GetAspect<UnitCirclePropertiesAspect>(unitCirclePropertiesEntity);

        var inputDataEntity = SystemAPI.GetSingletonEntity<GroundInputData>();
        var inputDataAspect = SystemAPI.GetAspect<GroundInputAspect>(inputDataEntity);

        if (unitCirclePropsAspect.UnitCirclePropData.ValueRO.CurrentSelectedSoldierCount <= 0) return;

        if (inputDataAspect.InputData.ValueRO.IsDragging == 1) // Dragging
        {

            if (IsCircleUnitsInstantiated == 1) return;

            if (math.distance(inputDataAspect.InputData.ValueRO.GroundInputStartingPos, inputDataAspect.InputData.ValueRO.GroundInputPos) > 3f)
            {
                _selectedBattalionAndCountBufferLookup.Update(ref state);

                var ecbSingleton = SystemAPI.GetSingleton<EndInitializationEntityCommandBufferSystem.Singleton>();
                var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

                //for (int i = 0; i < unitCirclePropsAspect.UnitCirclePropData.ValueRO.CurrentSelectedSoldierCount; i++)
                //{
                //    var handle = new CreateUnitCirclesJob
                //    {
                //        Counter = i,
                //        UnitCircleEntity = unitCirclePropsAspect.UnitCircleEntity,
                //        ecb = ecb
                //    }.Schedule();

                //    handle.Complete();
                //}

                if(_selectedBattalionAndCountBufferLookup.TryGetBuffer(unitCirclePropertiesEntity, out var buffer))
                {
                    for (int i = 0; i < buffer.Length; i++)
                    {
                        for (int j = 0; j < buffer[i].SoldierCount; j++)
                        {
                            var handle = new CreateUnitCirclesJob
                            {
                                Counter = i,
                                UnitCircleEntity = unitCirclePropsAspect.UnitCircleEntity,
                                BattalionId = buffer[i].BattalionId,
                                ecb = ecb
                            }.Schedule();

                            handle.Complete();
                        }
                    }
                }

                IsCircleUnitsInstantiated = 1;
            }
        }
        else // Not Dragging
        {
            if (IsCircleUnitsInstantiated == 0)
            {               
                return;
            }

            var ecbSingleton = SystemAPI.GetSingleton<EndInitializationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            ecb.AddBuffer<UnitCirclePlacementBufferElementData>(unitCirclePropertiesEntity);

            new AddUnitCirclesToGeneralBufferJob
            {
                UnitCirclePropertiesEntity = unitCirclePropertiesEntity,
                Ecb = ecb
            }.Run();

            new DestroyAllUnitCirclesJob
            {
                ecb = ecb
            }.Schedule(_unitCircleQuery);

            IsCircleUnitsInstantiated = 0;

            unitCirclePropsAspect.UnitCirclePropData.ValueRW.IsBufferLoadedWithPositions = 1;
        }
    }
}

[BurstCompile]
public partial struct CreateUnitCirclesJob : IJob
{
    public Entity UnitCircleEntity;
    public int Counter;
    public int BattalionId;
    public EntityCommandBuffer ecb;

    [BurstCompile]
    public void Execute()
    {
        var unitCircleEntity = ecb.Instantiate(UnitCircleEntity);

        LocalTransform localTransform = new LocalTransform
        {
            Position = new float3(Counter, 0.5f, 0),
            Rotation = quaternion.identity,
            Scale = 1f
        };

        ecb.AddComponent(unitCircleEntity, localTransform);
        ecb.AddSharedComponent(unitCircleEntity, new UnitCircleBattalionIdData { BattalionId = BattalionId });
    }

}

[BurstCompile]
public partial struct DestroyAllUnitCirclesJob : IJobEntity
{
    public EntityCommandBuffer ecb;

    [BurstCompile]
    public void Execute(Entity entity)
    {
        ecb.DestroyEntity(entity);
    }
}

[BurstCompile]
public partial struct AddUnitCirclesToGeneralBufferJob : IJobEntity
{
    public Entity UnitCirclePropertiesEntity;
    public EntityCommandBuffer Ecb;

    [BurstCompile]
    public void Execute(Entity entity, LocalTransform unitCircleTransform,UnitCircleData unitCircleData,UnitCircleBattalionIdData unitCircleBattalionIdData)
    {
        Ecb.AppendToBuffer<UnitCirclePlacementBufferElementData>(UnitCirclePropertiesEntity, new UnitCirclePlacementBufferElementData
        {
            UnitCirclePosXZ = new float2(unitCircleTransform.Position.x, unitCircleTransform.Position.z),
            UnitCircleRotation = unitCircleTransform.Rotation,
            BattalionId = unitCircleBattalionIdData.BattalionId,
            UnitCircleCounterAndLineIndexValues = new float2(unitCircleData.UnitCircleCounterAndLineValues.x,unitCircleData.UnitCircleCounterAndLineValues.y)
        });
    }
}