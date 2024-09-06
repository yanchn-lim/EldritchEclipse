using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using Unity.Transforms;

public partial struct BulletSpawnerSystem : ISystem,ISystemStartStop
{
    public void OnStartRunning(ref SystemState state)
    {
        return;
        Debug.Log("bullet spawner run");
        EntityManager em = state.EntityManager;
        Entity bulletSpawner = SystemAPI.GetSingletonEntity<BulletSpawnerComponent>();
        BulletSpawnerComponent bulletSpawnerComponent = em.GetComponentData<BulletSpawnerComponent>(bulletSpawner);

        for (int i = 0; i < bulletSpawnerComponent.BulletToPool; i++)
        {
            EntityCommandBuffer ECB = new(Allocator.Temp);
            Entity bullet = em.Instantiate(bulletSpawnerComponent.BulletToSpawn);

            ECB.AddComponent(bullet, new BulletComponent()
            {
                Speed = 25f,
                Size = 0.25f,
                Damage = 1f,
            });

            ECB.AddComponent(bullet, new BulletLifeTimeComponent()
            {
                RemainingLifeTime = 1.5f,
                DefaultLifeTime = 1.5f,
            });

            ECB.AddComponent(bullet, new BulletActive());
            

            LocalTransform bulletTransform = em.GetComponentData<LocalTransform>(bullet);
            bulletTransform.Position = new(0, -5, 0);

            ECB.SetComponent(bullet, bulletTransform);

            ECB.Playback(em);
            ECB.Dispose();
        }

        foreach (Entity e in em.GetAllEntities())
        {
            if (em.HasComponent(e, typeof(BulletActive)))
            {
                em.SetComponentEnabled<BulletActive>(e,false);
            }
        }

    }

    public void OnStopRunning(ref SystemState state)
    {
        
        //stop
    }
}

public struct BulletActive : IComponentData,IEnableableComponent
{

}