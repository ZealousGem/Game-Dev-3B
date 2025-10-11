using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.UI;
using Unity.Mathematics;

public class Enemy : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public float Health;

    public float Speed;

    public float Damage = 10f;

    public float TowerDamage = 20f;

    public int Money = 10;

    [HideInInspector]
    public int MoneyGiven = 1;

    [HideInInspector]

    public EnemyStates enemyStates;

     [HideInInspector]

    public List<GameObject> Towers = new List<GameObject>();

    public Image HealthUI;

    public GameObject HealthCanvas;

    public GameObject Explosion;

    float maxHealth = 0;

    MoveState moveState = new MoveState();

    AttackTowerState TowerState = new AttackTowerState();

    AttackState AttackState = new AttackState();

    DeathState Death = new DeathState();

    StopState Stop = new StopState();

    void Awake()
    {
        EventBus.Subscribe<DamageObjectEvent>(getDamage);
        EventBus.Subscribe<ChangeStateEvent>(ChangeState);
        EventBus.Subscribe<EndGameEvent>(getEndDate);
        maxHealth = Health;
        HealthCanvas.SetActive(false);
    }

    void OnDisable()
    {
        EventBus.Unsubscribe<DamageObjectEvent>(getDamage);
        EventBus.Unsubscribe<ChangeStateEvent>(ChangeState);
        EventBus.Unsubscribe<EndGameEvent>(getEndDate);
    }

    void getEndDate(EndGameEvent data)
    {
        if (data.type == StatsChange.EndGame)
        {
            setStopState();
        }
    }

    void setStopState()
    {
        Towers.RemoveAll(Towers.Contains);
        enemyStates.ChangeState(this, Stop);
        enemyStates.EnterState(this);
         
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
            StartCoroutine(SeesEnemy(other.gameObject));
        }
    }

    IEnumerator SeesEnemy(GameObject obj)
    {
        yield return new WaitForSeconds(0.5f);
        enemyStates.ChangeState(this, AttackState);
        enemyStates.EnterState(this);
        Towers.Add(obj);
    }


    void DecreaseHealth(float dam)
    {
        if (Health > 0)
        {

            if (dam > 4)
            {
                 GameManagerEvent giveMoney = new GameManagerEvent(MoneyGiven, StatsChange.MonenyGained);
                 EventBus.Act(giveMoney);
            }
            Health -= dam;
            StartCoroutine(EnemyUI());
            if (Health <= 0)
            {
                enemyStates.ChangeState(this, Death);
                enemyStates.EnterState(this);
            }
        }

      
       
    }
    
    IEnumerator EnemyUI()
    {
        HealthCanvas.SetActive(true);
        HealthUI.fillAmount = Health / maxHealth;
        yield return new WaitForSeconds(1f);
        HealthCanvas.SetActive(false);
    }

    public void KillEnemy()
    {
        float money = Money;
        GameManagerEvent giveMoney = new GameManagerEvent(money, StatsChange.MonenyGained);
        GameManagerEvent EnemyKilled = new GameManagerEvent(1, StatsChange.EnemyDead);
        EventBus.Act(giveMoney);
        EventBus.Act(EnemyKilled);
        Instantiate(Explosion, this.gameObject.transform.position, quaternion.identity);
        Destroy(this.gameObject);
       
        // Debug.Log("death"); 
    }

    

    // Update is called once per frame
}
