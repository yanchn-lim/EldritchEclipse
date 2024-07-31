using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class EnemyStat_SO : ScriptableObject
{
    public float HP;
    public float Damage;
    public float Speed;

    public abstract void Test();
}
