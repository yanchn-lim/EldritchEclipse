using Unity.Entities;
using UnityEngine;
public partial class InputSystem : SystemBase
{
    ControlsECS _controls;

    protected override void OnCreate()
    {
        if(!SystemAPI.TryGetSingleton(out InputComponent inputComponent))
        {
            EntityManager.CreateEntity(typeof(InputComponent));
        }

        _controls = new();
        _controls.Enable();
    }

    protected override void OnUpdate()
    {
        Vector2 moveVector = _controls.Player.Move.ReadValue<Vector2>();
        Vector2 mousePosition = _controls.Player.Aim.ReadValue<Vector2>();
        bool shoot = _controls.Player.Shoot.IsPressed();
        
        SystemAPI.SetSingleton(new InputComponent
        {
            Movement = moveVector,
            MousePosition = mousePosition,
            Shoot = shoot
        });
    }
}
