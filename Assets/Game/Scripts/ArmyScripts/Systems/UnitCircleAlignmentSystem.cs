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

        foreach (var unitCircleTransform in SystemAPI.Query<RefRW<LocalTransform>>().WithAll<UnitCircleData>())
        {
            unitCircleTransform.ValueRW.Position += new float3(0, 0, 1f) * deltaTime;
        }
    }


}
