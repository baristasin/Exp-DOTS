using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;

//[DisableAutoCreation]
[UpdateInGroup(typeof(InitializationSystemGroup),OrderFirst = true)]
public partial class GroundInputSystemBase : SystemBase
{
    private Camera _mainCamera;
    private CollisionWorld _collisionWorld;

    protected override void OnCreate()
    {
        _mainCamera = Camera.main;
        RequireForUpdate<GroundInputData>();
        base.OnCreate();
    }

    protected override void OnUpdate()
    {
        var inputDataSingleton = SystemAPI.GetSingletonEntity<GroundInputData>();
        var inputDataAspect = SystemAPI.GetAspect<GroundInputAspect>(inputDataSingleton);

        if (Input.GetMouseButtonDown(1))
        {
            inputDataAspect.InputData.ValueRW.GroundInputStartingPos = GetGroundInputPosition();
            inputDataAspect.InputData.ValueRW.IsDragging = 1;
            inputDataAspect.InputData.ValueRW.IsRightClickUpOnGround = 0;
        }

        if (Input.GetMouseButton(1))
        {
            inputDataAspect.InputData.ValueRW.GroundInputPos = GetGroundInputPosition();
            inputDataAspect.InputData.ValueRW.IsRightClickUpOnGround = 0;
        }

        if (Input.GetMouseButtonUp(1))
        {
            Debug.Log("right click up");
            inputDataAspect.InputData.ValueRW.GroundInputEndingPos = GetGroundInputPosition();
            inputDataAspect.InputData.ValueRW.IsDragging = 0;
            inputDataAspect.InputData.ValueRW.IsRightClickUpOnGround = 1;
        }
    }

    private float3 GetGroundInputPosition()
    {
        _collisionWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;

        var ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        var rayStart = ray.origin;
        var rayEnd = ray.GetPoint(1500f);

        if (Raycast(rayStart, rayEnd, out var hit))
        {
            //var hitEntity = SystemAPI.GetSingleton<PhysicsWorldSingleton>().Bodies[hit.RigidBodyIndex].Entity;

            return new float3(hit.Position.x,1.5f,hit.Position.z);
        }

        return new float3(0, 0, 0);
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
