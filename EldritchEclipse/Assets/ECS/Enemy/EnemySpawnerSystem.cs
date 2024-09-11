using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Burst;

[BurstCompile,DisableAutoCreation]
public partial struct EnemySpawnerSystem : ISystem
{
    EntityManager _entityManager;
    Entity _enemySpawnerEntity;
    EnemySpawnerComponent _enemySpawnerComponent;
    Entity _playerEntity;
    Unity.Mathematics.Random _random;
    public void OnCreate(ref SystemState state)
    {
        _random = Unity.Mathematics.Random.CreateFromIndex((uint)_enemySpawnerComponent.GetHashCode());
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        _entityManager = state.EntityManager;

        _enemySpawnerEntity = SystemAPI.GetSingletonEntity<EnemySpawnerComponent>();
        _enemySpawnerComponent = _entityManager.GetComponentData<EnemySpawnerComponent>(_enemySpawnerEntity);

        _playerEntity = SystemAPI.GetSingletonEntity<PlayerComponent>();

        SpawnEnemies(ref state);
    }

    [BurstCompile]
    void SpawnEnemies(ref SystemState state)
    {
        _enemySpawnerComponent.CurrentTimeBeforeNextSpawn -= SystemAPI.Time.DeltaTime;

        if(_enemySpawnerComponent.CurrentTimeBeforeNextSpawn <= 0f)
        {
            for (int i = 0; i < _enemySpawnerComponent.NumOfEnemiesToSpawnPerSecond; i++)
            {
                EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);
                Entity enemyEntity = _entityManager.Instantiate(_enemySpawnerComponent.EnemyPrefabToSpawn);
                
                LocalTransform enemyTransform = _entityManager.GetComponentData<LocalTransform>(enemyEntity);
                LocalTransform playerTransform =  _entityManager.GetComponentData<LocalTransform>(_playerEntity);

                //spawn position
                float minDistSqr = _enemySpawnerComponent.MinDistanceFromPlayer * _enemySpawnerComponent.MinDistanceFromPlayer;
                float2 randomOffset = _random.NextFloat2Direction() 
                    * _random.NextFloat(_enemySpawnerComponent.MinDistanceFromPlayer, _enemySpawnerComponent.EnemySpawnRadius);
                float2 playerPosition = playerTransform.Position.xz;
                float2 spawnPosition = playerPosition + randomOffset;
                float distSqr = math.lengthsq(spawnPosition - playerPosition);

                if(distSqr < minDistSqr){
                    spawnPosition = playerPosition + math.normalize(randomOffset) * math.sqrt(minDistSqr);
                }
                enemyTransform.Position = new float3(spawnPosition.x, 0f, spawnPosition.y);

                //spawn look direction
                float3 dir = math.normalize(playerTransform.Position - enemyTransform.Position);
                float angle = math.atan2(dir.x, dir.z);
                quaternion lookRot = quaternion.AxisAngle(new float3(0f, 1f, 0f), angle);
                enemyTransform.Rotation = lookRot;

                ecb.SetComponent(enemyEntity, enemyTransform);
                ecb.AddComponent(enemyEntity, new EnemyComponent{
                    MoveSpeed = 2f,
                    CurrentHealth = 1f
                });

                ecb.Playback(_entityManager);
                ecb.Dispose();
            }

            //increment spawn rate
            int desiredEnemiesPerWave = _enemySpawnerComponent.NumOfEnemiesToSpawnPerSecond 
                + _enemySpawnerComponent.NumOfEnemiesToSpawnIncrementAmount;
            int enemiesPerWave = math.min(desiredEnemiesPerWave, _enemySpawnerComponent.MaxNumberOfEnemiesToSpawnPerSecond);
            _enemySpawnerComponent.NumOfEnemiesToSpawnPerSecond = enemiesPerWave;

            _enemySpawnerComponent.CurrentTimeBeforeNextSpawn = _enemySpawnerComponent.TimeBeforeNextSpawn;
        }

        _entityManager.SetComponentData(_enemySpawnerEntity, _enemySpawnerComponent);

    }
}
