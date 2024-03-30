using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
public class InputHandler
{
    private static InputHandler instance;

    public static InputHandler Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new();
            }
            return instance;
        }
    }

    public InputHandler()
    {
        mainCam = Camera.main;
    }

    Camera mainCam;
    public Vector2 MovementInput { get; private set;}
    public Vector3 MovementAdjusted { get
        {
            Vector3 dir = new(
                MovementInput.x - MovementInput.y,
                0,
                MovementInput.x + MovementInput.y);

            return dir.normalized;
        }
    }
    public Ray MouseScreenToWorldRay { get; private set; }
    public bool FirePressed { get; private set; }
    public bool FireHeld { get; private set; }

    public void Update()
    {
        MovementInput = new(
            Input.GetAxis("Horizontal"),
            Input.GetAxis("Vertical")
            );

        MouseScreenToWorldRay = mainCam.ScreenPointToRay(Input.mousePosition);

        FirePressed = Input.GetMouseButtonDown(0);
        FireHeld = Input.GetMouseButton(0);
    }
}
