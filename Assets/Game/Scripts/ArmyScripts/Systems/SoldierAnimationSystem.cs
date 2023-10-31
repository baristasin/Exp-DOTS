using GPUECSAnimationBaker.Engine.AnimatorSystem;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

[UpdateInGroup(typeof(PresentationSystemGroup))]
[BurstCompile]
public partial struct SoldierAnimationSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<GpuEcsAnimatorControlComponent>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (soldierAnimationData,controlComponent,entity) in SystemAPI.Query<SoldierAnimationData, GpuEcsAnimatorControlComponent>().WithEntityAccess())
        {
            switch (soldierAnimationData.AnimationType)
            {
                case AnimationType.Idle:
                    {
                        state.EntityManager.GetAspect<GpuEcsAnimatorAspect>(entity).RunAnimation(0, 1);
                        break;
                    }
                case AnimationType.Running:
                    {
                        state.EntityManager.GetAspect<GpuEcsAnimatorAspect>(entity).RunAnimation(1, 1);
                        break;
                    }
                case AnimationType.Attacking:
                    {
                        state.EntityManager.GetAspect<GpuEcsAnimatorAspect>(entity).RunAnimation(2, 1);
                        break;
                    }                
            }
        }        
    } 
}
