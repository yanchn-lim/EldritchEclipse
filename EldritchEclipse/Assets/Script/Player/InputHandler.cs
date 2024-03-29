using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
public class InputHandler : MonoBehaviour
{
    public static Vector2 MovementInput { get; private set;}
    UnityEvent test;

    
    void Update()
    {
        MovementInput = new(
            Input.GetAxis("Horizontal"),
            Input.GetAxis("Vertical")
            );
    }
}
