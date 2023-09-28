using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;

//[DisableAutoCreation]
[UpdateInGroup(typeof(InitializationSystemGroup),OrderFirst = true)]
public partial class InputSystemBase : SystemBase
{
    private Camera _mainCamera;
    private CollisionWorld _collisionWorld;

    protected override void OnCreate()
    {
        _mainCamera = Camera.main;
        var inputEntity = EntityManager.CreateEntity();
        RequireForUpdate<InputData>();
        base.OnCreate();
    }

    protected override void OnUpdate()
    {
        var inputDataSingleton = SystemAPI.GetSingletonEntity<InputData>();
        var inputDataAspect = SystemAPI.GetAspect<InputAspect>(inputDataSingleton);

        if (Input.GetMouseButtonDown(1))
        {
            inputDataAspect.InputData.ValueRW.GroundInputStartingPos = GetGroundInputPosition();
            inputDataAspect.InputData.ValueRW.IsDragging = 1;
        }

        if (Input.GetMouseButton(1))
        {
            inputDataAspect.InputData.ValueRW.GroundInputPos = GetGroundInputPosition();
        }

        if (Input.GetMouseButtonUp(1))
        {
            inputDataAspect.InputData.ValueRW.GroundInputEndingPos = GetGroundInputPosition();
            inputDataAspect.InputData.ValueRW.IsDragging = 0;
        }
    }

    private float3 GetGroundInputPosition()
    {
        _collisionWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;

        var ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        var rayStart = ray.origin;
        var rayEnd = ray.GetPoint(100f);

        if (Raycast(rayStart, rayEnd, out var hit))
        {
            //var hitEntity = SystemAPI.GetSingleton<PhysicsWorldSingleton>().Bodies[hit.RigidBodyIndex].Entity;
            return hit.Position;
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
