using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

//[DisableAutoCreation]
[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct SoldierMoveOrderSystem : ISystem
{
    public BufferLookup<UnitCirclePlacementBufferElementData> _unitCirclePlacementBufferLookUp;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<UnitCirclePropertiesData>();
        _unitCirclePlacementBufferLookUp = state.GetBufferLookup<UnitCirclePlacementBufferElementData>(true);
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        int counter = 0;

        var unitCirclePropertiesEntity = SystemAPI.GetSingletonEntity<UnitCirclePropertiesData>();

        if (Input.GetMouseButtonUp(0))
        {
            foreach (var (soldierTransform, soldierMovementData,entity) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<SoldierMovementData>>()
                .WithSharedComponentFilter(new SoldierBattalionData { BattalionId = 1,IsBattalionChosen = 1 }).WithEntityAccess())
            {
                _unitCirclePlacementBufferLookUp.Update(ref state);

                _unitCirclePlacementBufferLookUp.TryGetBuffer(unitCirclePropertiesEntity, out var unitCirclePlacementBufferForMove);

                if (unitCirclePlacementBufferForMove.Length <= 0) return;

                soldierTransform.ValueRW.Position = new float3(unitCirclePlacementBufferForMove[counter].UnitCirclePosXZ.x,
                    soldierTransform.ValueRO.Position.y,
                    unitCirclePlacementBufferForMove[counter].UnitCirclePosXZ.y);

                soldierTransform.ValueRW.Rotation = unitCirclePlacementBufferForMove[counter].UnitCircleRotation;

                counter++;
            }

            var ecb = new EntityCommandBuffer(Allocator.Temp);
            ecb.RemoveComponent<UnitCirclePlacementBufferElementData>(unitCirclePropertiesEntity);
        }
    }
}
