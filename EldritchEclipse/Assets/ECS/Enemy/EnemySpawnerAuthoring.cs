using UnityEngine;
using Unity.Entities;
public class EnemySpawnerAuthoring : MonoBehaviour
{
    public GameObject EnemyPrefabToSpawn;

    public int NumOfEnemiesToSpawnPerSecond = 50;
    public int NumOfEnemiesToSpawnIncrementAmount = 2;
    public int MaxNumberOfEnemiesToSpawnPerSecond = 200;

    public float EnemySpawnRadius = 40f;
    public float MinDistanceFromPlayer = 5f;
    public float TimeBeforeNextSpawn = 2f;

    public class EnemySpawnBaker : Baker<EnemySpawnerAuthoring>
    {
        public override void Bake(EnemySpawnerAuthoring authoring)
        {
            //grab an entity
            Entity enemySpawnerEntity = GetEntity(TransformUsageFlags.None);

            //adds component
            AddComponent(enemySpawnerEntity, new EnemySpawnerComponent
            {
                EnemyPrefabToSpawn = GetEntity(authoring.EnemyPrefabToSpawn,TransformUsageFlags.None),
                NumOfEnemiesToSpawnPerSecond = authoring.NumOfEnemiesToSpawnPerSecond,
                NumOfEnemiesToSpawnIncrementAmount = authoring.NumOfEnemiesToSpawnIncrementAmount,
                MaxNumberOfEnemiesToSpawnPerSecond = authoring.MaxNumberOfEnemiesToSpawnPerSecond,
                EnemySpawnRadius = authoring.EnemySpawnRadius,
                MinDistanceFromPlayer = authoring.MinDistanceFromPlayer,
                TimeBeforeNextSpawn = authoring.TimeBeforeNextSpawn,
                CurrentTimeBeforeNextSpawn = 0f
            });
        }
    }
}
