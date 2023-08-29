using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Game.Scripts
{
    public readonly partial struct GraveyardAspect : IAspect
    {
        public readonly Entity Entity;

        //private readonly TransformAuthoring transformAuthoring;
        private readonly RefRO<LocalTransform> _transform;
        private LocalTransform Transform => _transform.ValueRO;

        private readonly RefRW<GraveyardProperties> _graveyardProperties;
        private readonly RefRW<GraveyardRandom> _graveyardRandom;
        private readonly RefRW<ZombieSpawnPoints> _zombieSpawnPoints;
        private readonly RefRW<ZombieSpawnTimer> _zombieSpawnTimer;

        public readonly RefRW<GraveyardProperties> GraveyardProperties => _graveyardProperties;
        public readonly RefRW<ZombieSpawnPoints> ZombieSpawnPoints => _zombieSpawnPoints;

        public LocalTransform GetRandomTombstoneTransform()
        {
            return new LocalTransform
            {
                Position = GetRandomPosition(),
                Rotation = GetRandomRotation(),
                Scale = GetRandomScale(0.5f)
            };
        }

        public LocalTransform GetRandomZombieSpawnTransform()
        {
            var zombiePosition = _zombieSpawnPoints.ValueRO.SpawnPoints.Value.ZombieSpawnPointBlobArray[_graveyardRandom.ValueRW.Value.NextInt(0,250)];

            return new LocalTransform
            {
                Position = zombiePosition,
                Rotation = quaternion.RotateY(RotateTowards(zombiePosition,Transform.Position)),
                Scale = 1f
            };
        }

        private float RotateTowards(float3 objectsPosition, float3 targetPosition)
        {
            var x = objectsPosition.x - targetPosition.x;
            var y = objectsPosition.z - targetPosition.z;

            return math.atan2(x, y) + math.PI;
        }

        private float3 GetRandomPosition()
        {
            float3 randomPosition;

            do
            {
                randomPosition = new float3
                {
                    x = _graveyardRandom.ValueRW.Value.NextFloat(-_graveyardProperties.ValueRO.FieldDimensions.x / 2,
                        _graveyardProperties.ValueRO.FieldDimensions.x / 2),
                    y = 0.5f,
                    z = _graveyardRandom.ValueRW.Value.NextFloat(-_graveyardProperties.ValueRO.FieldDimensions.y / 2,
                        _graveyardProperties.ValueRO.FieldDimensions.y / 2)
                };
            } while (math.distancesq(Transform.Position, randomPosition) <= 50);

            return randomPosition;


        }

        private quaternion GetRandomRotation()
        {
            return quaternion.RotateY(_graveyardRandom.ValueRW.Value.NextFloat(-0.25f, 0.25f));
        }

        private float GetRandomScale(float baseNumber)
        {
            return _graveyardRandom.ValueRW.Value.NextFloat(baseNumber, 1f);
        }

        public float ZombieSpawnTimer
        {
            get => _zombieSpawnTimer.ValueRO.TimerValue;
            set => _zombieSpawnTimer.ValueRW.TimerValue = value;
        }

        public bool IsRightTimeToSpawnZombie => ZombieSpawnTimer <= 0;
    }

}