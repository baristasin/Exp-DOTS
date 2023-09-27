using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

//[DisableAutoCreation]
[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial struct SoldierCreationSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<SoldierCreatorData>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        byte battalionIndex = 0;

        foreach (var soldierCreatorData in SystemAPI.Query<SoldierCreatorData>())
        {           
            for (int i = 0; i < 250; i++)
            {
                var soldierEntity = ecb.Instantiate(soldierCreatorData.SoldierEntity);

                LocalTransform soldierTransform = new LocalTransform
                {
                    Position = new float3(0, 1.5f, 0),
                    Rotation = quaternion.identity,
                    Scale = 1f
                };

                ecb.AddComponent(soldierEntity, new SoldierMovementData
                {
                    MovementSpeed = 6f,
                    TargetPosition = soldierTransform.Position,
                    TargetRotation = soldierTransform.Rotation
                });

                SoldierBattalionData soldierBattalionData = new SoldierBattalionData { BattalionId = 1,IsBattalionChosen = 1 };

                ecb.AddComponent(soldierEntity, soldierTransform);
                ecb.AddSharedComponent(soldierEntity, soldierBattalionData);
            }

        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();

        state.Enabled = false;
    }
}
