using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[DisableAutoCreation]
[UpdateInGroup(typeof(PresentationSystemGroup))]
[UpdateAfter(typeof(EnemySpawnSystem))]

public partial struct PlayerAnimationSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (enemyVisualObjectData,entity) in SystemAPI.Query<EnemyVisualObjectData>().WithNone<EnemyAnimatorData>().WithEntityAccess())
        {
            var newCompanionObject = Object.Instantiate(enemyVisualObjectData.EnemyVisualObject);
            var newAnimatorRef = new EnemyAnimatorData
            {
                EnemyAnimator = newCompanionObject.GetComponent<Animator>()
            };

            ecb.AddComponent(entity, newAnimatorRef);
        }

        foreach (var (transform,animatorRef) in SystemAPI.Query<LocalTransform,EnemyAnimatorData>())
        {
            animatorRef.EnemyAnimator.transform.localPosition = new float3(transform.Position.x, transform.Position.y - 2.6f, transform.Position.z);
            animatorRef.EnemyAnimator.transform.localRotation = transform.Rotation;
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }

}
