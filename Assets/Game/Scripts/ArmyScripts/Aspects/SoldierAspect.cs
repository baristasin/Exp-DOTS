using System;
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

    public readonly RefRW<SoldierAnimationData> SoldierAnimationData;

    public readonly DynamicBuffer<SoldierDamageBufferElement> SoldierDamageBuffer;

    public void Move(byte IsInstantMove, float deltaTime, float distToTarget, int isOrderedToMove)
    {
        if (IsInstantMove == 0)
        {
            if (SoldierMovementData.ValueRW.IsOrderTaken == 0 && distToTarget < 1f)
            {
                SoldierTransform.ValueRW.Rotation = SoldierMovementData.ValueRO.TargetRotation;
                if (SoldierAnimationData.ValueRW.AnimationType != AnimationType.Attacking)
                {
                    SoldierAnimationData.ValueRW.AnimationType = AnimationType.Idle;
                }

                return;
            }

            if (SoldierMovementData.ValueRW.OrderStartDelay > 0)
            {
                SoldierMovementData.ValueRW.OrderStartDelay -= deltaTime;
                SoldierAnimationData.ValueRW.AnimationType = AnimationType.Idle;
                return;
            }

            float3 targetPositionAligned = new float3(SoldierMovementData.ValueRO.TargetPosition.x, SoldierTransform.ValueRO.Position.y, SoldierMovementData.ValueRO.TargetPosition.z);

            SoldierTransform.ValueRW.Position += math.normalize(targetPositionAligned - SoldierTransform.ValueRO.Position) * deltaTime * SoldierMovementData.ValueRO.MovementSpeed;
            SoldierAnimationData.ValueRW.AnimationType = AnimationType.Running;

            if (distToTarget <= 0.055f)
            {
                //SoldierMovementData.ValueRW.IsOrderTaken = 0;

            }

            if (distToTarget < 0.2f)
            {
                SoldierTransform.ValueRW.Rotation = SoldierMovementData.ValueRO.TargetRotation;
                if (SoldierAnimationData.ValueRW.AnimationType != AnimationType.Attacking)
                {
                    SoldierAnimationData.ValueRW.AnimationType = AnimationType.Idle;
                }
            }
            else
            {
                //SoldierTransform.ValueRW.Rotation = quaternion.RotateY(RotateTowards(SoldierTransform.ValueRO.Position, targetPositionAligned));
                SoldierTransform.ValueRW.Rotation = math.slerp(SoldierTransform.ValueRW.Rotation, quaternion.RotateY(RotateTowards(SoldierTransform.ValueRO.Position, targetPositionAligned)), deltaTime * 5f);
            }
        }
        else
        {
            float3 targetPositionAligned = new float3(SoldierMovementData.ValueRO.TargetPosition.x, SoldierTransform.ValueRO.Position.y, SoldierMovementData.ValueRO.TargetPosition.z);
            SoldierTransform.ValueRW.Position = targetPositionAligned;
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

    public void StopSoldier()
    {
        SoldierAnimationData.ValueRW.AnimationType = AnimationType.Idle;
        SoldierMovementData.ValueRW.TargetPosition = SoldierTransform.ValueRO.Position;
        SoldierTransform.ValueRW.Rotation = SoldierTransform.ValueRO.Rotation;
    }
}
