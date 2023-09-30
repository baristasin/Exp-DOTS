using Unity.Entities;
using Unity.Mathematics;

public struct UnitCirclePlacementBufferElementData : IBufferElementData
{
    public float2 UnitCirclePosXZ;
    public quaternion UnitCircleRotation;
    public float2 UnitCircleCounterAndLineIndexValues;
    public int BattalionId;
}
