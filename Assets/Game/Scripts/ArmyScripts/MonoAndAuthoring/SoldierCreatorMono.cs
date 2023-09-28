using UnityEngine;
using Unity.Entities;
using System.Collections;

public class SoldierCreatorMono : MonoBehaviour
{
    public GameObject SoldierObject;
    public int SoldierCount;
}

public class SoldierCreatorBaker : Baker<SoldierCreatorMono>
{
    public override void Bake(SoldierCreatorMono authoring)
    {
        Entity entity = GetEntity(authoring, TransformUsageFlags.Dynamic);

        AddComponent(entity, new SoldierCreatorData
        {
            SoldierEntity = GetEntity(authoring.SoldierObject, TransformUsageFlags.Dynamic),
            SoldierCount = authoring.SoldierCount
        });

    }
}

