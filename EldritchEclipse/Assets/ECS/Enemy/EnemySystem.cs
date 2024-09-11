using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Burst;
using Unity.Collections;

[BurstCompile,DisableAutoCreation]
public partial struct EnemySystem : ISystem
{
    EntityManager _entityManager;
    Entity _playerEntity;

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        _entityManager = state.EntityManager;
        _playerEntity = SystemAPI.GetSingletonEntity<PlayerComponent>();
        LocalTransform playerTransform = _entityManager.GetComponentData<LocalTransform>(_playerEntity);

        NativeArray<Entity> allEntities = _entityManager.GetAllEntities();

        foreach (var entity in allEntities)
        {
            if(_entityManager.HasComponent<EnemyComponent>(entity))
            {
                LocalTransform enemyTransform = _entityManager.GetComponentData<LocalTransform>(entity);
                EnemyComponent enemyComponent = _entityManager.GetComponentData<EnemyComponent>(entity);

                float3 dir = math.normalize(playerTransform.Position - enemyTransform.Position);
                enemyTransform.Position += dir * enemyComponent.MoveSpeed * SystemAPI.Time.DeltaTime;

                //look at player
                float3 lookDir = math.normalize(playerTransform.Position - enemyTransform.Position);
                float angle = math.atan2(lookDir.x, lookDir.z);
                quaternion lookRot = quaternion.AxisAngle(new float3(0f, 1f, 0f), angle);
                enemyTransform.Rotation = lookRot;

                _entityManager.SetComponentData(entity, enemyTransform);
            }
        }
    }
}
