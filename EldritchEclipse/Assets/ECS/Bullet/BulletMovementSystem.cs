using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

[BurstCompile]
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public partial struct BulletMovementSystem : ISystem
{
    EntityQuery query;
    EntityManager entityManager;
    EntityCommandBuffer ecb;

    float deltaTime;
    BulletMovementJob job;
    public void OnCreate(ref SystemState state)
    {
        query = new EntityQueryBuilder(Allocator.Temp)
            .WithAllRW<LocalTransform>()
            .WithAllRW<BulletComponent>()
            .WithAllRW<BulletLifeTimeComponent>()
            .Build(ref state);

        entityManager = state.EntityManager;
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        return;
        deltaTime = SystemAPI.Time.DeltaTime;
        ecb = new EntityCommandBuffer(Allocator.TempJob);

        //run job

        RunJob(ref state);

        ecb.Dispose();
    }

    [BurstCompile]
    void RunJob(ref SystemState state)
    {
        job = new BulletMovementJob
        {
            deltaTime = deltaTime,
            ecb = ecb.AsParallelWriter()
        };

        job.ScheduleParallel(query);

        state.Dependency.Complete();
        ecb.Playback(entityManager);
    }

    [BurstCompile]
    public partial struct BulletMovementJob : IJobEntity
    {
        public float deltaTime;
        public EntityCommandBuffer.ParallelWriter ecb;

        [BurstCompile]
        void Execute(Entity entity, [EntityIndexInQuery] int entityIndex,
            ref LocalTransform bulletTransform,
            ref BulletComponent bulletComponent,
            ref BulletLifeTimeComponent bltc)
        {
            // Update bullet lifetime
            bltc.RemainingLifeTime -= deltaTime;

            // Destroy bullet if no remaining time
            if (bltc.RemainingLifeTime <= 0)
            {
                ecb.DestroyEntity(entityIndex, entity);
                return;
            }

            
            // Move the bullet
            bulletTransform.Position += bulletComponent.Speed * deltaTime * bulletTransform.Forward();
        }
    }
}
