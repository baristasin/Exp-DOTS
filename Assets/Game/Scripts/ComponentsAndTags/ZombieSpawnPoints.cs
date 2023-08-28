using System;
using Unity.Entities;

namespace Game.Scripts
{
    public struct ZombieSpawnPoints : IComponentData
    {
        public BlobAssetReference<ZombieSpawnPointBlobAsset> SpawnPoints;
    }
}