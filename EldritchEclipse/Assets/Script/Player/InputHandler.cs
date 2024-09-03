using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InputHandler
{ 
    static Camera mainCam = Camera.main;
    public static Vector2 MovementInput { get; private set;}
    public static Vector3 MovementAdjusted { get
        {
            Vector3 dir = new(
                MovementInput.x - MovementInput.y,
                0,
                MovementInput.x + MovementInput.y);

            return dir.normalized;
        }
    }
    public static Ray MouseScreenToWorldRay { get; private set; }
    public static bool FirePressed { get; private set; }
    public static bool FireHeld { get; private set; }

    public static void Update()
    {
        MovementInput = new(
            Input.GetAxis("Horizontal"),
            Input.GetAxis("Vertical")
            );

        MouseScreenToWorldRay = mainCam.ScreenPointToRay(Input.mousePosition);

        

        FirePressed = Input.GetMouseButtonDown(0) && !GameState.IsPlayerControlSuspended;
        FireHeld = Input.GetMouseButton(0);
    }
}
