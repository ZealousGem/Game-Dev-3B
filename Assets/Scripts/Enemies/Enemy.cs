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

 [HideInInspector]

  public MoveState moveState = new MoveState();

   [HideInInspector]

  public AttackTowerState TowerState = new AttackTowerState();

    // AttackState attackState;

    // AttackTowerState towerState;


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

    void OnTriggerExit(Collider other)
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
