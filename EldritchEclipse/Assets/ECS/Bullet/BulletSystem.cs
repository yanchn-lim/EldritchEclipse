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

        foreach(Entity entity in allEntities)
        {
            if(entityManager.HasComponent<BulletComponent>(entity) && entityManager.HasComponent<BulletLifeTimeComponent>(entity))
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
                if(bltc.RemainingLifeTime <= 0){
                    entityManager.DestroyEntity(entity);
                    continue;
                }

                //set the modified data back
                entityManager.SetComponentData(entity, bltc);
            }
        }
    }
}
