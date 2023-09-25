using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(InitializationSystemGroup))]
[UpdateAfter(typeof(SoldierCreationSystem))]
public partial class SoldierPlacementSystem : SystemBase
{
    public Vector3 GroundPositionValue;
    public Vector3 CurrentGroundPositionValue;

    [BurstCompile]
    protected override void OnCreate()
    {
        RequireForUpdate<SoldierCreatorData>();
        CurrentGroundPositionValue = new Vector3(0, 0, 0);
    }

    protected override void OnUpdate()
    {
        GroundPositionValue = Object.FindObjectOfType<GroundRaycaster>().OrderPositionValue;

        if(CurrentGroundPositionValue != GroundPositionValue)
        {
            int battalionSoldierCount = 0;

            foreach (var (localTransform, soldierMovementData, entity) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<SoldierMovementData>>()
    .WithSharedComponentFilter(new SoldierBattalionData { BattalionId = 1 }).WithEntityAccess())
            {
                battalionSoldierCount++;
            }

            var middleSoldierIndex = battalionSoldierCount / 2;
            Vector3 middleSoldierImaginaryPoint = new Vector3(0,0,0);


            middleSoldierImaginaryPoint.x = GroundPositionValue.x;
            middleSoldierImaginaryPoint.z = GroundPositionValue.z;

            int counter2 = 0;
            while (middleSoldierIndex > 0)
            {

                if (counter2 >= 5 && counter2 % 5 == 0)
                {
                    middleSoldierImaginaryPoint.x = GroundPositionValue.x;
                    middleSoldierImaginaryPoint.z -= 2;
                }

                middleSoldierImaginaryPoint.x += 2;
                counter2++;

                middleSoldierIndex--;
            }

            var offset = math.abs(GroundPositionValue - middleSoldierImaginaryPoint);

            Debug.Log($"GroundPos = {GroundPositionValue}, mid = {middleSoldierImaginaryPoint}, offset = {offset}");


            float soldierIndexX = GroundPositionValue.x;
            float soldierIndexZ = GroundPositionValue.z;

            int counter = 0;

            foreach (var (localTransform, soldierMovementData, entity) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<SoldierMovementData>>()
                .WithSharedComponentFilter(new SoldierBattalionData { BattalionId = 1 }).WithEntityAccess())
            {
                if (counter >= 5 && counter % 5 == 0)
                {
                    soldierIndexX = GroundPositionValue.x;
                    soldierIndexZ -= 2;
                }

                localTransform.ValueRW.Position = new float3(soldierIndexX - offset.x, 1.5f, soldierIndexZ + offset.z); 

                soldierIndexX += 2;
                counter++;
            }

            CurrentGroundPositionValue = GroundPositionValue;
        }
    }
}
