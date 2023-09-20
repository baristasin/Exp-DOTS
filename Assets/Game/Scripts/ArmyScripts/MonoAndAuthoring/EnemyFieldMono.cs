using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class EnemyFieldMono : MonoBehaviour
{
    public float SpawnInterval;
    public float2 FieldDimensions;
    public int WaveEnemyCount;
    public int MaxEnemyCount;
    public uint RandomSeedValue;

    public GameObject CubePrefab;
    public GameObject CapsulePrefab;
    public GameObject SpherePrefab;

}

public class EnemyFieldBaker : Baker<EnemyFieldMono>
{
    public override void Bake(EnemyFieldMono authoring)
    {
        Entity entity = GetEntity(authoring, TransformUsageFlags.Dynamic);

        AddComponent(entity, new EnemyFieldSpawnDatas
        {
            WaveSpawnInterval = authoring.SpawnInterval,
            WaveEnemyCount = authoring.WaveEnemyCount,
            CurrentWaveSpawnInterval = authoring.SpawnInterval,
            CurrentWaveEnemyCount = authoring.WaveEnemyCount,
            MaxEnemyCount = authoring.MaxEnemyCount
        });

        AddComponent(entity, new EnemyFieldPositionDatas
        {
            FieldDimensions = authoring.FieldDimensions
        });

        AddComponent(entity, new EnemyFieldEnemyPrefabsData
        {
            CubeEnemyPrefab = GetEntity(authoring.CubePrefab, TransformUsageFlags.Dynamic),
            CapsuleEnemyPrefab = GetEntity(authoring.CapsulePrefab, TransformUsageFlags.Dynamic),
            SphereEnemyPrefab = GetEntity(authoring.SpherePrefab, TransformUsageFlags.Dynamic)
        });

        AddComponent(entity, new EnemyFieldRandom
        {
            Value = Random.CreateFromIndex(authoring.RandomSeedValue)
        });

        AddBuffer<PositionInfoBufferElement>(entity);
    }
}

