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
    public byte IsInstantMove;

    public BufferLookup<UnitCirclePlacementBufferElementData> _unitCirclePlacementBufferLookUp;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<UnitCirclePropertiesData>();
        _unitCirclePlacementBufferLookUp = state.GetBufferLookup<UnitCirclePlacementBufferElementData>(true);
        IsInstantMove = 0;
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var deltaTime = SystemAPI.Time.DeltaTime;
        int counter = 0;

        var unitCirclePropertiesEntity = SystemAPI.GetSingletonEntity<UnitCirclePropertiesData>();

        if (Input.GetMouseButtonUp(0))
        {
            foreach (var (soldierTransform, soldierMovementData, entity) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<SoldierMovementData>>()
                .WithSharedComponentFilter(new SoldierBattalionData { BattalionId = 1, IsBattalionChosen = 1 }).WithEntityAccess())
            {
                _unitCirclePlacementBufferLookUp.Update(ref state);

                _unitCirclePlacementBufferLookUp.TryGetBuffer(unitCirclePropertiesEntity, out var unitCirclePlacementBufferForMove);

                if (unitCirclePlacementBufferForMove.Length <= 0) return;
                if (IsInstantMove == 1)
                {
                    soldierTransform.ValueRW.Position = new float3(unitCirclePlacementBufferForMove[counter].UnitCirclePosXZ.x,
                        soldierTransform.ValueRO.Position.y,
                        unitCirclePlacementBufferForMove[counter].UnitCirclePosXZ.y);

                    soldierTransform.ValueRW.Rotation = unitCirclePlacementBufferForMove[counter].UnitCircleRotation;
                }

                else
                {
                    soldierMovementData.ValueRW.TargetPosition = new float3(unitCirclePlacementBufferForMove[counter].UnitCirclePosXZ.x,
                        soldierTransform.ValueRO.Position.y,
                        unitCirclePlacementBufferForMove[counter].UnitCirclePosXZ.y);

                    soldierMovementData.ValueRW.TargetRotation = unitCirclePlacementBufferForMove[counter].UnitCircleRotation;
                }

                counter++;
            }

            var ecb = new EntityCommandBuffer(Allocator.Temp);
            ecb.RemoveComponent<UnitCirclePlacementBufferElementData>(unitCirclePropertiesEntity);
        }

        foreach (var (soldierTransform, soldierMovementData) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<SoldierMovementData>>()) // Jobify
        {
            if(Vector3.Distance(soldierTransform.ValueRO.Position,soldierMovementData.ValueRO.TargetPosition) is var distToTarget && distToTarget > 0.05f)
            {                
                soldierTransform.ValueRW.Position += math.normalize(soldierMovementData.ValueRO.TargetPosition - soldierTransform.ValueRO.Position) * deltaTime * soldierMovementData.ValueRO.MovementSpeed;

                if(distToTarget < 0.1f)
                {
                    soldierTransform.ValueRW.Rotation = soldierMovementData.ValueRO.TargetRotation;
                }
                else
                {
                    soldierTransform.ValueRW.Rotation = quaternion.RotateY(RotateTowards(soldierTransform.ValueRO.Position, soldierMovementData.ValueRO.TargetPosition));
                }
            }
        }

    }

    private float RotateTowards(float3 objectsPosition, float3 targetPosition)
    {
        var x = objectsPosition.x - targetPosition.x;
        var y = objectsPosition.z - targetPosition.z;

        return math.atan2(x, y) + math.PI;
    }
}
