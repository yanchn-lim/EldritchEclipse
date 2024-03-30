using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float MoveSpeed;

    Vector3 dirToMouse;
    [SerializeField]
    GameObject bulletPrefab;
    Vector3 mousePos;
    [SerializeField]
    LayerMask rayMask;
    InputHandler input;

    Ray mouseRay;
    private void Start()
    {
        input = InputHandler.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        //rotate the player towards the mouse
        mouseRay = input.MouseScreenToWorldRay;
        if(Physics.Raycast(mouseRay,out RaycastHit hit,rayMask))
        {
            dirToMouse = hit.point - transform.position;
            dirToMouse.Normalize();

        }

        Vector3 forward = new(dirToMouse.x, 0, dirToMouse.z);
        transform.forward = forward;

        if (input.FirePressed)
        {
            var bul = Instantiate(bulletPrefab,transform.position,Quaternion.identity);
            bul.transform.up = forward;
        }
    }

    private void FixedUpdate()
    {
        Move();
    }

    public void Move()
    {
        //switch this to proper movement
        transform.position += MoveSpeed * Time.fixedDeltaTime * input.MovementAdjusted;

    }

}
