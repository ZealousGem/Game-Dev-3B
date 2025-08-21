using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.XR;

public class Enemy : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public float Health;

    public float Speed;

    public float Damage = 10f;

    public float TowerDamage = 20f;

    public int Money = 10;


    [HideInInspector]

    public EnemyStates enemyStates;

     [HideInInspector]

    public List<GameObject> Towers = new List<GameObject>();

    MoveState moveState = new MoveState();

    AttackTowerState TowerState = new AttackTowerState();

    AttackState AttackState = new AttackState();

    DeathState Death = new DeathState();

    void Awake()
    {
        EventBus.Subscribe<DamageObjectEvent>(getDamage);
        EventBus.Subscribe<ChangeStateEvent>(ChangeState);
    }

    void OnDisable()
    {
        EventBus.Unsubscribe<DamageObjectEvent>(getDamage);
        EventBus.Unsubscribe<ChangeStateEvent>(ChangeState);
    }

    void ChangeState(ChangeStateEvent data)
    {
        if (data.name == gameObject.GetInstanceID())
        {
            MoveState(data.obj);
        }
    }

    void MoveState(List<GameObject> noTargets)
    {
         Towers = noTargets;
         enemyStates.ChangeState(this, moveState);
         enemyStates.EnterState(this);
    }

    void getDamage(DamageObjectEvent data)
    {
        if (data.name == gameObject.GetInstanceID())
        {
            DecreaseHealth(data.Damage);  
        }
    }


    void Start()
    {
        enemyStates = moveState;
        enemyStates.EnterState(this);    
       
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Tower"))
        {
            enemyStates.ChangeState(this, TowerState);
            enemyStates.EnterState(this);
        }

        else if (other.CompareTag("DefenceTower"))
        {
            enemyStates.ChangeState(this, AttackState);
            enemyStates.EnterState(this);
            Towers.Add(other.gameObject);
        }
    }


    void DecreaseHealth(float dam)
    {
        if (Health > 0)
        {
            Health -= dam;
            if (Health <= 0)
            {
            enemyStates.ChangeState(this, Death);
            enemyStates.EnterState(this);
            }
        }

      
       
    }

    public void KillEnemy()
    {
        float money = Money;
        GameManagerEvent giveMoney = new GameManagerEvent(money, StatsChange.MonenyGained);
        EventBus.Act(giveMoney);
        Destroy(this.gameObject);
       // Debug.Log("death"); 
    }

    

    // Update is called once per frame
}
