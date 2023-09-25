using System;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public readonly partial struct MinigunBulletAspect : IAspect
{
    public readonly Entity Entity;

    public readonly RefRW<LocalTransform> LocalTransform;
    public readonly RefRW<MinigunBullet> MinigunBullet;

}

