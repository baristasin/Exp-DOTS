using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

//[DisableAutoCreation]
[UpdateInGroup(typeof(InitializationSystemGroup))]
[UpdateAfter(typeof(UnitCirclePlacementSystem))]
public partial struct UnitCircleAlignmentSystem : ISystem
{
    public BufferLookup<UnitCircleSelectedBattalionAndCountBufferElementData> _selectedBattalionAndCountBufferLookup;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<UnitCirclePropertiesData>();
        state.RequireForUpdate<GroundInputData>();

        _selectedBattalionAndCountBufferLookup = state.GetBufferLookup<UnitCircleSelectedBattalionAndCountBufferElementData>(true);

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var unitCirclePropertiesEntity = SystemAPI.GetSingletonEntity<UnitCirclePropertiesData>();
        var unitCirclePropertiesAspect = SystemAPI.GetAspect<UnitCirclePropertiesAspect>(unitCirclePropertiesEntity);        

        if(unitCirclePropertiesAspect.UnitCirclePropData.ValueRO.CurrentSelectedSoldierCount > 0)
        {
            _selectedBattalionAndCountBufferLookup.Update(ref state);

            _selectedBattalionAndCountBufferLookup.TryGetBuffer(unitCirclePropertiesEntity, out var buffer);

            for (int i = 0; i < buffer.Length; i++)
            {
                int counter = 0;
                int lineIndex = 0;
                int lineMaximumSoldierCount = 10;

                foreach (var (unitCircleTransform, unitCircleData) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<UnitCircleData>>().WithSharedComponentFilter(new UnitCircleBattalionIdData { BattalionId = buffer[i].BattalionId }))
                {
                    var inputDataEntity = SystemAPI.GetSingletonEntity<GroundInputData>();
                    var inputDataAspect = SystemAPI.GetAspect<GroundInputAspect>(inputDataEntity);

                    FormationHelper.FormationFuncLocalTransform(inputDataAspect, ref state, ref counter, ref lineIndex, ref lineMaximumSoldierCount, unitCircleTransform, unitCircleData, i);
                }
            }
        }

    }    
}

