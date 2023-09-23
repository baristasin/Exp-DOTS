using System;
using Unity.Entities;
using Unity.Mathematics;

public struct EnemyFieldEnemyPrefabsData : IComponentData
{
    public Entity CubeEnemyPrefab;
    public Entity CapsuleEnemyPrefab;
    public Entity SphereEnemyPrefab;

    public Entity MinigunPrefab;
}

