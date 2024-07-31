using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameVariables
{
    public static float TimeTick { get { return 0.02f; } }
    public static float GameTime { get { return 1200f; } }
    public static float ElapsedTime = 0;
    public static float RemainingTime = 0;
    public static float MaxEntitySpeed = 50f;
    public static float MaxEnemyHP = 999f;
}

public enum CalculationType
{
    FLAT,
    PERCENTAGE_MAX,
    PERCENTAGE_CURRENT,
}
