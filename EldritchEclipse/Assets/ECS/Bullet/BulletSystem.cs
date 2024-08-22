using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Transforms;
// using UnityEngine;
using Unity.Burst;
using Unity.Physics;
using Unity.Burst.Intrinsics;
[BurstCompile]
public partial struct BulletSystem : ISystem
{
    EntityQuery query;
    ComponentLookup<EnemyComponent> enemyComponents;

    ComponentTypeHandle<BulletComponent> bulletHandler;
    ComponentTypeHandle<LocalTransform> transformHandler;
    ComponentTypeHandle<BulletLifeTimeComponent> bulletLifeTimeHandler;
    EntityTypeHandle entityTypeHandle;
    PhysicsWorld physicsWorld;

    public void OnCreate(ref SystemState state)
    {
        query = new EntityQueryBuilder(Allocator.Temp)
            .WithAllRW<LocalTransform>()
            .WithAllRW<BulletComponent>()
            .WithAllRW<BulletLifeTimeComponent>()
            .Build(ref state);

        enemyComponents = state.GetComponentLookup<EnemyComponent>();
        bulletLifeTimeHandler = state.GetComponentTypeHandle<BulletLifeTimeComponent>();
        bulletHandler = state.GetComponentTypeHandle<BulletComponent>();
        transformHandler = state.GetComponentTypeHandle<LocalTransform>();
        entityTypeHandle = state.GetEntityTypeHandle();
    }

    [BurstCompile]
    private void OnUpdate(ref SystemState state)
    {
        physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().PhysicsWorld;

        //UnityEngine.Profiling.Profiler.BeginSample("BulletSystem");

        //NaiveMethod(ref state); // 7.587952ms / 131fps 
        //QueryMethod(ref state); // 7.278624ms / 136fps
        //RunJob(ref state); // 3.957736ms / 252fps
        //RunJobChunk(ref state); // 5.403228ms / 184fps

        //UnityEngine.Profiling.Profiler.EndSample();
    }

    [BurstCompile]
    void RunJobChunk(ref SystemState state){

        float deltaTime = SystemAPI.Time.DeltaTime;
        var ecb = new EntityCommandBuffer(Allocator.TempJob);
        enemyComponents.Update(ref state);
        bulletHandler.Update(ref state);
        transformHandler.Update(ref state);
        bulletLifeTimeHandler.Update(ref state);
        entityTypeHandle.Update(ref state);

        var job = new BulletChunkJob
        {
            deltaTime = deltaTime,
            physicsWorld = physicsWorld,
            ecb = ecb.AsParallelWriter(),
            enemyComponents = enemyComponents,
            bulletLifeTimeHandler = bulletLifeTimeHandler,
            bulletHandler = bulletHandler,
            transformHandler = transformHandler,
            entityTypeHandle = entityTypeHandle,
        };
        var handle = job.ScheduleParallel(query,state.Dependency);
        state.Dependency = handle;

        state.Dependency.Complete();
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }

    [BurstCompile]
    void RunJob(ref SystemState state)
    {

        float deltaTime = SystemAPI.Time.DeltaTime;
        var ecb = new EntityCommandBuffer(Allocator.TempJob);
        enemyComponents.Update(ref state);

        var job = new BulletJob
        {
            deltaTime = deltaTime,
            physicsWorld = physicsWorld,
            ecb = ecb.AsParallelWriter(),
            enemyComponents = enemyComponents,
        };

        job.ScheduleParallel(query);
        
        state.Dependency.Complete(); // if removed, 5.91ms / 169fps
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }

    void QueryMethod2(ref SystemState state)
    {
        // PhysicsWorld physWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().PhysicsWorld;
        // float deltaTime = SystemAPI.Time.DeltaTime;

        // foreach (var (transform,bullet,lifetime,entity) in SystemAPI.Query<RefRW<LocalTransform>,RefRW<BulletComponent>,RefRW<BulletLifeTimeComponent>>().WithEntityAccess())
        // {
        //     transform.ValueRW.Position += bullet.ValueRW.Speed * deltaTime * transform.ValueRW.Forward();
        //     lifetime.ValueRW.RemainingLifeTime -= deltaTime;

        //     if(lifetime.ValueRO.RemainingLifeTime <= 0)
        //     {
        //         state.EntityManager.DestroyEntity(entity);       
        //         continue;
        //     }
        // }
    }

