using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;

//[DisableAutoCreation]
[UpdateInGroup(typeof(InitializationSystemGroup), OrderFirst = true)]
public partial class BattalionClickInputSystemBase : SystemBase
{
    private Camera _mainCamera;
    private CollisionWorld _collisionWorld;

    protected override void OnCreate()
    {
        _mainCamera = Camera.main;
        RequireForUpdate<UnitCirclePropertiesData>();
        base.OnCreate();
    }

    protected override void OnUpdate()
    {
        var unitCirclePropertiesEntity = SystemAPI.GetSingletonEntity<UnitCirclePropertiesData>();
        var unitCirclePropertiesAspect = SystemAPI.GetAspect<UnitCirclePropertiesAspect>(unitCirclePropertiesEntity);

        if (Input.GetMouseButtonDown(0))
        {
            // Send clear all choose
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            if (!Input.GetKey(KeyCode.T))
            {
                foreach (var (visualData, entity) in SystemAPI.Query<SoldierVisualData>().WithEntityAccess())
                {
                    ecb.SetSharedComponentManaged<SoldierBattalionIsChosenData>(entity, new SoldierBattalionIsChosenData { IsBattalionChosen = 0 });
                    ecb.SetEnabled(visualData.SelectedVisualObject, false);
                }

                unitCirclePropertiesAspect.UnitCirclePropData.ValueRW.CurrentSelectedSoldierCount = 0;

                Debug.Log("All selections canceled");

                ecb.AddBuffer<UnitCircleSelectedBattalionAndCountBufferElementData>(unitCirclePropertiesEntity);
            }

            var soldierEntity = GetSoldierEntity();

            if (SystemAPI.HasComponent<SoldierMovementData>(soldierEntity))
            {
                Debug.Log("Soldier clicked");

                int counter = 0;

                // Send soldier for battalion for choose process

                var soldierBattalionData = EntityManager.GetSharedComponent<SoldierBattalionIdData>(soldierEntity);

                foreach (var (movementData,visualData, entity) in SystemAPI.Query<SoldierMovementData,SoldierVisualData>().WithEntityAccess().WithSharedComponentFilter<SoldierBattalionIdData>
                    (new SoldierBattalionIdData { BattalionId = soldierBattalionData.BattalionId }))
                {
                    ecb.SetSharedComponentManaged<SoldierBattalionIsChosenData>(entity, new SoldierBattalionIsChosenData { IsBattalionChosen = 1 });
                    ecb.SetEnabled(visualData.SelectedVisualObject, true);
                    counter++;
                }

                Debug.Log("Battalion picked");

                unitCirclePropertiesAspect.UnitCirclePropData.ValueRW.CurrentSelectedSoldierCount += counter;
                ecb.AppendToBuffer<UnitCircleSelectedBattalionAndCountBufferElementData>(unitCirclePropertiesEntity
                    , new UnitCircleSelectedBattalionAndCountBufferElementData { BattalionId = soldierBattalionData.BattalionId, SoldierCount = counter });
            }

            ecb.Playback(EntityManager);

            //else
            //{
            //    // Send clear all choose

            //    var ecb = new EntityCommandBuffer(Allocator.Temp);

            //    foreach (var (movementData, entity) in SystemAPI.Query<SoldierMovementData>().WithEntityAccess())
            //    {
            //        ecb.SetSharedComponentManaged<SoldierBattalionIsChosenData>(entity, new SoldierBattalionIsChosenData { IsBattalionChosen = 0 });
            //    }

            //    unitCirclePropertiesAspect.UnitCirclePropData.ValueRW.CurrentSelectedSoldierCount = 0;

            //    Debug.Log("All selections canceled");

            //    ecb.Playback(EntityManager);
            //}
        }
    }

    private Entity GetSoldierEntity()
    {
        _collisionWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;

        var ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        var rayStart = ray.origin;
        var rayEnd = ray.GetPoint(1500f);

        if (Raycast(rayStart, rayEnd, out var hit))
        {
            return hit.Entity;
        }

        return new Entity();
    }

    private bool Raycast(float3 rayStart, float3 rayEnd, out Unity.Physics.RaycastHit raycastHit)
    {
        var raycastInput = new RaycastInput
        {
            Start = rayStart,
            End = rayEnd,
            Filter = new CollisionFilter
            {
                BelongsTo = ~0u,
                CollidesWith = (uint) 1 << 0,
                GroupIndex = 0
            }
        };

        return _collisionWorld.CastRay(raycastInput, out raycastHit);
    }
}
