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
[UpdateInGroup(typeof(InitializationSystemGroup))]
[UpdateAfter(typeof(UnitCirclePlacementSystem))]
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
        int counter = 0;

        foreach (var unitCircleTransform in SystemAPI.Query<RefRW<LocalTransform>>().WithAll<UnitCircleData>())
        {
            var inputDataEntity = SystemAPI.GetSingletonEntity<InputData>();
            var inputData = SystemAPI.GetComponent<InputData>(inputDataEntity);

            var alignmentDirectionNormalized = math.normalize(inputData.GroundInputPos - inputData.GroundInputStartingPos);
            float angleBetweenVectors = 0;
            if (inputData.GroundInputPos.z <= inputData.GroundInputStartingPos.z)
            {
                angleBetweenVectors = Vector3.Angle(Vector3.right, alignmentDirectionNormalized);
            }
            else
            {
                angleBetweenVectors = -1f * Vector3.Angle(Vector3.right, alignmentDirectionNormalized);
            }

            unitCircleTransform.ValueRW.Position = inputData.GroundInputStartingPos + (counter * alignmentDirectionNormalized);
            unitCircleTransform.ValueRW.Rotation = quaternion.Euler(0, Mathf.Deg2Rad * angleBetweenVectors, 0);
            counter++;
        }
    }
}
