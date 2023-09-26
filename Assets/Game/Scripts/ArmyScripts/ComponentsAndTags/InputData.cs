using Unity.Entities;
using Unity.Mathematics;

public struct InputData : IComponentData
{
    public float3 GroundInputStartingPos;
    public float3 GroundInputPos;
    public float3 GroundInputEndingPos;
}
