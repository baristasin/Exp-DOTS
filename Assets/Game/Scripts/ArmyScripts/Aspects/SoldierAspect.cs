using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public readonly partial struct SoldierAspect : IAspect
{
    public readonly Entity Entity;

    public readonly RefRW<SoldierMovementData> SoldierMovementData;

    public readonly RefRW<LocalTransform> SoldierTransform;

    public readonly RefRW<SoldierHealthData> SoldierHealthData;

    public readonly DynamicBuffer<SoldierDamageBufferElement> SoldierDamageBuffer;

    public void Move(byte IsInstantMove, float deltaTime, float distToTarget, int isOrderedToMove)
    {
        if (IsInstantMove == 0)
        {
            if (SoldierMovementData.ValueRW.IsOrderTaken == 0 && distToTarget < 1f)
            {
                SoldierTransform.ValueRW.Rotation = SoldierMovementData.ValueRO.TargetRotation;
                return;
            }

            if (SoldierMovementData.ValueRW.OrderStartDelay > 0)
            {
                SoldierMovementData.ValueRW.OrderStartDelay -= deltaTime;
                return;
            }
            SoldierTransform.ValueRW.Position += math.normalize(SoldierMovementData.ValueRO.TargetPosition - SoldierTransform.ValueRO.Position) * deltaTime * SoldierMovementData.ValueRO.MovementSpeed;

            if (distToTarget <= 0.055f)
            {
                SoldierMovementData.ValueRW.IsOrderTaken = 0;

            }

            if (distToTarget < 0.2f)
            {
                SoldierTransform.ValueRW.Rotation = SoldierMovementData.ValueRO.TargetRotation;
            }
            else
            {
                SoldierTransform.ValueRW.Rotation = quaternion.RotateY(RotateTowards(SoldierTransform.ValueRO.Position, SoldierMovementData.ValueRO.TargetPosition));
            }
        }
        else
        {
            SoldierTransform.ValueRW.Position = SoldierMovementData.ValueRO.TargetPosition;
            SoldierTransform.ValueRW.Rotation = SoldierMovementData.ValueRO.TargetRotation;
        }
    }

    public void GetDamage()
    {
        foreach (var element in SoldierDamageBuffer)
        {
            SoldierHealthData.ValueRW.Health -= element.DamageAmount;
        }

        SoldierDamageBuffer.Clear();
    }

    private float RotateTowards(float3 objectsPosition, float3 targetPosition)
    {
        var x = objectsPosition.x - targetPosition.x;
        var y = objectsPosition.z - targetPosition.z;

        return math.atan2(x, y) + math.PI;
    }
}
