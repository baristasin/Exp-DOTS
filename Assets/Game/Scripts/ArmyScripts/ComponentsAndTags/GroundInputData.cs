using Unity.Entities;
using Unity.Mathematics;

public struct GroundInputData : IComponentData
{
    public float3 GroundInputStartingPos;
    public float3 GroundInputPos;
    public float3 GroundInputEndingPos;

    public byte IsRightClickUpOnGround;

    public byte IsDragging;
}
