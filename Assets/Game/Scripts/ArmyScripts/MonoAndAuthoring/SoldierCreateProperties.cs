using UnityEngine;
using Unity.Entities;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class SoldierCreatePropsData
{
    public SoldierType SoldierType;
    public int SoldierCount;
}

public class SoldierCreateProperties : MonoBehaviour
{
    public GameObject SwordsmenObject;
    public GameObject ArcherObject;
    public GameObject KnightObject;

    public List<SoldierCreatePropsData> SoldierCreatePropsDatas;
}

public class SoldierCreatorBaker : Baker<SoldierCreateProperties>
{
    public override void Bake(SoldierCreateProperties authoring)
    {
        Entity entity = GetEntity(authoring, TransformUsageFlags.Dynamic);

        AddComponent(entity, new SoldierFactoryData
        {
            SwordsmenObject = GetEntity(authoring.SwordsmenObject,TransformUsageFlags.Dynamic),
            ArcherObject = GetEntity(authoring.ArcherObject, TransformUsageFlags.Dynamic),
            KnightObject = GetEntity(authoring.KnightObject, TransformUsageFlags.Dynamic),
        });

        var buffer = AddBuffer<SoldierCreationBufferElementData>(entity);

        for (int i = 0; i < authoring.SoldierCreatePropsDatas.Count; i++)
        {
            buffer.Add(new SoldierCreationBufferElementData
            {
                SoldierType = authoring.SoldierCreatePropsDatas[i].SoldierType,
                SoldierCount = authoring.SoldierCreatePropsDatas[i].SoldierCount
            });
        }
    }
}

