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
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        var inputEntity = ecb.CreateEntity();
        ecb.AddComponent(inputEntity, new InputData());
        RequireForUpdate<InputData>();

        ecb.Playback(EntityManager);
        base.OnCreate();
    }

    protected override void OnUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var inputGroundFirstPos = GetGroundInputPosition();

            foreach (var inputData in SystemAPI.Query<RefRW<InputData>>())
            {
                inputData.ValueRW.GroundInputStartingPos = inputGroundFirstPos;
            }
        }

        if (Input.GetMouseButton(0))
        {
            var inputGroundPos = GetGroundInputPosition();

            foreach (var inputData in SystemAPI.Query<RefRW<InputData>>())
            {
                inputData.ValueRW.GroundInputPos = inputGroundPos;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            var inputGroundEndingPos = GetGroundInputPosition();

            foreach (var inputData in SystemAPI.Query<RefRW<InputData>>())
            {
                inputData.ValueRW.GroundInputEndingPos = inputGroundEndingPos;
            }

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
