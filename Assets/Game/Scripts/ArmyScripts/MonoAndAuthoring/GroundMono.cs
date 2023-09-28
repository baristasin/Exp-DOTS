using UnityEngine;
using Unity.Entities;
using System.Collections;

public class GroundMono : MonoBehaviour
{
    
}

public class GroundBaker : Baker<GroundMono>
{
    public override void Bake(GroundMono authoring)
    {
        Entity entity = GetEntity(authoring, TransformUsageFlags.Dynamic);

        AddComponent(entity, new GroundTag());
    }
}

