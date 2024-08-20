using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Transforms;
// using UnityEngine;
using Unity.Burst;
using Unity.Physics;

[BurstCompile]
public partial struct BulletSystem : ISystem
{
    EntityQuery query;
    public void OnCreate(ref SystemState state)
    {
        query = new EntityQueryBuilder(Allocator.Temp)
            .WithAllRW<LocalTransform>()
            .WithAllRW<BulletComponent>()
            .WithAllRW<BulletLifeTimeComponent>()
            .Build(ref state);
    }

    [BurstCompile]
    private void OnUpdate(ref SystemState state)
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
        //OldMethod(ref state);
        QueryMethod(ref state);
        
    }

    void QueryMethod(ref SystemState state){
        PhysicsWorld physWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().PhysicsWorld;

        var e = query.ToEntityArray(Allocator.Temp);
        var entityManager = state.EntityManager;

        foreach(var bullet in e){
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

    void OldMethod(ref SystemState state)
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
}
