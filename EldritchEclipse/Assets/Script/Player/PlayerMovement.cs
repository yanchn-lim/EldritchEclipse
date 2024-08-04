using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float MoveSpeed;

    Vector3 dirToMouse;
    [SerializeField]
    GameObject bulletPrefab;
    [SerializeField]
    LayerMask rayMask;

    Ray mouseRay;
    private void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //rotate the player towards the mouse
        mouseRay = InputHandler.MouseScreenToWorldRay;
        if(Physics.Raycast(mouseRay,out RaycastHit hit,999f,rayMask))
        {
            dirToMouse = hit.point - transform.position;
            dirToMouse.Normalize();
        }

        Vector3 forward = new(dirToMouse.x, 0, dirToMouse.z);
        transform.forward = forward;
    }

    private void FixedUpdate()
    {
        Move();
    }

    public void Move()
    {
        //switch this to proper movement
        transform.position += MoveSpeed * Time.fixedDeltaTime * InputHandler.MovementAdjusted;
        
    }

}