    [BurstCompile]
    void QueryMethod(ref SystemState state)
    {
        PhysicsWorld physWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().PhysicsWorld;

        var e = query.ToEntityArray(Allocator.Temp);
        var entityManager = state.EntityManager;

        foreach (var bullet in e)
        {
            //move the bullet
            LocalTransform bulletTransform = entityManager.GetComponentData<LocalTransform>(bullet);
            BulletComponent bulletComponent = entityManager.GetComponentData<BulletComponent>(bullet);

            bulletTransform.Position += bulletComponent.Speed * SystemAPI.Time.DeltaTime * bulletTransform.Forward();

            //set data
            entityManager.SetComponentData(bullet, bulletTransform);

            BulletLifeTimeComponent bltc = entityManager.GetComponentData<BulletLifeTimeComponent>(bullet);
            bltc.RemainingLifeTime -= SystemAPI.Time.DeltaTime;

            //destroy if no remaining time
            if (bltc.RemainingLifeTime <= 0)
            {
                entityManager.DestroyEntity(bullet);
                continue;
            }

            //set the modified data back
            entityManager.SetComponentData(bullet, bltc);

            //physics
            NativeList<ColliderCastHit> hits = new NativeList<ColliderCastHit>(Allocator.Temp);
            float3 point1 = new(bulletTransform.Position - bulletTransform.Forward() * 0.15f);
            float3 point2 = new(bulletTransform.Position + bulletTransform.Forward() * 0.15f);
            uint layerMask = LayerMaskHelper.GetLayerMaskFromTwoLayers(CollisionLayer.Wall, CollisionLayer.Enemy);
            physWorld.CapsuleCastAll(point1,
                point2,
                bulletComponent.Size / 2,
                float3.zero,
                1f,
                ref hits,
                new CollisionFilter
                {
                    BelongsTo = (uint)CollisionLayer.Default,
                    CollidesWith = layerMask,
                });



            if (hits.Length > 0)
            {
                for (int i = 0; i < hits.Length; i++)
                {
                    Entity hitEntity = hits[i].Entity;
                    if (entityManager.HasComponent<EnemyComponent>(hitEntity))
                    {
                        EnemyComponent enemyComponent = entityManager.GetComponentData<EnemyComponent>(hitEntity);
                        enemyComponent.CurrentHealth -= bulletComponent.Damage;
                        entityManager.SetComponentData(hitEntity, enemyComponent);

                        if (enemyComponent.CurrentHealth <= 0)
                        {
                            entityManager.DestroyEntity(hitEntity);
                        }
                    }
                }

                entityManager.DestroyEntity(bullet);

            }
            hits.Dispose();
        }
    }

    [BurstCompile]
    void NaiveMethod(ref SystemState state)
    {
        EntityManager entityManager = state.EntityManager;
        NativeArray<Entity> allEntities = entityManager.GetAllEntities();

        //get reference to the world's physics
        PhysicsWorldSingleton physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();

        //loop through all entities
        foreach (Entity entity in allEntities)
        {
            if (entityManager.HasComponent<BulletComponent>(entity) && entityManager.HasComponent<BulletLifeTimeComponent>(entity))
            {
                //move the bullet
                LocalTransform bulletTransform = entityManager.GetComponentData<LocalTransform>(entity);
                BulletComponent bulletComponent = entityManager.GetComponentData<BulletComponent>(entity);

                bulletTransform.Position += bulletComponent.Speed * SystemAPI.Time.DeltaTime * bulletTransform.Forward();

                //set data
                entityManager.SetComponentData(entity, bulletTransform);

                BulletLifeTimeComponent bltc = entityManager.GetComponentData<BulletLifeTimeComponent>(entity);
                bltc.RemainingLifeTime -= SystemAPI.Time.DeltaTime;

                //destroy if no remaining time
                if (bltc.RemainingLifeTime <= 0)
                {
                    entityManager.DestroyEntity(entity);
                    continue;
                }

                //set the modified data back
                entityManager.SetComponentData(entity, bltc);

                //physics
                NativeList<ColliderCastHit> hits = new NativeList<ColliderCastHit>(Allocator.Temp);
                float3 point1 = new(bulletTransform.Position - bulletTransform.Forward() * 0.15f);
                float3 point2 = new(bulletTransform.Position + bulletTransform.Forward() * 0.15f);
                uint layerMask = LayerMaskHelper.GetLayerMaskFromTwoLayers(CollisionLayer.Wall, CollisionLayer.Enemy);
                physicsWorld.CapsuleCastAll(point1,
                    point2,
                    bulletComponent.Size / 2,
                    float3.zero,
                    1f,
                    ref hits,
                    new CollisionFilter
                    {
                        BelongsTo = (uint)CollisionLayer.Default,
                        CollidesWith = layerMask,
                    });



                if (hits.Length > 0)
                {
                    for (int i = 0; i < hits.Length; i++)
                    {
                        Entity hitEntity = hits[i].Entity;
                        if (entityManager.HasComponent<EnemyComponent>(hitEntity))
                        {
                            EnemyComponent enemyComponent = entityManager.GetComponentData<EnemyComponent>(hitEntity);
                            enemyComponent.CurrentHealth -= bulletComponent.Damage;
                            entityManager.SetComponentData(hitEntity, enemyComponent);

                            if (enemyComponent.CurrentHealth <= 0)
                            {
                                entityManager.DestroyEntity(hitEntity);
                            }
                        }
                    }

                    entityManager.DestroyEntity(entity);

                }
                hits.Dispose();
            }
        }
    }

