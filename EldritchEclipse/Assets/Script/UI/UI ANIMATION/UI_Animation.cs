using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EasingFunctions;
using System.Threading.Tasks;

[System.Serializable]
public abstract class UI_Animation
{
    public float Time;
    public float Speed;
    public EasingFunction EasingFunc;

    //public abstract void Update();

    public virtual async Task Play() 
    {
        
    }
}

