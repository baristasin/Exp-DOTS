using Unity.Entities;

public readonly partial struct UnitCirclePropertiesAspect : IAspect
{
    public readonly Entity Entity;

    public readonly Entity UnitCircleEntity => UnitCirclePropData.ValueRO.UnitCircleEntity;

    public readonly RefRW<UnitCirclePropertiesData> UnitCirclePropData;
}
