using UnityEngine;
using System.Collections;
using Unity.Entities;

namespace Game.Scripts
{
    public class BrainMono : MonoBehaviour
    {
        public float MaxHealth;
    }

    public class BrainBaker : Baker<BrainMono>
    {
        public override void Bake(BrainMono authoring)
        {
            Entity entity = GetEntity(authoring, TransformUsageFlags.Dynamic);

            AddComponent(entity, new BrainHealth
            {
                MaxHealth = authoring.MaxHealth,
                CurrentHealth = authoring.MaxHealth
            });

            AddComponent(entity, new BrainTag());
            AddBuffer<BrainDamageBufferElement>(entity);
        }
    }
}