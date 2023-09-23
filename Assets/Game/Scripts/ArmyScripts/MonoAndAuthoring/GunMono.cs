using UnityEngine;
using Unity.Entities;
using System.Collections;
using Unity.Mathematics;
using Unity.Transforms;

public class GunMono : MonoBehaviour
{   
    public GameObject GunBulletPrefab;
    public GameObject GunRenderer;
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
            GunRenderer = GetEntity(authoring.GunRenderer,TransformUsageFlags.Dynamic),
            GunType = authoring.GunType,
            GunPositionXZ = new float2(authoring.transform.position.x, authoring.transform.position.z)
        });

        AddComponent(entity, new GunTimer
        {
            ShootInterval = authoring.GunShootInterval            
        });
    }
}

