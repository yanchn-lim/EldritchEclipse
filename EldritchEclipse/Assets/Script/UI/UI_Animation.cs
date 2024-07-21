using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EasingFunctions;
[System.Serializable]
public abstract class UI_Animation
{
    public float Time;
    public float Speed;
    
    public abstract IEnumerator PlayAnimation();
    
}
