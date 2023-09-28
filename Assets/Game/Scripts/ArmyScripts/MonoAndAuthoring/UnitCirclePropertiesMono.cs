using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class UnitCirclePropertiesMono : MonoBehaviour
{
    public GameObject UnitCircleObject;
    public int SoldierCount;
}

public class UnitCirclePropertiesBaker : Baker<UnitCirclePropertiesMono>
{
    public override void Bake(UnitCirclePropertiesMono authoring)
    {
        Entity entity = GetEntity(authoring, TransformUsageFlags.Dynamic);

        AddComponent(entity, new UnitCirclePropertiesData
        {
            UnitCircleEntity = GetEntity(authoring.UnitCircleObject, TransformUsageFlags.Dynamic),
            CurrentSelectedSoldierCount = authoring.SoldierCount
        });
    }
}
