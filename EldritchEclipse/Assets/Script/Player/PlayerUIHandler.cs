using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIHandler : MonoBehaviour
{
    public Slider xpBar;
    public GameObject levelUpPanel;
    private void Awake()
    {
        xpBar = GameObject.Find("XP_Bar").GetComponent<Slider>();
        levelUpPanel = GameObject.Find("LevelUp_Panel");

        //hide some panels 
        levelUpPanel.SetActive(false);
    }

    public void InitializeXPBar(float xptonext)
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

    public void OpenLevelUpPanel()
    {
        levelUpPanel.SetActive(true);
    }
}
