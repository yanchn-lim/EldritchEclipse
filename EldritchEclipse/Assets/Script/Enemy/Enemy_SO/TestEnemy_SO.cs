using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TestEnemyStats", menuName = "Enemy Stats SO/TestEnemyStats")]
public class TestEnemyStat_SO : EnemyStat_SO
{
    public override void Test()
    {
        Debug.Log("testing method...");
    }
}
