using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatHandler : MonoBehaviour
{
    InputHandler input;

    [SerializeField]
    float _delayBetweenShots;
    [SerializeField]
    int _bulletPerShot;
    [SerializeField]
    GameObject _bulletPrefab;

    float _fireRate;

    private void Start()
    {
        input = InputHandler.Instance;
    }

    private void Update()
    {
        if (input.FirePressed)
            StartCoroutine(Fire());
    }

    IEnumerator Fire()
    {
        SpawnBullet();
        yield return new WaitForSeconds(_delayBetweenShots);
        while (input.FireHeld)
        {
            SpawnBullet();
            yield return new WaitForSeconds(_delayBetweenShots);

        }
    }

    void SpawnBullet()
    {
        Vector3 forward = transform.forward;
        var bullet = Instantiate(_bulletPrefab,transform.position,Quaternion.identity);
        bullet.transform.up = forward;
    }
}
