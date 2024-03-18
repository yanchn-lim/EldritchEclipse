using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public float MoveSpeed;
    Vector3 inputDir;
    Camera cam;
    Vector3 dirToMouse;
    [SerializeField]
    GameObject bulletPrefab;

    private void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        inputDir = new(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        inputDir.Normalize();

        //rotate the player towards the mouse
        Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        dirToMouse = mousePos - transform.position;
        dirToMouse.Normalize();
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


}
