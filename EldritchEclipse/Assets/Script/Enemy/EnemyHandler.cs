using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHandler : MonoBehaviour
{
    [SerializeField]
    EnemyStat_SO stat_SO; //assigned
    [SerializeField]
    GameObject xpOrb;


    public Enemy_AI ai;
    public EnemyStat stat = new();
    public Enemy_Hitbox hitbox;
    public Transform player;

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

    public void GetHit()
    {
        //play hit reaction
        stat.TakeDamage(CalculationType.FLAT,1);

        if (stat.IsDead)
        {
            Instantiate(xpOrb,transform.position,Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
