using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

//[DisableAutoCreation]
[UpdateInGroup(typeof(InitializationSystemGroup))]
[UpdateAfter(typeof(UnitCirclePlacementSystem))]
public partial struct UnitCircleAlignmentSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<UnitCirclePropertiesData>();
        state.RequireForUpdate<GroundInputData>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        int counter = 0;
        int lineIndex = 0;
        int lineMaximumSoldierCount = 10;

        foreach (var (unitCircleTransform,unitCircleData) in SystemAPI.Query<RefRW<LocalTransform>,RefRW<UnitCircleData>>())
        {
            var inputDataEntity = SystemAPI.GetSingletonEntity<GroundInputData>();
            var inputDataAspect = SystemAPI.GetAspect<GroundInputAspect>(inputDataEntity);

            FormationHelper.FormationFuncLocalTransform(inputDataAspect, ref state, ref counter, ref lineIndex, ref lineMaximumSoldierCount, unitCircleTransform, unitCircleData);            
        }
    }    
}

