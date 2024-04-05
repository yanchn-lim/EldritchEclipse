using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIHandler : MonoBehaviour
{
    [SerializeField]
    Slider xpBar;

    public void Initialize(float xptonext)
    {
        xpBar.maxValue = xptonext;
    }

    public void UpdateXP(float xp)
    {
        xpBar.value = xp;
    }

    public void UpdateXP(float xp, float xpToNext)
    {
        xpBar.maxValue = xpToNext;
        xpBar.value = xp;
    }
}
