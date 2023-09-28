using Unity.Entities;
using Unity.Mathematics;

public struct SoldierCreationBufferElementData : IBufferElementData
{
    public SoldierType SoldierType;
    public int SoldierCount;
}

public enum SoldierType
{
    Swordsmen,
    Archer,
    Knight
}
