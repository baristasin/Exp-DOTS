using Unity.Entities;

public readonly partial struct SoldierCreatePropertiesAspect : IAspect
{
    public readonly Entity Entity;

    public readonly RefRO<SoldierFactoryData> SoldierFactoryData;
}
