using UnityEngine;

public abstract class EnemyStates

{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public abstract void EnterState(Enemy change);

    public abstract void ChangeState(Enemy change, EnemyStates state);
}

// Enemy States used to easily transtion between enemy scripts to make orngaition better , read enemy script to see state functions 

public class MoveState : EnemyStates
{

    public override void ChangeState(Enemy change, EnemyStates state)
    {
        
        if (change.GetComponent<Pathing>())
        {
            Pathing path = change.GetComponent<Pathing>();
            path.StopMoving();
            change.enemyStates = state;
        }

    }

    public override void EnterState(Enemy change)
    {
      

        if (change.GetComponent<Pathing>())
        {
            Pathing path = change.GetComponent<Pathing>();
            LandGenerator land = GameObject.FindGameObjectWithTag("island").GetComponent<LandGenerator>();

            path.intPathing(land.getPointState(), land.getMiddle(), change.transform.position, change.Speed);


        }

    }

}

public class AttackState : EnemyStates
{
    public override void ChangeState(Enemy change, EnemyStates state)
    {
       

        change.enemyStates = state;

        
          
    }

    public override void EnterState(Enemy change)
    {
       
        if (change.GetComponent<AttackDefenceTowers>())
        {

            AttackDefenceTowers attackTowers = change.GetComponent<AttackDefenceTowers>();
            attackTowers.getEnemies(change.Towers, change.Damage);
            
           }
    }
}

public class DeathState : EnemyStates
{
    public override void ChangeState(Enemy change, EnemyStates state)
    {
        
    }

    public override void EnterState(Enemy change)
    {
        if (change != null)
        {
            change.KillEnemy();
           // Debug.Log("death");  
        }
        
    }
}

public class AttackTowerState : EnemyStates
{
    public override void ChangeState(Enemy change, EnemyStates state)
    {
        
    }

    public override void EnterState(Enemy change)
    {
        //  throw new System.NotImplementedException();
        if (change.GetComponent<AttackTower>())
        {
            AttackTower sucide = change.GetComponent<AttackTower>();
            sucide.BlowUpTower(change.TowerDamage);
        }
    }
}

public class StopState : EnemyStates
{
    public override void ChangeState(Enemy change, EnemyStates state)
    {
       
    }

    public override void EnterState(Enemy change)
    {
        
        Debug.Log("stopped");
       
    }
}

