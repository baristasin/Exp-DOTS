﻿using System;
using Unity.Entities;
using UnityEngine;

namespace Game.Scripts
{
    public class ZombieMono : MonoBehaviour
    {
        public float RiseRate;
        public float WalkSpeed;
        public float WalkAmplitude;
        public float WalkFrequency;
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

            AddComponent(entity, new ZombieWalkProperties
            {
                WalkSpeed = authoring.WalkSpeed,
                WalkAmplitude = authoring.WalkAmplitude,
                WalkFrequency = authoring.WalkFrequency
            });

            AddComponent(entity, new ZombieTimer());
            AddComponent(entity, new ZombieHeading());
        }
    }

}