using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class UnitCircleMono : MonoBehaviour
{

}

public class UnitCircleBaker : Baker<UnitCircleMono>
{
    public override void Bake(UnitCircleMono authoring)
    {
        Entity entity = GetEntity(authoring, TransformUsageFlags.Dynamic);

        AddComponent(entity, new UnitCircleData());
    }
}