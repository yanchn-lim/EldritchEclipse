using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Weapon : ScriptableObject
{
    public int BulletsPerShot;
    public float FireRate;
    public float FireRateMultiplier;
    public int MaxAmmoCount;
    public float ReloadSpeed;
    public float ReloadSpeedMultiplier;
    public float BaseDamage;
    public float DamageMultiplier;
    public GameObject BulletPrefab;
}

