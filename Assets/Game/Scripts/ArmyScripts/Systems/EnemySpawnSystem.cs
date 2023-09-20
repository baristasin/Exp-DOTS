using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

//[DisableAutoCreation]
[UpdateInGroup(typeof(PresentationSystemGroup))]
public partial struct EnemySpawnSystem : ISystem
{
    //private BufferLookup<PositionInfoBufferElement> _positionInfoBuffer;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<EnemyFieldEnemyPrefabsData>();

        //_positionInfoBuffer = state.GetBufferLookup<PositionInfoBufferElement>(true);
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        //_positionInfoBuffer.Update(ref state);

        var enemyFieldEntity = SystemAPI.GetSingletonEntity<EnemyFieldEnemyPrefabsData>();
        var enemyFieldAspect = SystemAPI.GetAspect<EnemyFieldAspect>(enemyFieldEntity);

        if (enemyFieldAspect.EnemyFieldSpawnDatas.ValueRO.CurrentEnemyCount >= enemyFieldAspect.EnemyFieldSpawnDatas.ValueRO.MaxEnemyCount) return;

        if (enemyFieldAspect.EnemyFieldSpawnDatas.ValueRO.CurrentWaveEnemyCount > 0)
        {
            for (int i = 0; i < enemyFieldAspect.EnemyFieldSpawnDatas.ValueRO.WaveEnemyCount; i++)
            {
                var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();

                var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
                var chosenEnemy = ecb.Instantiate(enemyFieldAspect.GetRandomEnemy());
                var randomTransform = enemyFieldAspect.GetEnemyTransform(enemyFieldAspect.EnemyFieldSpawnDatas.ValueRO.CurrentWaveEnemyCount);

                ecb.AddComponent(chosenEnemy, randomTransform);

                enemyFieldAspect.EnemyFieldSpawnDatas.ValueRW.CurrentWaveEnemyCount -= 1;

                ecb.AppendToBuffer<PositionInfoBufferElement>(enemyFieldAspect.Entity, new float3(1, 1, 1));

                //if (_positionInfoBuffer.TryGetBuffer(enemyFieldAspect.Entity, out var bufferData) && bufferData.Length > 0)
                //{
                //    //Debug.Log(bufferData[0].Position);
                //}

                enemyFieldAspect.EnemyFieldSpawnDatas.ValueRW.CurrentEnemyCount += 1;

                if (enemyFieldAspect.EnemyFieldSpawnDatas.ValueRO.CurrentEnemyCount >= enemyFieldAspect.EnemyFieldSpawnDatas.ValueRO.MaxEnemyCount) return;
            }

        }
        else
        {
            enemyFieldAspect.EnemyFieldSpawnDatas.ValueRW.CurrentWaveSpawnInterval -= SystemAPI.Time.DeltaTime;

            if (enemyFieldAspect.EnemyFieldSpawnDatas.ValueRW.CurrentWaveSpawnInterval <= 0)
            {

                enemyFieldAspect.EnemyFieldSpawnDatas.ValueRW.CurrentWaveSpawnInterval = enemyFieldAspect.EnemyFieldSpawnDatas.ValueRO.WaveSpawnInterval;
                enemyFieldAspect.EnemyFieldSpawnDatas.ValueRW.CurrentWaveEnemyCount = enemyFieldAspect.EnemyFieldSpawnDatas.ValueRO.WaveEnemyCount;
            }
        }

    }
}

