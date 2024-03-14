using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    [SerializeField]
    Transform player;
    [SerializeField]
    float height;
    private void Update()
    {
        transform.position = player.position + (Vector3.up * height);
    }
}
