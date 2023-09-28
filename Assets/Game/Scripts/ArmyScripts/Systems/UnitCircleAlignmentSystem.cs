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
        state.RequireForUpdate<GroundInputData>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        int counter = 0;
        int lineIndex = 0;
        int lineMaximumSoldierCount = 10;

        foreach (var unitCircleTransform in SystemAPI.Query<RefRW<LocalTransform>>().WithAll<UnitCircleData>())
        {
            var inputDataEntity = SystemAPI.GetSingletonEntity<GroundInputData>();
            var inputDataAspect = SystemAPI.GetAspect<GroundInputAspect>(inputDataEntity);

            var alignmentDirectionNormalized = math.normalize(inputDataAspect.InputData.ValueRO.GroundInputPos - inputDataAspect.InputData.ValueRO.GroundInputStartingPos) * 2f;
            float angleBetweenVectors = 0;

            // Q1 - Determining facing direction, is user dragged to right or left, if right => they should look forward
            if (inputDataAspect.InputData.ValueRO.GroundInputPos.z <= inputDataAspect.InputData.ValueRO.GroundInputStartingPos.z)
            {
                angleBetweenVectors = Vector3.Angle(Vector3.right, alignmentDirectionNormalized);
            }
            else
            {
                angleBetweenVectors = -1f * Vector3.Angle(Vector3.right, alignmentDirectionNormalized);
            }
            // Q1 - Ends

            // Q2 - Align formation via calculating drag distance
            var distanceBetweenInputs = Vector3.Distance(inputDataAspect.InputData.ValueRO.GroundInputPos, inputDataAspect.InputData.ValueRO.GroundInputStartingPos);

            if ((int)distanceBetweenInputs - 3 > 1)
            {
                lineMaximumSoldierCount = 10 + (((int)distanceBetweenInputs - 3));
            }

            if(distanceBetweenInputs - 3f <= -0.3f)
            {
                if(distanceBetweenInputs < 0.9f)
                {
                    distanceBetweenInputs = 0.9f;
                }
                lineMaximumSoldierCount = 10 - (10 - (int)(distanceBetweenInputs / 0.3f));
            }

            if (counter >= lineMaximumSoldierCount && counter % lineMaximumSoldierCount == 0)
            {
                counter = 0;
                lineIndex++;
            }

            // Q2 - Ends

            // Q3 - Counter determines soldier horizontal order
            unitCircleTransform.ValueRW.Position = inputDataAspect.InputData.ValueRO.GroundInputStartingPos + (counter * alignmentDirectionNormalized);
            // Q3 - Ends

            // Q4 - Line Index determines soldier vertical order
            alignmentDirectionNormalized.x *= -1;
            float3 rotated90DegreesVector = new float3(alignmentDirectionNormalized.z,0, alignmentDirectionNormalized.x);
            unitCircleTransform.ValueRW.Position += rotated90DegreesVector * lineIndex;
            // Q4 - Ends

            // Q5 - Determines facing direction via calculating the angle direction vector and vector3 right
            unitCircleTransform.ValueRW.Rotation = quaternion.Euler(0, Mathf.Deg2Rad * angleBetweenVectors, 0);
            // Q5 - Ends
            counter++;
        }
    }
}
