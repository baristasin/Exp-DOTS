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

        foreach (var soldierCreatorData in SystemAPI.Query<SoldierCreatorData>())
        {
            int counter = 10;
            int lineIndex = soldierCreatorData.SoldierCount / 10;

            for (int i = 0; i < soldierCreatorData.SoldierCount; i++)
            {
                if (counter <= 0)
                {
                    counter = 10;
                    lineIndex--;
                }

                var soldierEntity = ecb.Instantiate(soldierCreatorData.SoldierEntity);

                LocalTransform soldierTransform = new LocalTransform
                {
                    Position = new float3(counter * 1.5f, 1.5f, -lineIndex * 1.5f),
                    Rotation = quaternion.identity,
                    Scale = 1f
                };

                ecb.AddComponent(soldierEntity, new SoldierMovementData
                {
                    MovementSpeed = 15f,
                    TargetPosition = soldierTransform.Position,
                    TargetRotation = soldierTransform.Rotation
                });

                SoldierBattalionData soldierBattalionData = new SoldierBattalionData { BattalionId = 1, IsBattalionChosen = 0 };

                ecb.AddComponent(soldierEntity, soldierTransform);
                ecb.AddSharedComponent(soldierEntity, soldierBattalionData);

                counter--;
            }

        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();

        state.Enabled = false;
    }
}
