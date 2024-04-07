 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    int HP;
    int maxHP;
    float baseMoveSpeed;
    float moveSpeed;
    float moveSpeedMultiplier = 1;

    float xp = 0;
    float xpToLevel = 100;

    [SerializeField]
    Stat stat;
    [SerializeField]
    PlayerUIHandler ui;

    private void Start()
    {
        Initialize();
        ui.InitializeXPBar(xpToLevel); // change
    }

    void Initialize()
    {
        maxHP = stat.MaxHp;
        baseMoveSpeed = stat.MoveSpeed;
        moveSpeed = baseMoveSpeed * moveSpeedMultiplier;
        HP = maxHP;
    }

    #region Health
    public void RecoverHealth(int hp)
    {
        HP = Mathf.Clamp(HP + hp, 0, maxHP);
    }

    public void ReduceHealth(int hp)
    {
        HP = Mathf.Clamp(HP - hp,0,maxHP);
    }

    public void IncreaseMaxHealth(int hp)
    {
        maxHP += hp;
    }

    public void DecreaseMaxHealth(int hp)
    {
        maxHP -= hp;
    }
    #endregion

    public void IncreaseMoveSpeed(float percentage)
    {
        moveSpeedMultiplier += percentage;
        moveSpeed = baseMoveSpeed * moveSpeedMultiplier;
    }

    public void ReduceMoveSpeed(float percentage)
    {
        moveSpeedMultiplier -= percentage;
        moveSpeed = baseMoveSpeed * moveSpeedMultiplier;
    }

    public void GainXP(float gain)
    {
        xp += gain;
        //update UI;

        if (CheckIfLevelUp())
            LevelUp();

        ui.UpdateXP(xp);
    }

    public bool CheckIfLevelUp()
    {
        return xp >= xpToLevel;
    }

    public void LevelUp()
    {
        Debug.Log("LEVEL UP!");
        float temp = xp - xpToLevel;
        xp = temp;
        xpToLevel *= 1.3f;
        ui.UpdateXP(xp, xpToLevel);

        //do other level logic
        ui.OpenLevelUpPanel();
    }


}
                                                        