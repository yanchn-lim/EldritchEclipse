using UnityEngine;
using Unity.Entities;

public class PlayerAuthoring : MonoBehaviour
{
    public float MoveSpeed = 5f;
    public GameObject BulletPrefab;
    public int NumOfBulletsToSpawn = 50;
    [Range(0f,10f)]
    public float BulletSpread = 5f;


    //set up a baker to bake monobehaviours into entities
    public class PlayerBaker : Baker<PlayerAuthoring>
    {
        public override void Bake(PlayerAuthoring authoring)
        {
            Entity playerEntity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(playerEntity,new PlayerComponent
            {
                MoveSpeed = authoring.MoveSpeed,
                BulletPrefab = GetEntity(authoring.BulletPrefab,TransformUsageFlags.Renderable),
                NumOfBulletsToSpawn = authoring.NumOfBulletsToSpawn,
                BulletSpread = authoring.BulletSpread
            });
        }
    }
}
