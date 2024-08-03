using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    Transform _parent;

    [SerializeField]
    GameObject[] _enemyPrefabs;
    Transform _player;
    Vector3[] bounds;

    
    private void Awake()
    {
        Initialize();
    }

    public void Initialize()
    {
        _player = GameObject.Find("Player").transform;
    }

    private void Update()
    {
        GetSpawnArea();
    }

    void GetSpawnArea()
    {
        ProjectBoundsToGround(Camera.main, out bounds);
    }

    #region Project Camera To Ground
    void GetCameraBounds(Camera cam, out Vector3 topLeft, out Vector3 topRight, out Vector3 bottomLeft, out Vector3 bottomRight)
    {
        float vertExtent = cam.orthographicSize;
        float horzExtent = vertExtent * cam.aspect;

        topLeft = cam.transform.position + cam.transform.rotation * new Vector3(-horzExtent, vertExtent, 0);
        topRight = cam.transform.position + cam.transform.rotation * new Vector3(horzExtent, vertExtent, 0);
        bottomLeft = cam.transform.position + cam.transform.rotation * new Vector3(-horzExtent, -vertExtent, 0);
        bottomRight = cam.transform.position + cam.transform.rotation * new Vector3(horzExtent, -vertExtent, 0);
    }

    void ProjectBoundsToGround(Camera cam, out Vector3[] groundBounds)
    {
        Vector3 topLeft, topRight, bottomLeft, bottomRight;
        GetCameraBounds(cam, out topLeft, out topRight, out bottomLeft, out bottomRight);

        Plane groundPlane = new Plane(Vector3.up, Vector3.zero); // Assuming ground plane is at y = 0
        groundBounds = new Vector3[4];
        Ray ray = new Ray();

        ray.origin = topLeft;
        ray.direction = cam.transform.forward;
        groundPlane.Raycast(ray, out float enter);
        groundBounds[0] = ray.GetPoint(enter);

        ray.origin = topRight;
        groundPlane.Raycast(ray, out enter);
        groundBounds[1] = ray.GetPoint(enter);

        ray.origin = bottomRight;
        groundPlane.Raycast(ray, out enter);
        groundBounds[2] = ray.GetPoint(enter);

        ray.origin = bottomLeft;
        groundPlane.Raycast(ray, out enter);
        groundBounds[3] = ray.GetPoint(enter);
    }
    #endregion

    void SpawnEnemy()
    {

    }

    private void OnDrawGizmos()
    {
        ProjectBoundsToGround(Camera.main, out bounds);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(bounds[0], bounds[1]);
        Gizmos.DrawLine(bounds[1], bounds[2]);
        Gizmos.DrawLine(bounds[2], bounds[3]);
        Gizmos.DrawLine(bounds[3], bounds[0]);
    }
}
