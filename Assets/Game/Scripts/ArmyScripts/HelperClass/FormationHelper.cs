using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public static class FormationHelper
{
    [BurstCompile]
    public static void FormationFuncLocalTransform(GroundInputAspect inputDataAspect, ref SystemState state, ref int counter, ref int lineIndex, ref int lineMaximumSoldierCount, RefRW<LocalTransform> unitTransform, RefRW<UnitCircleData> unitCircleData,int battalionId)
    {
        var alignmentDirectionNormalized = math.normalize(inputDataAspect.InputData.ValueRO.GroundInputPos - inputDataAspect.InputData.ValueRO.GroundInputStartingPos) * 2f;
        float angleBetweenVectors;

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

        if (distanceBetweenInputs - 3f <= -0.3f)
        {
            if (distanceBetweenInputs < 0.9f)
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
        unitTransform.ValueRW.Position = inputDataAspect.InputData.ValueRO.GroundInputStartingPos + (counter * alignmentDirectionNormalized * 0.7f);
        // Q3 - Ends

        unitTransform.ValueRW.Position += alignmentDirectionNormalized * battalionId * 2f * math.clamp(distanceBetweenInputs,3,100);

        // Q4 - Line Index determines soldier vertical order
        alignmentDirectionNormalized.x *= -1;
        float3 rotated90DegreesVector = new float3(alignmentDirectionNormalized.z, 0, alignmentDirectionNormalized.x);
        unitTransform.ValueRW.Position += rotated90DegreesVector * lineIndex * 0.7f;
        // Q4 - Ends

        // Q5 - Determines facing direction via calculating the angle direction vector and vector3 right
        unitTransform.ValueRW.Rotation = quaternion.Euler(0, Mathf.Deg2Rad * angleBetweenVectors, 0);
        // Q5 - Ends

        unitCircleData.ValueRW.UnitCircleCounterAndLineValues = new float2(counter, lineIndex);

        counter++;
    }
}