using Unity.Entities;
using Unity.Mathematics;

public struct SoldierMovementData : IComponentData
{
    public float MovementSpeed;
    public float3 TargetPosition;
    public quaternion TargetRotation;

    public float2 SoldierCounterAndLineValues;

    public float OrderStartDelay;

    public byte IsOrderTaken;
}
