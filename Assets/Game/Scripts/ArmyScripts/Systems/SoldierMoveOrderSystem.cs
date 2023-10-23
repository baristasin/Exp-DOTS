using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

//[DisableAutoCreation]
[UpdateInGroup(typeof(SimulationSystemGroup))]
[BurstCompile]
public partial struct SoldierMoveOrderSystem : ISystem
{
    public byte IsInstantMove;

    public BufferLookup<UnitCirclePlacementBufferElementData> _unitCirclePlacementBufferLookUp;
    public BufferLookup<UnitCircleSelectedBattalionAndCountBufferElementData> _selectedBattalionAndCountBufferLookup;


    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<UnitCirclePropertiesData>();
        _unitCirclePlacementBufferLookUp = state.GetBufferLookup<UnitCirclePlacementBufferElementData>(false);
        _selectedBattalionAndCountBufferLookup = state.GetBufferLookup<UnitCircleSelectedBattalionAndCountBufferElementData>(true);

        IsInstantMove = 0;
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }

    public (UnitCirclePlacementBufferElementData circle, int itemIndex) GetDesiredCircleAndItsIndex(RefRW<SoldierMovementData> soldierMovementData, DynamicBuffer<UnitCirclePlacementBufferElementData> unitCirclePlacementBufferElementDatas, int battalionId)
    {
        for (int i = 0; i < unitCirclePlacementBufferElementDatas.Length; i++)
        {
            if (unitCirclePlacementBufferElementDatas[i].BattalionId == battalionId)
            {
                if (math.Equals(soldierMovementData.ValueRO.SoldierCounterAndLineValues, unitCirclePlacementBufferElementDatas[i].UnitCircleCounterAndLineIndexValues))
                {
                    return (unitCirclePlacementBufferElementDatas[i], i);
                }
            }
        }

        for (int i = 0; i < unitCirclePlacementBufferElementDatas.Length; i++)
        {
            if (unitCirclePlacementBufferElementDatas[i].BattalionId == battalionId)
            {
                return (unitCirclePlacementBufferElementDatas[i], i);
            }
        }

        return (unitCirclePlacementBufferElementDatas[unitCirclePlacementBufferElementDatas.Length - 1], unitCirclePlacementBufferElementDatas.Length - 1);

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var deltaTime = SystemAPI.Time.DeltaTime;
        int counter = 0;
        var unitCirclePropertiesEntity = SystemAPI.GetSingletonEntity<UnitCirclePropertiesData>();
        var unitCirclePropertiesAspect = SystemAPI.GetAspect<UnitCirclePropertiesAspect>(unitCirclePropertiesEntity);

        var groundInputDataEntity = SystemAPI.GetSingletonEntity<GroundInputData>();
        var groundInputDataAspect = SystemAPI.GetAspect<GroundInputAspect>(groundInputDataEntity);

        _unitCirclePlacementBufferLookUp.Update(ref state);

        if (unitCirclePropertiesAspect.UnitCirclePropData.ValueRO.IsBufferLoadedWithPositions == 1)
        {
            _selectedBattalionAndCountBufferLookup.Update(ref state);

            if (_selectedBattalionAndCountBufferLookup.TryGetBuffer(unitCirclePropertiesEntity, out var selectedBattalionAndCountBuffer))
            {
                for (int i = 0; i < selectedBattalionAndCountBuffer.Length; i++)
                {
                    foreach (var (soldierTransform, soldierMovementData, entity) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<SoldierMovementData>>()
    .WithSharedComponentFilter(new SoldierBattalionIsChosenData { IsBattalionChosen = 1 })
    .WithEntityAccess())
                    {
                        _unitCirclePlacementBufferLookUp.TryGetBuffer(unitCirclePropertiesEntity, out var unitCirclePlacementBufferForMove);

                        if (unitCirclePlacementBufferForMove.Length <= 0) return;

                        var battalionId = state.EntityManager.GetSharedComponent<SoldierBattalionIdData>(entity);

                        var desiredCircleAndItsIndex = GetDesiredCircleAndItsIndex(soldierMovementData, unitCirclePlacementBufferForMove, battalionId.BattalionId);

                        soldierMovementData.ValueRW.TargetPosition = new float3(desiredCircleAndItsIndex.circle.UnitCirclePosXZ.x,
                            1.5f,
                            desiredCircleAndItsIndex.circle.UnitCirclePosXZ.y);

                        soldierMovementData.ValueRW.TargetRotation = desiredCircleAndItsIndex.circle.UnitCircleRotation;

                        soldierMovementData.ValueRW.SoldierCounterAndLineValues = new float2(desiredCircleAndItsIndex.circle.UnitCircleCounterAndLineIndexValues.x, desiredCircleAndItsIndex.circle.UnitCircleCounterAndLineIndexValues.y);

                        soldierMovementData.ValueRW.IsOrderTaken = 1;

                        soldierMovementData.ValueRW.OrderStartDelay = (counter) * 0.0025f;

                        unitCirclePlacementBufferForMove.RemoveAt(desiredCircleAndItsIndex.itemIndex);

                        counter++;

                    }

                unitCirclePropertiesAspect.UnitCirclePropData.ValueRW.IsBufferLoadedWithPositions = 0;
                groundInputDataAspect.InputData.ValueRW.IsRightClickUpOnGround = 0;
                }
            }


            //var ecb = new EntityCommandBuffer(Allocator.Temp);

            //ecb.RemoveComponent<UnitCirclePlacementBufferElementData>(unitCirclePropertiesEntity);

        }

        else if (groundInputDataAspect.InputData.ValueRO.IsRightClickUpOnGround == 1) // Right click movement
        {
            groundInputDataAspect.InputData.ValueRW.IsRightClickUpOnGround = 0;

            float3 movementDistanceVector = new float3(0, 0, 0);

            foreach (var (soldierTransform, soldierMovementData, entity) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<SoldierMovementData>>()
                .WithSharedComponentFilter(new SoldierBattalionIsChosenData { IsBattalionChosen = 1 }).WithEntityAccess())
            {
                if (counter == 0) // Calculate movement distance vector from first soldier
                {
                    movementDistanceVector = soldierMovementData.ValueRO.TargetPosition - groundInputDataAspect.InputData.ValueRO.GroundInputEndingPos;
                    soldierMovementData.ValueRW.TargetPosition = groundInputDataAspect.InputData.ValueRO.GroundInputEndingPos;
                    Debug.Log(groundInputDataAspect.InputData.ValueRO.GroundInputEndingPos);
                }
                else
                {
                    soldierMovementData.ValueRW.TargetPosition = soldierMovementData.ValueRO.TargetPosition - movementDistanceVector;
                }

                soldierMovementData.ValueRW.IsOrderTaken = 1;

                counter++;
            }
        }

        new SoldierMoveJob
        {
            IsInstantMove = IsInstantMove,
            DeltaTime = deltaTime
        }.ScheduleParallel();

    }
}

[BurstCompile]
public partial struct SoldierMoveJob : IJobEntity
{
    public byte IsInstantMove;
    public float DeltaTime;

    [BurstCompile]
    public void Execute(SoldierAspect soldierAspect)
    {
        //soldierAspect.SoldierMovementData.ValueRW.TargetPosition = new float3(soldierAspect.SoldierMovementData.ValueRW.TargetPosition.x, soldierAspect.SoldierTransform.ValueRO.Position.y, soldierAspect.SoldierMovementData.ValueRW.TargetPosition.z);
        float3 targetPositionAligned = new float3(soldierAspect.SoldierMovementData.ValueRO.TargetPosition.x, soldierAspect.SoldierTransform.ValueRO.Position.y, soldierAspect.SoldierMovementData.ValueRO.TargetPosition.z);

        if (Vector3.Distance(soldierAspect.SoldierTransform.ValueRO.Position, targetPositionAligned) is var distToTarget && distToTarget >= 0.05f)
        {
            soldierAspect.Move(IsInstantMove, DeltaTime, distToTarget, distToTarget >= 0.5f ? 1 : 0);
        }
    }
}
