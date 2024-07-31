using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyStat
{
    /*
     * b = base
     * r = recover
     * i = increase
     * f = flat
     * p = percentage
     */

    float _hp, _maxhp, _bhp;
    float _spd, _bspd;
    float _dmg, _bdmg;

    public void Initialize(EnemyStat_SO so)
    {
        _bhp = _hp = _maxhp = so.HP;
        _spd = _bspd = so.Speed;
        _dmg = _bdmg = so.Damage;
    }

    public void TakeDamage(CalculationType type, float val)
    {
        float dmg;

        switch (type)
        {
            case CalculationType.FLAT:
                dmg = val;
                break;
            case CalculationType.PERCENTAGE_MAX:
                dmg = _maxhp * val;
                break;
            case CalculationType.PERCENTAGE_CURRENT:
                dmg = _hp * val;
                break;
            default:
                dmg = 0;
                Debug.LogWarning("Enemy_TakeDamage : Issues with getting the calculation type...");
                break;
        }

        _hp = Mathf.Clamp(_hp - dmg, 0, _maxhp);
    }

    public void RecoverHP(CalculationType type, float val)
    {
        float rhp;

        switch (type)
        {
            case CalculationType.FLAT:
                rhp = val;
                break;
            case CalculationType.PERCENTAGE_MAX:
                rhp = _maxhp * val;
                break;
            case CalculationType.PERCENTAGE_CURRENT:
                rhp = _hp * val;
                break;
            default:
                rhp = 0;
                Debug.LogWarning("Enemy_RecoverHP : Issues with getting the calculation type...");
                break;
        }

        _hp = Mathf.Clamp(_hp + rhp, 0, _maxhp);
    }

    public void IncreaseMaxHP(CalculationType type, float val)
    {
        float ihp;

        switch (type)
        {
            case CalculationType.FLAT:
                ihp = val;
                break;
            case CalculationType.PERCENTAGE_MAX:
                ihp = _maxhp * val;
                break;
            case CalculationType.PERCENTAGE_CURRENT:
                ihp = _hp * val; //weird case
                break;
            default:
                ihp = 0;
                Debug.LogWarning("Enemy_RecoverHP : Issues with getting the calculation type...");
                break;
        }
        _maxhp = Mathf.Clamp(_maxhp + ihp, 0, GameVariables.MaxEnemyHP);
    }

    #region Getter
    public float Speed => _spd;
    public bool IsDead => _hp <= 0;
    #endregion
}
