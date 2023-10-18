using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = UnityEngine.Random;

//[DisableAutoCreation]
[UpdateInGroup(typeof(InitializationSystemGroup))]
[BurstCompile]
public partial struct SoldierCreationSystem : ISystem
{
    public BufferLookup<SoldierCreationBufferElementData> _soldierCreationBufferElementData;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<SoldierFactoryData>();
        _soldierCreationBufferElementData = state.GetBufferLookup<SoldierCreationBufferElementData>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var soldierCreatePropertiesEntity = SystemAPI.GetSingletonEntity<SoldierFactoryData>();
        var soldierCreatePropertiesAspect = SystemAPI.GetAspect<SoldierCreatePropertiesAspect>(soldierCreatePropertiesEntity);

        var ecb = new EntityCommandBuffer(Allocator.Temp);

        _soldierCreationBufferElementData.Update(ref state);

        if (_soldierCreationBufferElementData.TryGetBuffer(soldierCreatePropertiesEntity, out var soldierCreatePropsBuffer))
        {

            for (int i = 0; i < soldierCreatePropsBuffer.Length; i++)
            {
                var horizontalBattalionOffset = 20f * i;

                int counter = 10;
                int lineIndex = soldierCreatePropsBuffer[i].SoldierCount / 10;

                for (int j = 0; j < soldierCreatePropsBuffer[i].SoldierCount; j++)
                {
                    if (counter <= 0)
                    {
                        counter = 10;
                        lineIndex--;
                    }

                    Entity createEntityType = soldierCreatePropertiesEntity;
                    switch (soldierCreatePropsBuffer[i].SoldierType)
                    {
                        case SoldierType.Swordsmen:
                            {
                                createEntityType = soldierCreatePropertiesAspect.SoldierFactoryData.ValueRO.SwordsmenObject;
                                break;
                            }
                        case SoldierType.Archer:
                            {
                                createEntityType = soldierCreatePropertiesAspect.SoldierFactoryData.ValueRO.ArcherObject;
                                break;
                            }
                        case SoldierType.Knight:
                            {
                                createEntityType = soldierCreatePropertiesAspect.SoldierFactoryData.ValueRO.KnightObject;
                                break;
                            }
                    }

                    var soldierEntity = ecb.Instantiate(createEntityType);

                    LocalTransform soldierTransform = new LocalTransform
                    {
                        Position = new float3(counter * 1.3f + horizontalBattalionOffset, 1.5f, -lineIndex * 1.3f),
                        Rotation = quaternion.identity,
                        Scale = 1f
                    };

                    ecb.AddComponent(soldierEntity, new SoldierMovementData
                    {
                        MovementSpeed = Random.Range(7f, 10f),
                        TargetPosition = soldierTransform.Position,
                        TargetRotation = soldierTransform.Rotation,
                        SoldierCounterAndLineValues = new float2(counter, lineIndex)
                    });

                    SoldierBattalionIdData soldierBattalionIdData = new SoldierBattalionIdData { BattalionId = i };
                    SoldierBattalionIsChosenData soldierBattalionIsChosenData = new SoldierBattalionIsChosenData { IsBattalionChosen = 0 };

                    ecb.AddComponent(soldierEntity, soldierTransform);
                    ecb.AddSharedComponent(soldierEntity, soldierBattalionIdData);
                    ecb.AddSharedComponent(soldierEntity, soldierBattalionIsChosenData);

                    counter--;

                    if(i == soldierCreatePropsBuffer.Length - 1)
                    {
                        ecb.AddComponent(soldierEntity, new EnemySoldierTag());
                    }
                }
            }

        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();

        var visualEcb = new EntityCommandBuffer(Allocator.Temp);
        foreach (var visualData in SystemAPI.Query<SoldierVisualData>())
        {
            visualEcb.SetEnabled(visualData.SelectedVisualObject, false);
        }

        visualEcb.Playback(state.EntityManager);
        visualEcb.Dispose();

        state.Enabled = false;
    }
}
