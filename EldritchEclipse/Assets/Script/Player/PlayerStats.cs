 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    float HP;
    float maxHP;
    float baseMoveSpeed;
    float moveSpeed;
    float moveSpeedMultiplier = 1;

    float xp = 0;
    float xpToLevel = 100;

    [SerializeField]
    Stat stat;
    [SerializeField]
    PlayerUIHandler ui;

    EventManager em = EventManager.Instance;

    private void Awake()
    {
        em.AddListener<float>(Event.PLAYER_XP_GAIN,GainXP);
    }

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
    public void RecoverHealth(float hp)
    {
        HP = Mathf.Clamp(HP + hp, 0, maxHP);
    }

    public void ReduceHealth(float hp)
    {
        HP = Mathf.Clamp(HP - hp,0,maxHP);
    }

    public void IncreaseMaxHealth(float hp)
    {
        maxHP += hp;
    }

    public void DecreaseMaxHealth(float hp)
    {
        maxHP -= hp;
    }
    #endregion

    void IncreaseMoveSpeed(float percentage)
    {
        moveSpeedMultiplier += percentage;
        moveSpeed = baseMoveSpeed * moveSpeedMultiplier;
    }

    void ReduceMoveSpeed(float percentage)
    {
        moveSpeedMultiplier -= percentage;
        moveSpeed = baseMoveSpeed * moveSpeedMultiplier;
    }

    void GainXP(float gain)
    {
        xp += gain;
        //update UI;

        if (CheckIfLevelUp())
            LevelUp();

        ui.UpdateXP(xp);
    }

    bool CheckIfLevelUp()
    {
        return xp >= xpToLevel;
    }

    void LevelUp()
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
                                                        