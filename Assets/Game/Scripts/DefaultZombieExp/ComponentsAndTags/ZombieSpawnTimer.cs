using System;
using Unity.Entities;

namespace Game.Scripts
{
    public struct ZombieSpawnTimer : IComponentData
    {
        public float TimerValue;
    }
}
