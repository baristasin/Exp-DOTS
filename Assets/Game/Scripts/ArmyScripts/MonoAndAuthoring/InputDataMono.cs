using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class InputDataMono : MonoBehaviour
{

}

public class InputDataBaker : Baker<InputDataMono>
{
    public override void Bake(InputDataMono authoring)
    {
        Entity entity = GetEntity(authoring, TransformUsageFlags.Dynamic);

        AddComponent(entity, new InputData());
    }
}
