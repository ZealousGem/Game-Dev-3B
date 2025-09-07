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

    // enemy stats 
    public float Health;

    public float Speed;

    public float Damage = 10f;

    public float TowerDamage = 20f;

    public int Money = 10;
    // enemy stats 

    [HideInInspector]

    public EnemyStates enemyStates; // current state enemy is using 

     [HideInInspector]

    public List<GameObject> Towers = new List<GameObject>(); // defence towers that will be located in its radius 

    public Image HealthUI; // ui to show how much health the enemy has left 

    public GameObject HealthCanvas;

    public GameObject Explosion; // explosion effect once enemy is dead 

    float maxHealth = 0;


    // list of states enemy can use

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

    void setStopState() // stop state happenes once game is over making enemy stop
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

    void OnTriggerEnter(Collider other) // if tower is located enemy will change to attackstate and add the tower to it's list so it can go to the other once it has killed the fromer 
    {
        if (other.CompareTag("Tower"))
        {
            enemyStates.ChangeState(this, TowerState); 
            enemyStates.EnterState(this); // enemy will change state to attack main tower state if radius is close to main tower
        }

        else if (other.CompareTag("DefenceTower")) // if tower is located enemy will change to attackstate and add the tower to it's list so it can go to the other once it has killed the fromer
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


    void DecreaseHealth(float dam) // decreases enemy health
    {
        if (Health > 0)
        {
            
            Health -= dam;
            StartCoroutine(EnemyUI());
            if (Health <= 0)
            {
                enemyStates.ChangeState(this, Death);
                enemyStates.EnterState(this);
            }
        }

      
       
    }
    
    IEnumerator EnemyUI() // HealthUI changes if enemies health has decreased 
    {
        HealthCanvas.SetActive(true);
        HealthUI.fillAmount = Health / maxHealth;
        yield return new WaitForSeconds(1f);
        HealthCanvas.SetActive(false);
    }

    public void KillEnemy() // destorys the enemy and gives the player money so they can use it to buy turrets 
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
