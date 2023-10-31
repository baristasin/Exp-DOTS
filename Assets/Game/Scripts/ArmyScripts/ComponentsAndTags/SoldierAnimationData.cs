using Unity.Entities;
using UnityEngine;

public enum AnimationType
{
    Idle,
    Running,
    Attacking
}

public struct SoldierAnimationData : IComponentData
{
    public AnimationType AnimationType;
}
