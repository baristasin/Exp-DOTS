using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

//[DisableAutoCreation]
[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct EnemyMovementSystem : ISystem
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
        var deltaTime = SystemAPI.Time.DeltaTime;

        //foreach(var (transform,movementData) in SystemAPI.Query<RefRW<LocalTransform>,RefRO<EnemyMovementData>>())
        //{
        //    transform.ValueRW.Position += transform.ValueRW.Forward() * movementData.ValueRO.MoveSpeed * deltaTime;
        //}

        new EnemyMovementJob
        {
            DeltaTime = deltaTime
        }.Schedule();
    }
}

public partial struct EnemyMovementJob : IJobEntity
{
    public float DeltaTime;

    public void Execute(EnemyMoveAspect enemyMoveAspect)
    {
        enemyMoveAspect.Move(DeltaTime);
    }
}

