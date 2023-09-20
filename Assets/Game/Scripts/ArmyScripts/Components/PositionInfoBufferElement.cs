using System;
using Unity.Entities;
using Unity.Mathematics;

public struct PositionInfoBufferElement : IBufferElementData
{
    public float3 Position;

    public static implicit operator PositionInfoBufferElement(float3 value)
    {
        return new PositionInfoBufferElement { Position = value };
    }
}

