using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using UnityEngine;
using Unity.Burst;

public partial struct PlayerSystem : ISystem
{
    EntityManager _entityManager;

    Entity _playerEntity;
    Entity _inputEntity;

    PlayerComponent _playerComponent;
    InputComponent _inputComponent;

    //updates state every frame
    public void OnUpdate(ref SystemState state)
    {
        _entityManager = state.EntityManager;
        _playerEntity = SystemAPI.GetSingletonEntity<PlayerComponent>();
        _inputEntity = SystemAPI.GetSingletonEntity<InputComponent>();

        _playerComponent = _entityManager.GetComponentData<PlayerComponent>(_playerEntity);
        _inputComponent = _entityManager.GetComponentData<InputComponent>(_inputEntity);

        Move(ref state);
        Shoot(ref state);
    }

    void Move(ref SystemState state)
    {
        //grabs transform from the entity
        LocalTransform playerTransform = _entityManager.GetComponentData<LocalTransform>(_playerEntity);
        float3 move = new(_inputComponent.Movement.x,0,_inputComponent.Movement.y);
        playerTransform.Position += new float3(move * _playerComponent.MoveSpeed * SystemAPI.Time.DeltaTime);

        //rotation calculations
        float2 dir = _inputComponent.MousePosition - new float2(Camera.main.WorldToScreenPoint(playerTransform.Position).x, Camera.main.WorldToScreenPoint(playerTransform.Position).y);
        float angle = math.degrees(math.atan2(dir.x, dir.y));
        playerTransform.Rotation = Quaternion.AngleAxis(angle,Vector3.up);

        //call this function to set the new data into the entity
        _entityManager.SetComponentData(_playerEntity,playerTransform);
    }

    void Shoot(ref SystemState state)
    {
        if(_inputComponent.Shoot)
        {
            for (int i = 0; i < _playerComponent.NumOfBulletsToSpawn; i++)
            {
                EntityCommandBuffer ECB = new(Allocator.Temp);
                
                //instantiating the bullet
                Entity bulletEntity = _entityManager.Instantiate(_playerComponent.BulletPrefab);

                //assign values to the instantiated bullet's variables
                ECB.AddComponent(bulletEntity,new BulletComponent(){
                    Speed = 25f,
                    Size = 0.25f,
                    Damage = 1f,
                });

                ECB.AddComponent(bulletEntity, new BulletLifeTimeComponent()
                {
                    RemainingLifeTime = 1.5f
                });

                //get transform of bullet and player
                LocalTransform bulletTransform = _entityManager.GetComponentData<LocalTransform>(bulletEntity);
                LocalTransform playerTransform = _entityManager.GetComponentData<LocalTransform>(_playerEntity);

                //align bullet to player's forward
                bulletTransform.Rotation = playerTransform.Rotation;

                float randomOffset = UnityEngine.Random.Range(-_playerComponent.BulletSpread,_playerComponent.BulletSpread);
                bulletTransform.Position = playerTransform.Position + playerTransform.Forward() * 1.25f + playerTransform.Right() * randomOffset;

                //set the modified bullet transform data back to the entity
                ECB.SetComponent(bulletEntity, bulletTransform);
                //apply the changes to the bullet in the entity manager
                ECB.Playback(_entityManager);

                //release memory
                ECB.Dispose();
            }
        }
    }
}
