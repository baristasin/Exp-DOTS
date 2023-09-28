using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;
using RaycastHit = Unity.Physics.RaycastHit;

//[DisableAutoCreation]
[UpdateInGroup(typeof(AfterPhysicsSystemGroup))]
public partial class InputSystemBase : SystemBase
{
    private Camera _mainCamera;

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
            if (SystemAPI.HasComponent<SoldierMovementData>(GetRaycastEntity()))
            {
                var soldierBattalionData = EntityManager.GetSharedComponent<SoldierBattalionIdData>(GetRaycastEntity());

                int counter = 0;

                var ecb = new EntityCommandBuffer(Allocator.Temp);

                foreach (var (movementData, entity) in SystemAPI.Query<SoldierMovementData>().WithEntityAccess().WithSharedComponentFilter<SoldierBattalionIdData>
                    (new SoldierBattalionIdData { BattalionId = soldierBattalionData.BattalionId }))
                {
                    ecb.SetSharedComponentManaged<SoldierBattalionIsChosenData>(entity, new SoldierBattalionIsChosenData { IsBattalionChosen = 1 });
                    counter++;
                    Debug.Log($"Unit chosen {counter}");
                }

                Debug.Log("Unit ChooseCompleted");

                foreach (var unitCirclePropsData in SystemAPI.Query<RefRW<UnitCirclePropertiesData>>())
                {
                    unitCirclePropsData.ValueRW.CurrentSelectedSoldierCount = counter;
                    Debug.Log($"CurrentSelected Count: {counter}");
                }

                ecb.Playback(EntityManager);
            }

