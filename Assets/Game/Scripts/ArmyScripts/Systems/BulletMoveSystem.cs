using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(GunShootingSystem))]
public partial struct BulletMoveSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var enemyFieldEntity = SystemAPI.GetSingletonEntity<EnemyFieldSpawnDatas>();
        var enemyFieldAspect = SystemAPI.GetAspect<EnemyFieldAspect>(enemyFieldEntity);

        var deltaTime = SystemAPI.Time.DeltaTime;

        var ecb = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (minigunBulletTransform,minigunBullet,bulletEntity) in SystemAPI.Query<RefRW<LocalTransform>,RefRW<MinigunBullet>>().WithEntityAccess())
        {
            minigunBulletTransform.ValueRW.Position += minigunBulletTransform.ValueRO.Forward() * (minigunBullet.ValueRO.BulletSpeed * deltaTime);

            minigunBullet.ValueRW.CurrentLifeTimeValue += deltaTime;

            if(minigunBullet.ValueRO.CurrentLifeTimeValue >= minigunBullet.ValueRO.MaxLifeTimeValue)
            {
                ecb.DestroyEntity(bulletEntity);
                break;
            }

            foreach(var (enemyTransform,enemyMovement,enemyEntity) in SystemAPI.Query<RefRO<LocalTransform>,RefRO<EnemyMovementData>>().WithEntityAccess())
            {
                var minigunBulletXZ = new float2(minigunBulletTransform.ValueRO.Position.x, minigunBulletTransform.ValueRO.Position.z);
                var enemyXZ = new float2(enemyTransform.ValueRO.Position.x, enemyTransform.ValueRO.Position.z);

                if (math.distance(minigunBulletXZ, enemyXZ) < 1f)
                {
                    ecb.DestroyEntity(enemyEntity);
                    ecb.DestroyEntity(bulletEntity);
                    enemyFieldAspect.EnemyFieldSpawnDatas.ValueRW.CurrentEnemyCount -= 1;
                    break;
                }
            }
        }

        ecb.Playback(state.EntityManager);
    }
}


