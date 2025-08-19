using JetBrains.Annotations;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public float Health;

    public float Speed;

    public float Damage;

    public float TowerDamage = 20f;


  [HideInInspector]

    public EnemyStates enemyStates;

    MoveState moveState = new MoveState();

    AttackTowerState TowerState = new AttackTowerState();

    DeathState Death = new DeathState();

    void OnEnable()
    {
        EventBus.Subscribe<DamageObjectEvent>(getDamage);
    }

    void OnDisable()
    {
          EventBus.Unsubscribe<DamageObjectEvent>(getDamage);
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
        Destroy(this.gameObject);
        Debug.Log("death"); 
    }

    void OnTriggerExit(Collider other)
    {
        
    }

    // Update is called once per frame
}
