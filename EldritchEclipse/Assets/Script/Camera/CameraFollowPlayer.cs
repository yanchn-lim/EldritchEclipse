using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    [SerializeField]
    Transform player;
    [SerializeField]
    float height,distance;
    private void Update()
    {
        //Vector3 dirToPlayer = transform.position - player.position;
        //dirToPlayer.Normalize();
        //transform.forward = dirToPlayer;

        transform.position = player.position + (Vector3.up * height) + (-transform.forward * distance);
    }
}
