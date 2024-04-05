using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class PlayerCombatHandler : MonoBehaviour
{
    [SerializeField]
    Weapon w;
    float _delayBetweenShots;
    int _bulletPerShot;
    GameObject _bulletPrefab;
    int _ammoCount;
    int _maxAmmoCount;
    float delayBeforeReload;
    float _reloadSpeed;

    public Slider reloadBar;

    //perma
    FSM _fsm;
    PlayerCombatState_Idle _idleState;
    PlayerCombatState_Shooting _shootState;
    PlayerCombatState_Reload _reloadState;

    public TMP_Text debugtxt;
    
    //sequence
    //shoot => no ammo => no shoot until reloaded
    //shoot => delay(still got ammo) => auto-reload => can cancel anytime by shooting

    private void Start()
    {
        Initialize();
    }

    void Initialize()
    {
        _bulletPerShot = w.BulletsPerShot;
        _delayBetweenShots = w.FireRate * w.FireRateMultiplier;
        _maxAmmoCount = w.MaxAmmoCount;
        _reloadSpeed = w.ReloadSpeed * w.ReloadSpeedMultiplier;
        _ammoCount = _maxAmmoCount;
        _bulletPrefab = w.BulletPrefab;

        _fsm = new();
        _idleState = new((int)CombatStates.IDLE, _fsm, this, delayBeforeReload);
        _shootState = new((int)CombatStates.SHOOTING, _fsm, this);
        _reloadState = new((int)CombatStates.RELOAD, _fsm, this);
        _fsm.AddState(_idleState, _shootState, _reloadState);
        _fsm.SwitchState(_idleState);

        ToggleReloadBar();
    }

    private void Update()
    {
        _fsm.Update();
        debugtxt.text = $"{_fsm._currentState.ToString()}";
    }

    public void Reload()
    {
        _ammoCount = _maxAmmoCount;
    }

    public void ToggleReloadBar()
    {
        reloadBar.gameObject.SetActive(!reloadBar.gameObject.active);
    }

    public void UpdateReloadBar(float val)
    {
        reloadBar.value = val;
    }

    public void SpawnBullet()
    {
        Vector3 forward = transform.forward;
        var bullet = Instantiate(_bulletPrefab, transform.position, Quaternion.identity);
        bullet.transform.up = forward;
        _ammoCount--;
    }

    #region GETTER
    public float AmmoPercentage { get { return ((float)_ammoCount / (float)_maxAmmoCount); } }
    public float DelayBetweenShots { get { return _delayBetweenShots; } }
    public int AmmoCount { get { return _ammoCount; } set { _ammoCount = value; } }
    public float ReloadSpeed { get { return _reloadSpeed; } }
    #endregion

    
}
