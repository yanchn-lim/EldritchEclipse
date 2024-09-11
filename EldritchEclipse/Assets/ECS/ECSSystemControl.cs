using UnityEngine;
using Unity.Entities;

public class ECSSystemControl : MonoBehaviour
{
    World GameWorld;

    void Start(){
        GameWorld = new World("GameWorld",WorldFlags.Game);
        // var playerSystem = GameWorld.GetOrCreateSystem<PlayerSystem>();

        // var enemySystem = GameWorld.GetOrCreateSystem<EnemySystem>();
        // var enemySpawnerSystem = GameWorld.GetOrCreateSystem<EnemySpawnerSystem>();

        // var bulletSystem = GameWorld.GetOrCreateSystem<BulletSystem>();
        // var bulletMovementSystem = GameWorld.GetOrCreateSystem<BulletMovementSystem>();
        // var bulletSpawnerSystem = GameWorld.GetOrCreateSystem<BulletSpawnerSystem>();
        // var bulletCollisionSystem = GameWorld.GetOrCreateSystem<BulletCollisionSystem>();

    
    }

    void Update()
    {
        //GameWorld.Update();
        var playerSystem = GameWorld.GetOrCreateSystem<PlayerSystem>();

        var enemySystem = GameWorld.GetOrCreateSystem<EnemySystem>();
        var enemySpawnerSystem = GameWorld.GetOrCreateSystem<EnemySpawnerSystem>();

        var bulletSystem = GameWorld.GetOrCreateSystem<BulletSystem>();
        var bulletMovementSystem = GameWorld.GetOrCreateSystem<BulletMovementSystem>();
        var bulletSpawnerSystem = GameWorld.GetOrCreateSystem<BulletSpawnerSystem>();
        var bulletCollisionSystem = GameWorld.GetOrCreateSystem<BulletCollisionSystem>();
    }
}
