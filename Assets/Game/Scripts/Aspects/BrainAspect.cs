using System;
using Unity.Entities;
using Unity.Transforms;

namespace Game.Scripts
{
    public readonly partial struct BrainAspect : IAspect
    {
        public readonly Entity Entity;
        private readonly RefRW<LocalTransform> _transform;
        private readonly RefRW<BrainHealth> _brainHealth;
        private readonly DynamicBuffer<BrainDamageBufferElement> _brainDamageBuffer;

        public void DamageBrain()
        {
            foreach (var element in _brainDamageBuffer)
            {
                _brainHealth.ValueRW.CurrentHealth -= element.Value;
            }

            _brainDamageBuffer.Clear();

            _transform.ValueRW.Scale = (_brainHealth.ValueRO.CurrentHealth / _brainHealth.ValueRO.MaxHealth) * 5.5f;

        }
    }

}