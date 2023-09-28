using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;

//[DisableAutoCreation]
[UpdateInGroup(typeof(InitializationSystemGroup), OrderFirst = true)]
public partial class InputSystemBase : SystemBase
{
    private Camera _mainCamera;
    private CollisionWorld _collisionWorld;

    private byte _isDraggingOnGround;

    protected override void OnCreate()
    {
        _mainCamera = Camera.main;
        var inputEntity = EntityManager.CreateEntity();
        EntityManager.AddComponent(inputEntity, typeof(InputData));
        RequireForUpdate<InputData>();
        RequireForUpdate<UnitCirclePropertiesData>();
        RequireForUpdate<PhysicsWorldSingleton>();
        base.OnCreate();
    }

    protected override void OnUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (SystemAPI.HasComponent<GroundTag>(GetGroundInputPosition().hitTargetEntity))
            {
                var unitCirclePropsEntity = SystemAPI.GetSingletonEntity<UnitCirclePropertiesData>();
                var unityCirclePropsData = EntityManager.GetComponentData<UnitCirclePropertiesData>(unitCirclePropsEntity);

                if(unityCirclePropsData.CurrentSelectedSoldierCount <= 0)
                {
                    return;
                }

                var inputGroundFirstPos = GetGroundInputPosition().Item1;

                foreach (var inputData in SystemAPI.Query<RefRW<InputData>>())
                {
                    inputData.ValueRW.GroundInputStartingPos = inputGroundFirstPos;
                }

                _isDraggingOnGround = 1;
            }

            else if (SystemAPI.HasComponent<SoldierMovementData>(GetGroundInputPosition().hitTargetEntity))
            {

                var soldierBattalionData = EntityManager.GetSharedComponent<SoldierBattalionData>(GetGroundInputPosition().Item2);

                int counter = 0;

                var ecb = new EntityCommandBuffer(Allocator.Temp);

                var unitCirclePropertiesEntity = SystemAPI.GetSingletonEntity<UnitCirclePropertiesData>();


                foreach (var (movementData, entity) in SystemAPI.Query<SoldierMovementData>().WithEntityAccess().WithSharedComponentFilter<SoldierBattalionData>
                    (new SoldierBattalionData { BattalionId = soldierBattalionData.BattalionId, IsBattalionChosen = 0 }))
                {
                    ecb.SetSharedComponentManaged<SoldierBattalionData>(entity, new SoldierBattalionData { BattalionId = soldierBattalionData.BattalionId, IsBattalionChosen = 1 });
                    counter++;
                }

                foreach (var unitCirclePropsData in SystemAPI.Query<RefRW<UnitCirclePropertiesData>>())
                {
                    unitCirclePropsData.ValueRW.CurrentSelectedSoldierCount = counter;
                }

                ecb.Playback(EntityManager);
            }
        }

        if (Input.GetMouseButton(0))
        {
            if (_isDraggingOnGround == 1)
            {
                var inputGroundPos = GetGroundInputPosition().Item1;

                foreach (var inputData in SystemAPI.Query<RefRW<InputData>>())
                {
                    inputData.ValueRW.GroundInputPos = inputGroundPos;
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (_isDraggingOnGround == 1)
            {

                var inputGroundEndingPos = GetGroundInputPosition();

                foreach (var inputData in SystemAPI.Query<RefRW<InputData>>())
                {
                    inputData.ValueRW.GroundInputEndingPos = inputGroundEndingPos.Item1;
                }
            }
            _isDraggingOnGround = 0;
        }
    }

    private (float3 position, Entity hitTargetEntity) GetGroundInputPosition()
    {
        _collisionWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;

        var ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        var rayStart = ray.origin;
        var rayEnd = ray.GetPoint(100f);

        if (Raycast(rayStart, rayEnd, out var hit))
        {
            var hitEntity = SystemAPI.GetSingleton<PhysicsWorldSingleton>().Bodies[hit.RigidBodyIndex].Entity;
            return (hit.Position, hitEntity);
        }

        return (new float3(0, 0, 0), new Entity());
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
                CollidesWith = ~0u,
                GroupIndex = 0
            }
        };

        return _collisionWorld.CastRay(raycastInput, out raycastHit);
    }
}

