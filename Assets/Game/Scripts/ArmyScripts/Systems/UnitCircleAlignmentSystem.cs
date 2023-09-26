using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct UnitCircleAlignmentSystem : ISystem
{
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
        var deltaTime = SystemAPI.Time.DeltaTime;
        var unitCirclePropertiesEntity = SystemAPI.GetSingletonEntity<UnitCirclePropertiesData>();
        var unitCircleAspect = SystemAPI.GetAspect<UnitCirclePropertiesAspect>(unitCirclePropertiesEntity);

        if (unitCircleAspect.UnitCirclePropData.ValueRO.CurrentSelectedSoldierCount > 0 && Input.GetMouseButton(0))
        {
            foreach (var (unitCircleData, unitCircleTransform) in SystemAPI.Query<RefRO<UnitCircleData>, RefRW<LocalTransform>>())
            {
                unitCircleTransform.ValueRW.Position += new float3(0, 0, 1) * deltaTime;
            }
        }
    }
}
