using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameVariables
{
    public static float TimeTick { get { return 0.01f; } }
    public static float TimeTickMiliSec { get { return TimeTick * 1000; } }

    public static float GameTime { get { return 1200f; } }
    public static float ElapsedTime = 0;
    public static float RemainingTime = 0;
    public static float MaxEntitySpeed = 50f;
    public static float MaxEnemyHP = 999f;
}

public static class GameAssets
{
    private static GameObject RetrieveGameObject(string path)
    {
        try
        {
            return Resources.Load<GameObject>(path);
        }
        catch
        {
            Debug.Log("Problem retrieving...");
            return null;
        }
    } 

    public static GameObject xporb { get { return RetrieveGameObject("Prefabs/xp_orb"); } }

    public static GameObject dmgPopUp { get { return RetrieveGameObject("Prefabs/DamagePopUpText"); } }

}

public enum CalculationType
{
    FLAT,
    PERCENTAGE_MAX,
    PERCENTAGE_CURRENT,
}
