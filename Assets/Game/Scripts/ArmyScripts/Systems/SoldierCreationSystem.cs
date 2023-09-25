using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

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
            for (int i = 0; i < 25; i++)
            {
                var soldierEntity = ecb.Instantiate(soldierCreatorData.SoldierEntity);

                LocalTransform soldierTransform = new LocalTransform
                {
                    Position = new float3(0, 0, 0),
                    Rotation = quaternion.identity,
                    Scale = 1f
                };

                SoldierBattalionData soldierBattalionData = new SoldierBattalionData { BattalionId = 1 };

                ecb.AddComponent(soldierEntity, soldierTransform);
                ecb.AddSharedComponent(soldierEntity, soldierBattalionData);
            }

        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();

        state.Enabled = false;
    }
}
