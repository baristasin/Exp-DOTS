using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public struct SoldierChaseData : IComponentData
{
    public int EnemyBattalionId;
    public LocalTransform EnemyLocalTransform;
    public Entity EnemyEntity;
}
