using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Transforms;
using UnityEngine;
using Unity.Burst;
using Unity.Physics;

[BurstCompile]
public partial struct BulletSystem : ISystem
{
    [BurstCompile]
    private void OnUpdate(ref SystemState state)
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
