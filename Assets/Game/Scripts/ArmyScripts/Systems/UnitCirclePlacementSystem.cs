using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

//[DisableAutoCreation]
[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial struct UnitCirclePlacementSystem : ISystem
{
    public int IsCircleUnitsInstantiated;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<UnitCirclePropertiesData>();
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
            // Destroy all CircleUnits
            // IsCircleUnitsInstantiated = 0
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (unitCircleData, unitCircleEntity) in SystemAPI.Query<RefRO<UnitCircleData>>().WithEntityAccess())
            {
                ecb.DestroyEntity(unitCircleEntity);
            }

            IsCircleUnitsInstantiated = 0;
            ecb.Playback(state.EntityManager);
        }
        else // selected count > 0 and circles not instantiated
        {
            if (Input.GetMouseButton(0) && IsCircleUnitsInstantiated == 0)
            {
                var inputDataEntity = SystemAPI.GetSingletonEntity<InputData>();
                var inputData = SystemAPI.GetComponent<InputData>(inputDataEntity);

                Debug.Log($"start: {inputData.GroundInputStartingPos}, current: {inputData.GroundInputPos}");

                if (math.distance(inputData.GroundInputStartingPos, inputData.GroundInputPos) > 3f)
                {
                    var ecb = new EntityCommandBuffer(Allocator.Temp);

                    for (int i = 0; i < unitCirclePropsAspect.UnitCirclePropData.ValueRO.CurrentSelectedSoldierCount; i++)
                    {
                        var unitCircleEntity = ecb.Instantiate(unitCirclePropsAspect.UnitCircleEntity);

                        LocalTransform localTransform = new LocalTransform
                        {
                            Position = new float3(i, 0.5f, 0),
                            Rotation = quaternion.identity,
                            Scale = 1f
                        };

                        ecb.AddComponent(unitCircleEntity, localTransform);
                    }
                    IsCircleUnitsInstantiated = 1;

                    ecb.Playback(state.EntityManager);
                }
            }
        }

        if (Input.GetMouseButtonUp(0)) // mouse up, clear all again
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (unitCircleData, unitCircleTransform) in SystemAPI.Query<RefRO<UnitCircleData>, RefRW<LocalTransform>>())
            {
                ecb.AppendToBuffer(unitCirclePropertiesEntity, new UnitCirclePlacementBufferElementData
                {
                    UnitCirclePosXZ = new float2(unitCircleTransform.ValueRO.Position.x, unitCircleTransform.ValueRO.Position.z),
                });
            }

            // Destroy all CircleUnits
            // IsCircleUnitsInstantiated = 0

            foreach (var (unitCircleData, unitCircleEntity) in SystemAPI.Query<RefRO<UnitCircleData>>().WithEntityAccess())
            {
                ecb.DestroyEntity(unitCircleEntity);
            }

            IsCircleUnitsInstantiated = 0;
            ecb.Playback(state.EntityManager);
        }

    }
}
