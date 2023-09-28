using Unity.Entities;

public struct SoldierCreatorData : IComponentData
{
    public Entity SoldierEntity;
    public int SoldierCount;
}
