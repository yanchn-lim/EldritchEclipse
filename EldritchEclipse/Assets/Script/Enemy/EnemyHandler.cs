using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHandler : MonoBehaviour
{
    [SerializeField]
    EnemyStat_SO stat_SO; //assigned

    Enemy_AI ai;
    EnemyStat stat = new();
    Enemy_Hitbox hitbox;
    Transform player;
    [SerializeField]
    Transform worldCanvas;

    #region MONOBEHAVIOUR
    private void Awake()
    {
        Initialize(); //change to be handled by SLS?
    }

    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        ai.MoveTowardsPlayer(stat.Speed);
    }
    #endregion

    #region Initialization
    void Initialize()
    {
        GetReference();
        InitializeComponents();
    }

    void GetReference()
    {
        ai = GetComponent<Enemy_AI>();
        hitbox = GetComponentInChildren<Enemy_Hitbox>();

        //player
        player = GameObject.Find("Player").transform;
    }

    void InitializeComponents()
    {
        stat.Initialize(stat_SO);
        ai.Initialize(player);
        hitbox.Initialize(this);
    }
    #endregion

    #region Taking Damage
    public void GetHit(float dmg)
    {
        //play hit reaction
        stat.TakeDamage(CalculationType.FLAT,dmg);
        var popup = Instantiate(GameAssets.dmgPopUp, worldCanvas);
        popup.GetComponent<DamagePopUpAnimation>().BeginAnimation(dmg);
        if (stat.IsDead)
        {
            Die();
        }
    }

    void Die()
    {
        var xp = Instantiate(GameAssets.xporb, transform.position, Quaternion.identity);
        xp.GetComponent<XpOrb>().ChangeValue(stat.XP);
        Destroy(gameObject);
    }
    #endregion
}
