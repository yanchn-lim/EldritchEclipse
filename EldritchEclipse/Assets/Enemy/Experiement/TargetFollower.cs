using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetFollower : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float stoppingValue = 1.0f;
    [SerializeField] private float walkingSpeed = 1.0f;
    [SerializeField] private float rotationSpeed = 1.0f;

    private void Update()
    {
        if (Vector3.Distance(transform.position, target.position) > stoppingValue)
        {
            var direction = (target.position - transform.position).normalized;
            transform.position += direction * Time.deltaTime * walkingSpeed;
            Debug.DrawRay(transform.position,direction * 5, Color.yellow);

            var targetRotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation,targetRotation, Time.deltaTime * rotationSpeed);
        }
    }
}