            else if (SystemAPI.HasComponent<GroundTag>(GetRaycastEntity()))
            {
                var ecb = new EntityCommandBuffer(Allocator.Temp);

                foreach (var (movementData, entity) in SystemAPI.Query<SoldierMovementData>().WithEntityAccess().WithSharedComponentFilter<SoldierBattalionIsChosenData>
                (new SoldierBattalionIsChosenData { IsBattalionChosen = 1 }))
                {
                    Debug.Log("Ground left click had selection, clear all selection");
                    ecb.SetSharedComponentManaged<SoldierBattalionIsChosenData>(entity, new SoldierBattalionIsChosenData { IsBattalionChosen = 0 });
                }

                foreach (var unitCirclePropsData in SystemAPI.Query<RefRW<UnitCirclePropertiesData>>())
                {
                    Debug.Log("Ground left click, current selection count = 0");
                    unitCirclePropsData.ValueRW.CurrentSelectedSoldierCount = 0;
                }

                ecb.Playback(EntityManager);
            }
        }

        if (Input.GetKeyDown("a"))
        {
            Debug.Log("Right click");
            if (SystemAPI.HasComponent<GroundTag>(GetRaycastEntity()))
            {
                var unitCirclePropsEntity = SystemAPI.GetSingletonEntity<UnitCirclePropertiesData>();
                var unitCirclePropsData = EntityManager.GetComponentData<UnitCirclePropertiesData>(unitCirclePropsEntity);


                if (unitCirclePropsData.CurrentSelectedSoldierCount <= 0)
                {
                    Debug.Log("No Selected Soldier, return");

                    return;
                }

                var inputGroundFirstPos = GetRaycastPosition();

                foreach (var inputData in SystemAPI.Query<RefRW<InputData>>())
                {
                    inputData.ValueRW.GroundInputStartingPos = inputGroundFirstPos;
                }

                _isDraggingOnGround = 1;
            }
        }

        if (Input.GetKeyDown("a"))
        {
            if (_isDraggingOnGround == 1)
            {
                var inputGroundPos = GetRaycastPosition();

                foreach (var inputData in SystemAPI.Query<RefRW<InputData>>())
                {
                    inputData.ValueRW.GroundInputPos = inputGroundPos;
                }
            }
        }

        if (Input.GetMouseButtonUp(1))
        {
            if (_isDraggingOnGround == 1)
            {

                var inputGroundEndingPos = GetRaycastPosition();

                foreach (var inputData in SystemAPI.Query<RefRW<InputData>>())
                {
                    inputData.ValueRW.GroundInputEndingPos = inputGroundEndingPos;
                }
            }
            _isDraggingOnGround = 0;
        }
    }

    public Entity GetRaycastEntity()
    {
        // Set up Entity Query to get PhysicsWorldSingleton
        // If doing this in SystemBase or ISystem, call GetSingleton<PhysicsWorldSingleton>()/SystemAPI.GetSingleton<PhysicsWorldSingleton>() directly.

        var collisionWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;


        var ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        var rayStart = ray.origin;
        var rayEnd = ray.GetPoint(100f);

        RaycastInput input = new RaycastInput()
        {
            Start = rayStart,
            End = rayEnd,
            Filter = new CollisionFilter()
            {
                BelongsTo = ~0u,
                CollidesWith = ~0u, // all 1s, so all layers, collide with everything
                GroupIndex = 0
            }
        };

        RaycastHit hit = new RaycastHit();
        bool haveHit = collisionWorld.CastRay(input, out hit);
        if (haveHit)
        {
            return hit.Entity;
        }
        return Entity.Null;
    }

    public float3 GetRaycastPosition()
    {
        // Set up Entity Query to get PhysicsWorldSingleton
        // If doing this in SystemBase or ISystem, call GetSingleton<PhysicsWorldSingleton>()/SystemAPI.GetSingleton<PhysicsWorldSingleton>() directly.

        var collisionWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;


        var ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        var rayStart = ray.origin;
        var rayEnd = ray.GetPoint(100f);

        RaycastInput input = new RaycastInput()
        {
            Start = rayStart,
            End = rayEnd,
            Filter = new CollisionFilter()
            {
                BelongsTo = ~0u,
                CollidesWith = ~0u, // all 1s, so all layers, collide with everything
                GroupIndex = 0
            }
        };

        RaycastHit hit = new RaycastHit();
        bool haveHit = collisionWorld.CastRay(input, out hit);
        if (haveHit)
        {
            return hit.Position;
        }
        return float3.zero;
    }

    //private (float3 position, Entity hitTargetEntity) GetGroundInputPosition()
    //{
    //    //if (SystemAPI.TryGetSingleton<PhysicsWorldSingleton>(out var world))
    //    //{

    //    //    _collisionWorld = world.CollisionWorld;

    //    //    var ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
    //    //    var rayStart = ray.origin;
    //    //    var rayEnd = ray.GetPoint(100f);

    //    //    if (Raycast(rayStart, rayEnd, out var hit))
    //    //    {
    //    //        //var hitEntity = world.Bodies[hit.RigidBodyIndex].Entity;
    //    //        return (hit.Position, hit.Entity);
    //    //    }

    //    //    return (new float3(0, 0, 0), new Entity());
    //    //}
    //    //else
    //    //{
    //    //    return (new float3(0, 0, 0), new Entity());

    //    //}
    //    _collisionWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;

    //    var ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
    //    var rayStart = ray.origin;
    //    var rayEnd = ray.GetPoint(100f);

    //    if (Raycast(rayStart, rayEnd, out var hit))
    //    {
    //        var hitEntity = SystemAPI.GetSingleton<PhysicsWorldSingleton>().Bodies[hit.RigidBodyIndex].Entity;
    //        return (hit.Position, hitEntity);
    //    }

    //    return (new float3(0, 0, 0), new Entity());
    //}

    //private bool Raycast(float3 rayStart, float3 rayEnd, out Unity.Physics.RaycastHit raycastHit)
    //{
    //    var raycastInput = new RaycastInput
    //    {
    //        Start = rayStart,
    //        End = rayEnd,
    //        Filter = new CollisionFilter
    //        {
    //            BelongsTo = ~0u,
    //            CollidesWith = ~0u,
    //            GroupIndex = 0
    //        }
    //    };

    //    return _collisionWorld.CastRay(raycastInput, out raycastHit);
    //}
}

