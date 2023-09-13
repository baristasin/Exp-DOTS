using System;
using Unity.Entities;
using Unity.Mathematics;

public struct EnemyFieldProperties : IComponentData
{
    public float SpawnInterval;
    public float2 FieldDimensions;

    public Entity CubeEnemyPrefab;
    public Entity CapsuleEnemyPrefab;
    public Entity SphereEnemyPrefab;
}

