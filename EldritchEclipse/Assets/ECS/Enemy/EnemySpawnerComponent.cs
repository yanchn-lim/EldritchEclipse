using Unity.Entities;
using Unity.Entities.UniversalDelegates;

public struct EnemySpawnerComponent : IComponentData
{
    public Entity EnemyPrefabToSpawn;

    public int NumOfEnemiesToSpawnPerSecond;
    public int NumOfEnemiesToSpawnIncrementAmount;
    public int MaxNumberOfEnemiesToSpawnPerSecond;
    
    public float EnemySpawnRadius;
    public float MinDistanceFromPlayer;
    public float TimeBeforeNextSpawn;
    public float CurrentTimeBeforeNextSpawn;
}
