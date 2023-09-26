using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

//[DisableAutoCreation]
[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct UnitCircleAlignmentSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<UnitCirclePropertiesData>();
        state.RequireForUpdate<InputData>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var deltaTime = SystemAPI.Time.DeltaTime;
        var unitCirclePropertiesEntity = SystemAPI.GetSingletonEntity<UnitCirclePropertiesData>();
        var unitCircleAspect = SystemAPI.GetAspect<UnitCirclePropertiesAspect>(unitCirclePropertiesEntity);

        if (unitCircleAspect.UnitCirclePropData.ValueRO.CurrentSelectedSoldierCount > 0 && Input.GetMouseButton(0)) // selected soldier count > 0 this means there are
                                                                                                                    //unit circles (not a good prediction), so if input on, we can align their positions
        {
            foreach (var unitCircleTransform in SystemAPI.Query<RefRW<LocalTransform>>().WithAll<UnitCircleData>())
            {
                unitCircleTransform.ValueRW.Position += new float3(0, 0, 1f) * deltaTime;
            }
        }
    }


}
