using System;
using Unity.Entities;
using Unity.Mathematics;

public struct EnemyFieldSpawnDatas : IComponentData
{
    public float WaveSpawnInterval;
    public int WaveEnemyCount;
    public int MaxEnemyCount;

    public float CurrentWaveSpawnInterval;
    public int CurrentWaveEnemyCount;
    public int CurrentEnemyCount;
}

