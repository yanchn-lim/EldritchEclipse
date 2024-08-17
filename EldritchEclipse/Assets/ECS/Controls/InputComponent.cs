using Unity.Entities;
using Unity.Mathematics;
public struct InputComponent : IComponentData
{
    public float2 Movement;
    public float2 MousePosition;
    public bool Shoot;
}
