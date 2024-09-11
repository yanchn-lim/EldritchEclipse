using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using UnityEngine;
using Unity.Burst;
using Unity.Physics;

[DisableAutoCreation]
public partial struct PlayerSystem : ISystem
{
    EntityManager _entityManager;

    Entity _playerEntity;
    Entity _inputEntity;

    PlayerComponent _playerComponent;
    InputComponent _inputComponent;
    EntityQuery query;
    public void OnCreate(ref SystemState state)
    {
        query = new EntityQueryBuilder(Allocator.Temp)
            .WithAllRW<LocalTransform>()
            .WithAllRW<BulletComponent>()
            .WithAllRW<BulletLifeTimeComponent>()
            .WithDisabled<BulletActive>()
            .Build(ref state);
    }

    //updates state every frame
    public void OnUpdate(ref SystemState state)
    {
        _entityManager = state.EntityManager;
        _playerEntity = SystemAPI.GetSingletonEntity<PlayerComponent>();
        _inputEntity = SystemAPI.GetSingletonEntity<InputComponent>();

        _playerComponent = _entityManager.GetComponentData<PlayerComponent>(_playerEntity);
        _inputComponent = _entityManager.GetComponentData<InputComponent>(_inputEntity);

        Move(ref state);
        //Shoot(ref state);
        Shoot_Pool(ref state);
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

    [BurstCompile]
    void Shoot_Pool(ref SystemState state)
    {
        if (_inputComponent.Shoot)
        {
            var e = query.ToEntityArray(Allocator.Temp);

            int counter = 0;
            LocalTransform playerTransform = _entityManager.GetComponentData<LocalTransform>(_playerEntity);

            //get inactive bullets from query
            foreach (var b in e) 
            {
                EntityCommandBuffer ECB = new(Allocator.Temp);
                
                if (counter > _playerComponent.NumOfBulletsToSpawn)
                    break;
                counter++;


                LocalTransform lt = _entityManager.GetComponentData<LocalTransform>(b);
                BulletLifeTimeComponent bltc = _entityManager.GetComponentData<BulletLifeTimeComponent>(b);
                bltc.RemainingLifeTime = bltc.DefaultLifeTime;
                //align bullet to player's forward
                lt.Rotation = playerTransform.Rotation;

                float randomOffset = UnityEngine.Random.Range(-_playerComponent.BulletSpread, _playerComponent.BulletSpread);
                lt.Position = playerTransform.Position + playerTransform.Forward() * 1.25f + playerTransform.Right() * randomOffset;

                ECB.SetComponentEnabled<BulletActive>(b,true);
                ECB.SetComponent(b, lt);
                ECB.SetComponent(b, bltc);
                ECB.Playback(_entityManager);

                //release memory
                ECB.Dispose();
            }
        }
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

                //ECB.AddComponent(bulletEntity, new PhysicsCollider());
                //ECB.AddComponent(bulletEntity, new PhysicsVelocity());

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
