using System;
using Unity.Entities;
using UnityEngine;

namespace Game.Scripts
{
    public class ZombieMono : MonoBehaviour
    {
        public float RiseRate;
    }

    public class ZombieBaker : Baker<ZombieMono>
    {
        public override void Bake(ZombieMono authoring)
        {
            Entity entity = GetEntity(authoring, TransformUsageFlags.Dynamic);

            AddComponent(entity, new ZombieRiseRate
            {
                RiseRate = authoring.RiseRate
            });
        }
    }

}