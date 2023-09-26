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

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<UnitCirclePropertiesData>();

        _unitCircleQuery = new EntityQueryBuilder(state.WorldUpdateAllocator).WithAll<UnitCircleData>().WithAll<LocalTransform>().Build(ref state);
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

        if (unitCirclePropsAspect.UnitCirclePropData.ValueRO.CurrentSelectedSoldierCount <= 0
            && IsCircleUnitsInstantiated == 1) // if chosen soldier count = 0 and there are circleunits instantiated, clear all
        {
            var ecbSingleton = SystemAPI.GetSingleton<EndInitializationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            new DestroyAllUnitCirclesJob
            {
                ecb = ecb
            }.Schedule();

            IsCircleUnitsInstantiated = 0;
        }
        else // selected count > 0 and circles not instantiated
        {
            if (Input.GetMouseButton(0) && IsCircleUnitsInstantiated == 0)
            {

                var inputDataEntity = SystemAPI.GetSingletonEntity<InputData>();
                var inputData = SystemAPI.GetComponent<InputData>(inputDataEntity);

                if (math.distance(inputData.GroundInputStartingPos, inputData.GroundInputPos) > 3f)
                {
                    var ecbSingleton = SystemAPI.GetSingleton<EndInitializationEntityCommandBufferSystem.Singleton>();
                    var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

                    for (int i = 0; i < unitCirclePropsAspect.UnitCirclePropData.ValueRO.CurrentSelectedSoldierCount; i++)
                    {
                        var handle = new CreateUnitCirclesJob
                        {
                            Counter = i,
                            UnitCircleEntity = unitCirclePropsAspect.UnitCircleEntity,
                            ecb = ecb
                        }.Schedule();

                        handle.Complete();
                    }


                    IsCircleUnitsInstantiated = 1;

                    //for (int i = 0; i < unitCirclePropsAspect.UnitCirclePropData.ValueRO.CurrentSelectedSoldierCount; i++)
                    //{
                    //    var unitCircleEntity = ecb.Instantiate(unitCirclePropsAspect.UnitCircleEntity);

                    //    LocalTransform localTransform = new LocalTransform
                    //    {
                    //        Position = new float3(i, 0.5f, 0),
                    //        Rotation = quaternion.identity,
                    //        Scale = 1f
                    //    };

                    //    ecb.AddComponent(unitCircleEntity, localTransform);
                    //}

                    //IsCircleUnitsInstantiated = 1;
                }
            }
        }

        if (Input.GetMouseButtonUp(0)) // mouse up, clear all again
        {
            var ecbSingleton = SystemAPI.GetSingleton<EndInitializationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            //foreach (var (unitCircleData, unitCircleTransform) in SystemAPI.Query<RefRO<UnitCircleData>, RefRW<LocalTransform>>())
            //{
            //    ecb.AppendToBuffer(unitCirclePropertiesEntity, new UnitCirclePlacementBufferElementData
            //    {
            //        UnitCirclePosXZ = new float2(unitCircleTransform.ValueRO.Position.x, unitCircleTransform.ValueRO.Position.z),
            //    });
            //}

            new AddUnitCirclesToGeneralBufferJob
            {
                UnitCirclePropertiesEntity = unitCirclePropertiesEntity,
                Ecb = ecb
            }.Schedule(_unitCircleQuery);

            new DestroyAllUnitCirclesJob
            {
                ecb = ecb
            }.Schedule(_unitCircleQuery);

            IsCircleUnitsInstantiated = 0;
        }

    }
}

[BurstCompile]
public partial struct CreateUnitCirclesJob : IJob
{
    public Entity UnitCircleEntity;
    public int Counter;
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
    public void Execute(Entity entity, LocalTransform unitCircleTransform)
    {
        Ecb.AppendToBuffer<UnitCirclePlacementBufferElementData>(UnitCirclePropertiesEntity, new UnitCirclePlacementBufferElementData
        {
            UnitCirclePosXZ = new float2(unitCircleTransform.Position.x, unitCircleTransform.Position.z),
        });
    }
}