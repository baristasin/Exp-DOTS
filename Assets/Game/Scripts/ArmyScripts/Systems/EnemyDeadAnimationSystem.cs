using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

//[DisableAutoCreation]
[UpdateInGroup(typeof(PresentationSystemGroup))]
[UpdateAfter(typeof(EnemySpawnSystem))]

public partial struct EnemyDeadAnimationSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<EnemyDeadTag>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var deltaTime = SystemAPI.Time.DeltaTime;
        var ecb = new EntityCommandBuffer(Allocator.TempJob);

        new DeadAnimateJob
        {
            EntityCommandBuffer = ecb,
            DeltaTime = deltaTime
        }.Run();

        ecb.Playback(state.EntityManager);
    }

}

public partial struct DeadAnimateJob : IJobEntity
{
    public EntityCommandBuffer EntityCommandBuffer;
    public float DeltaTime;

    public void Execute(DeadEnemyMoveAspect deadEnemyMoveAspect)
    {
        //EntityCommandBuffer.DestroyEntity(deadEnemyMoveAspect.Entity);
        deadEnemyMoveAspect.AnimateAndKillEnemy(EntityCommandBuffer,DeltaTime);
    }
}