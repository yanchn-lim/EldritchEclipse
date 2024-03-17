using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public float MoveSpeed;
    Vector3 inputDir;
    // Update is called once per frame
    void Update()
    {
        inputDir = new(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        inputDir.Normalize();

    }

    private void FixedUpdate()
    {
        transform.position += inputDir * Time.fixedDeltaTime * MoveSpeed;

    }
}
