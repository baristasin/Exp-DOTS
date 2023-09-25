using UnityEngine;
using Unity.Entities;
using System.Collections;

public class EnemyMono : MonoBehaviour
{
    public float MovementSpeed;
    public GameObject EnemyVisualObject;
}

public class EnemyBaker : Baker<EnemyMono>
{
    public override void Bake(EnemyMono authoring)
    {
        Entity entity = GetEntity(authoring, TransformUsageFlags.Dynamic);

        AddComponent(entity, new EnemyMovementData { MoveSpeed = authoring.MovementSpeed });

        AddComponentObject(entity, new EnemyVisualObjectData
        {
            EnemyVisualObject = authoring.EnemyVisualObject
        });

        AddComponent(entity, new EnemyDeadTag());
    }
}

