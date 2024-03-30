using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatHandler : MonoBehaviour
{
    InputHandler input;
    [SerializeField]
    GameManager gm;
    [SerializeField]
    float _delayBetweenShots;
    [SerializeField]
    int _bulletPerShot;
    [SerializeField]
    GameObject _bulletPrefab;

    float _fireRate;

    //temporary stats, to be moved a stat handler
    public int _ammoCount;
    [SerializeField]
    int _maxAmmoCount;
    [SerializeField]
    float _reloadSpeed;

    bool _reloadTrigger;

    private void Start()
    {
        input = InputHandler.Instance;
        _ammoCount = _maxAmmoCount;
    }

    private void Update()
    {
        if (input.FirePressed)
            StartCoroutine(Fire());

        if (_reloadTrigger)
        {
            StartCoroutine(Reload());
        }
    }

    IEnumerator Fire()
    {
        //SpawnBullet();
        while (input.FireHeld && CheckAmmo())
        {
            if (_reloadTrigger)
                yield break;
            SpawnBullet();

            yield return new WaitForSeconds(_delayBetweenShots);
            //SpawnBullet();

        }
    }

    IEnumerator Reload()
    {
        _reloadTrigger = false;

        float progress = 0;
        Debug.Log("reload start");
        while(progress < 100f)
        {
            yield return new WaitForSeconds(gm.timeTick);
            progress += _reloadSpeed * gm.timeTick;
        }
        Debug.Log("reload complete");
        _ammoCount = _maxAmmoCount;
    }

    bool SpawnBullet()
    {
        if (_reloadTrigger)
            return false;

        Vector3 forward = transform.forward;
        var bullet = Instantiate(_bulletPrefab,transform.position,Quaternion.identity);
        bullet.transform.up = forward;
        _ammoCount--;
        return true;
    }

    bool CheckAmmo()
    {
        bool check = _ammoCount > 0;
        _reloadTrigger = check ? false: true;
        return _ammoCount > 0;
    }
}
