using JetBrains.Annotations;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public float Health;

    public float Speed;

    public float Damage;

     [HideInInspector]
    public Pathing moving;

    EnemyStates enemyStates;

    MoveState moveState = new MoveState();

    // AttackState attackState;

    // AttackTowerState towerState;


    void Start()
    {
        enemyStates = moveState;
        enemyStates.EnterState(this);    
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