    //jobs
    [BurstCompile]
    public partial struct BulletJob : IJobEntity
    {
        public float deltaTime;
        [ReadOnly] public PhysicsWorld physicsWorld;
        public EntityCommandBuffer.ParallelWriter ecb;
        [ReadOnly] public ComponentLookup<EnemyComponent> enemyComponents;

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

            // Perform physics checks
            NativeList<ColliderCastHit> hits = new NativeList<ColliderCastHit>(Allocator.Temp);
            float3 point1 = new(bulletTransform.Position - bulletTransform.Forward() * 0.15f);
            float3 point2 = new(bulletTransform.Position + bulletTransform.Forward() * 0.15f);
            uint layerMask = LayerMaskHelper.GetLayerMaskFromTwoLayers(CollisionLayer.Wall, CollisionLayer.Enemy);

            physicsWorld.CollisionWorld.CapsuleCastAll(point1, point2, bulletComponent.Size / 2, float3.zero, 1f, ref hits, new CollisionFilter
            {
                BelongsTo = (uint)CollisionLayer.Default,
                CollidesWith = layerMask,
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

    [BurstCompile]
    public struct BulletChunkJob : IJobChunk
    {
        public float deltaTime;
        public ComponentTypeHandle<BulletComponent> bulletHandler;
        public ComponentTypeHandle<LocalTransform> transformHandler;
        public ComponentTypeHandle<BulletLifeTimeComponent> bulletLifeTimeHandler;

        [ReadOnly] public PhysicsWorld physicsWorld;
        [ReadOnly] public ComponentLookup<EnemyComponent> enemyComponents;
        public EntityCommandBuffer.ParallelWriter ecb;
        public EntityTypeHandle entityTypeHandle;
        public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
        {
            var bullets = chunk.GetNativeArray(ref bulletHandler);
            var transforms = chunk.GetNativeArray(ref transformHandler);
            var bulletLifeTimes = chunk.GetNativeArray(ref bulletLifeTimeHandler);
            var entities = chunk.GetNativeArray(entityTypeHandle);
            var enumerator = new ChunkEntityEnumerator(useEnabledMask, chunkEnabledMask, chunk.Count);

            while(enumerator.NextEntityIndex(out var i)){
                var bullet = bullets[i];
                var transform = transforms[i];
                var bulletLifeTime = bulletLifeTimes[i];
                var entity = entities[i];

                // Update bullet lifetime
                bulletLifeTime.RemainingLifeTime -= deltaTime;

                // Destroy bullet if no remaining time
                if (bulletLifeTime.RemainingLifeTime <= 0)
                {
                    ecb.DestroyEntity(unfilteredChunkIndex, entity);
                    continue;
                }

                // Move the bullet
                transform.Position += bullet.Speed * deltaTime * transform.Forward();              

                // Perform physics checks
                NativeList<ColliderCastHit> hits = new NativeList<ColliderCastHit>(Allocator.Temp);
                float3 point1 = transform.Position - transform.Forward() * 0.15f;
                float3 point2 = transform.Position + transform.Forward() * 0.15f;
                uint layerMask = LayerMaskHelper.GetLayerMaskFromTwoLayers(CollisionLayer.Wall, CollisionLayer.Enemy);

                physicsWorld.CollisionWorld.CapsuleCastAll(point1, point2, bullet.Size / 2, float3.zero, 1f, ref hits, new CollisionFilter
                {
                    BelongsTo = (uint)CollisionLayer.Default,
                    CollidesWith = layerMask,
                });

                // Process hits
                if (hits.Length > 0)
                {
                    for (int j = 0; j < hits.Length; j++)
                    {
                        Entity hitEntity = hits[j].Entity;

                        if (enemyComponents.HasComponent(hitEntity))
                        {
                            // Get the EnemyComponent safely using ComponentLookup
                            var enemyComponent = enemyComponents[hitEntity];
                            enemyComponent.CurrentHealth -= bullet.Damage;
                            ecb.SetComponent(unfilteredChunkIndex, hitEntity, enemyComponent);

                            if (enemyComponent.CurrentHealth <= 0)
                            {
                                ecb.DestroyEntity(unfilteredChunkIndex, hitEntity);
                            }
                        }

                        // Destroy the bullet entity after processing all hits
                        ecb.DestroyEntity(unfilteredChunkIndex, entity);
                    }
                }

                hits.Dispose(); // Don't forget to dispose of the native list!

                ecb.SetComponent(unfilteredChunkIndex, entity, transform);
                ecb.SetComponent(unfilteredChunkIndex, entity, bulletLifeTime);
                ecb.SetComponent(unfilteredChunkIndex, entity, bullet);
            }
        }
    }
}
