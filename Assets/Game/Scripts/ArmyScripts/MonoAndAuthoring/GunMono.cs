using UnityEngine;
using Unity.Entities;
using System.Collections;

public class GunMono : MonoBehaviour
{
    public GameObject GunBulletPrefab;
    public GunType GunType;
    public float GunShootInterval;
}

public class GunBaker : Baker<GunMono>
{
    public override void Bake(GunMono authoring)
    {
        Entity entity = GetEntity(authoring, TransformUsageFlags.Dynamic);

        AddComponent(entity, new GunData
        {
            GunBulletPrefab = GetEntity(authoring.GunBulletPrefab, TransformUsageFlags.Dynamic),
            GunType = authoring.GunType
        });

        AddComponent(entity, new GunTimer
        {
            ShootInterval = authoring.GunShootInterval            
        });
    }
}

