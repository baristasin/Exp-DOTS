using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;

public class SoldierMono : MonoBehaviour
{
    public float MovementSpeed;
}

public class SoldierBaker : Baker<SoldierMono>
{
    public override void Bake(SoldierMono authoring)
    {
        Entity entity = GetEntity(authoring, TransformUsageFlags.Dynamic);

        AddComponent(entity, new SoldierMovementData { MovementSpeed = authoring.MovementSpeed });
    }
}

