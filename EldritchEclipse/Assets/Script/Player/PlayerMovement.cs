using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public float MoveSpeed;
    Vector3 inputDir;
    Camera cam;
    Transform camT;
    Vector3 dirToMouse;
    [SerializeField]
    GameObject bulletPrefab;
    Vector3 mousePos;
    [SerializeField]
    LayerMask rayMask;

    private void Start()
    {
        cam = Camera.main;
        camT = cam.transform;
    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 hori = new(x, 0, x);
        Vector3 vert = new(-z, 0, z);
        inputDir = hori + vert;     
        inputDir.Normalize();

        //rotate the player towards the mouse
        Ray mouseRay = cam.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(mouseRay,out RaycastHit hit,rayMask))
        {
            dirToMouse = hit.point - transform.position;
            dirToMouse.Normalize();

        }

        Vector3 forward = new(dirToMouse.x, 0, dirToMouse.z);
        transform.forward = forward;

        if (Input.GetMouseButtonDown(0))
        {
            var bul = Instantiate(bulletPrefab,transform.position,Quaternion.identity);
            bul.transform.up = forward;
        }
    }

    private void FixedUpdate()
    {
        transform.position += inputDir * Time.fixedDeltaTime * MoveSpeed;

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawCube(mousePos,new(1,1,1));
    }
}
