using Unity.Entities;

public readonly partial struct InputAspect : IAspect
{
    public readonly Entity Entity;

    public readonly RefRW<InputData> InputData;
}
