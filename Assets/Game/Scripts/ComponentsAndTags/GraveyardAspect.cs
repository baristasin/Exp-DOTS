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

        private readonly RefRO<GraveyardProperties> _graveyardProperties;
        private readonly RefRW<GraveyardRandom> _graveyardRandom;

        public readonly RefRO<GraveyardProperties> GraveyardProperties => _graveyardProperties;

        public LocalTransform GetRandomTombstoneTransform()
        {
            return new LocalTransform
            {
                Position = GetRandomPosition(),
                Rotation = GetRandomRotation(),
                Scale = GetRandomScale(0.5f)
            };
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


    }

}