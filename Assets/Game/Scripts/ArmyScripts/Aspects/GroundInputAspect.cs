using Unity.Entities;

public readonly partial struct GroundInputAspect : IAspect
{
    public readonly Entity Entity;

    public readonly RefRW<GroundInputData> InputData;
}
