using Unity.Entities;

public struct BulletLifeTimeComponent : IComponentData
{
    public float RemainingLifeTime;
    public float DefaultLifeTime;
}
