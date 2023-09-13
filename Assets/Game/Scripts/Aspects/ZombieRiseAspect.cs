using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Game.Scripts
{
    public readonly partial struct ZombieRiseAspect : IAspect
    {
        public readonly Entity Entity;

        private readonly RefRW<LocalTransform> _transform;

        private readonly RefRO<ZombieRiseRate> _zombieRiseRate;

        public void Rise(float deltaTime)
        {
            _transform.ValueRW.Position += math.up() * 3f * deltaTime;
        }

        public bool IsAboveGround => _transform.ValueRO.Position.y >= 3f;

        public void SetAtGroundLevel()
        {
            var currentPosition = _transform.ValueRO.Position;
            currentPosition.y = 3f;
            _transform.ValueRW.Position = currentPosition;
        }

    }

}