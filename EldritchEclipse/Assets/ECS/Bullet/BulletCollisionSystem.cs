using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Transforms;
using Unity.Burst;
using Unity.Physics;

[BurstCompile,DisableAutoCreation]
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public partial struct BulletCollisionSystem : ISystem
{
    PhysicsWorld physicsWorld;
    EntityCommandBuffer ecb;
    EntityManager entityManager;
    EntityQuery query;
    uint layerMask;

    BulletCollisionJob job;
    ComponentLookup<EnemyComponent> enemyComponents;



    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        query = new EntityQueryBuilder(Allocator.Temp)
            .WithAllRW<LocalTransform>()
            .WithAllRW<BulletComponent>()
            .WithAllRW<BulletLifeTimeComponent>()
            .Build(ref state);

        entityManager = state.EntityManager;

        layerMask = LayerMaskHelper.GetLayerMaskFromTwoLayers(CollisionLayer.Wall, CollisionLayer.Enemy);
        enemyComponents = state.GetComponentLookup<EnemyComponent>();

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        return;

        physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().PhysicsWorld;
        ecb = new EntityCommandBuffer(Allocator.TempJob);
        enemyComponents.Update(ref state);

        job = new BulletCollisionJob
        {
            physicsWorld = physicsWorld,
            enemyComponents = enemyComponents,
            ecb = ecb.AsParallelWriter(),
            mask = layerMask,
        };

        job.ScheduleParallel(query);
        state.Dependency.Complete();

        ecb.Playback(entityManager);
        ecb.Dispose();
    }

    [BurstCompile]
    public partial struct BulletCollisionJob : IJobEntity
    {
        [ReadOnly] public PhysicsWorld physicsWorld;
        [ReadOnly] public ComponentLookup<EnemyComponent> enemyComponents;
        public EntityCommandBuffer.ParallelWriter ecb;
        public uint mask;

        [BurstCompile]
        void Execute(Entity entity, [EntityIndexInQuery] int entityIndex, ref LocalTransform bulletTransform, ref BulletComponent bulletComponent)
        {
            NativeList<ColliderCastHit> hits = new NativeList<ColliderCastHit>(Allocator.Temp);
            float3 point1 = bulletTransform.Position - bulletTransform.Forward() * 0.15f;
            float3 point2 = bulletTransform.Position + bulletTransform.Forward() * 0.15f;

            physicsWorld.CollisionWorld.CapsuleCastAll(point1, point2, bulletComponent.Size / 2, float3.zero, 1f, ref hits, new CollisionFilter
            {
                BelongsTo = (uint)CollisionLayer.Default,
                CollidesWith = mask,
            });

            if (hits.Length > 0)
            {
                for (int i = 0; i < hits.Length; i++)
                {
                    Entity hitEntity = hits[i].Entity;

                    if (enemyComponents.HasComponent(hitEntity))
                    {
                        // Get the EnemyComponent safely using ComponentLookup
                        var enemyComponent = enemyComponents[hitEntity];
                        enemyComponent.CurrentHealth -= bulletComponent.Damage;
                        ecb.SetComponent(entityIndex, hitEntity, enemyComponent);

                        if (enemyComponent.CurrentHealth <= 0)
                        {
                            ecb.DestroyEntity(entityIndex, hitEntity);
                        }
                    }

                    ecb.DestroyEntity(entityIndex, entity);

                }
                hits.Dispose();
            }
        }
    }
}
