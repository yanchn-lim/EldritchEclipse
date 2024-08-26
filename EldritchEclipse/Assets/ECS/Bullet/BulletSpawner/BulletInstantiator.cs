using UnityEngine;
using Unity.Entities;

public class BulletSpawnerAuthoring : MonoBehaviour
{
    public int BulletToPool;
    public GameObject bulletPrefab;

    public class BulletSpawnerBaker : Baker<BulletSpawnerAuthoring>
    {
        public override void Bake(BulletSpawnerAuthoring authoring)
        {
            //grab an entity
            Entity bulletSpawnerEntity = GetEntity(TransformUsageFlags.None);

            //adds component
            AddComponent(bulletSpawnerEntity, new BulletSpawnerComponent
            {
                BulletToSpawn = GetEntity(authoring.bulletPrefab, TransformUsageFlags.None),
                BulletToPool = authoring.BulletToPool
            });
        }
    }
}

public struct BulletSpawnerComponent : IComponentData
{
    public Entity BulletToSpawn;
    public int BulletToPool;
}

