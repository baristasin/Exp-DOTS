using Unity.Entities;
using Unity.Mathematics;

public struct UnitCirclePlacementBufferElementData : IBufferElementData
{
    public float2 UnitCirclePosXZ;
    public quaternion UnitCircleRotation;
}
