using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_AniHandler : MonoBehaviour
{   
    UI_Animation[] _animations;

    void PlayAnimations()
    {
        foreach (var ani in _animations)
        {
            StartCoroutine(ani.PlayAnimation());
        }
    }
}
