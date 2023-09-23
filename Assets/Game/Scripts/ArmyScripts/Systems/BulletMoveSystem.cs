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
        var ecbSingleton = SystemAPI.GetSingleton<BeginPresentationEntityCommandBufferSystem.Singleton>();

        var enemyFieldEntity = SystemAPI.GetSingletonEntity<EnemyFieldSpawnDatas>();
        var enemyFieldAspect = SystemAPI.GetAspect<EnemyFieldAspect>(enemyFieldEntity);

        var deltaTime = SystemAPI.Time.DeltaTime;

        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        //PARALLELIZE THIS

        //new BulletMoveJob
        //{
        //    DeltaTime = deltaTime,
        //    EntityCommandBuffer = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter()
        //}.ScheduleParallel();

        //Move minigun bullet, if lifetime passed, kill it.
        foreach (var (minigunBulletTransform, minigunBullet, bulletEntity) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<MinigunBullet>>().WithEntityAccess())
        {
            minigunBulletTransform.ValueRW.Position += minigunBulletTransform.ValueRO.Forward() * (state.EntityManager.GetSharedComponent<MinigunBulletSpeedData>(bulletEntity).BulletSpeed * deltaTime);

            minigunBullet.ValueRW.LifeTimeValue -= deltaTime;

            if (minigunBullet.ValueRO.LifeTimeValue <= 0)
            {
                ecb.DestroyEntity(bulletEntity);
                break;
            }

            // if minigun and enemy fake collides, kill both
            foreach (var (enemyTransform, enemyMovement, enemyEntity) in SystemAPI.Query<RefRO<LocalTransform>, RefRO<EnemyMovementData>>().WithEntityAccess().WithNone<EnemyDeadTag>())
            {
                var minigunBulletXZ = new float2(minigunBulletTransform.ValueRO.Position.x, minigunBulletTransform.ValueRO.Position.z);
                var enemyXZ = new float2(enemyTransform.ValueRO.Position.x, enemyTransform.ValueRO.Position.z);

                if (math.distance(minigunBulletXZ, enemyXZ) < 1f)
                {
                    //ecb.DestroyEntity(enemyEntity);
                    ecb.DestroyEntity(bulletEntity);
                    ecb.SetComponentEnabled<EnemyMovementData>(enemyEntity,false);
                    ecb.SetComponent<EnemyDeadTag>(enemyEntity, new EnemyDeadTag
                    {
                        HitDirection = math.normalize(enemyXZ - minigunBulletXZ)
                    });
                    ecb.SetComponentEnabled<EnemyDeadTag>(enemyEntity,true);
                    enemyFieldAspect.EnemyFieldSpawnDatas.ValueRW.CurrentEnemyCount -= 1;
                    break;
                }
            }
        }
    }
}

//public partial struct BulletMoveJob : IJobEntity
//{
//    public EntityCommandBuffer.ParallelWriter EntityCommandBuffer;
//    public float DeltaTime;

//    public void Execute(MinigunBulletAspect minigunBulletAspect, [EntityIndexInQuery] int queryIndex)
//    {
//        minigunBulletAspect.LocalTransform.ValueRW.Position += minigunBulletAspect.LocalTransform.ValueRO.Forward()
//            * 50f * DeltaTime;

//        minigunBulletAspect.MinigunBullet.ValueRW.LifeTimeValue -= DeltaTime;

//        if (minigunBulletAspect.MinigunBullet.ValueRW.LifeTimeValue <= 0)
//        {
//            EntityCommandBuffer.DestroyEntity(queryIndex,minigunBulletAspect.Entity);
//            return;
//        }
//    }
//}


