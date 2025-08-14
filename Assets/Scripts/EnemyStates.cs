using UnityEngine;

public abstract class EnemyStates

{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public abstract void EnterState(MonoBehaviour change);

    public abstract void ChangeState(MonoBehaviour change, EnemyStates state);
}


public class MoveState : EnemyStates
{

    public override void ChangeState(MonoBehaviour change, EnemyStates state)
    {
        // throw new System.NotImplementedException();
        change.enabled = false;
       
    }

    public override void EnterState(MonoBehaviour change)
    {
        //  throw new System.NotImplementedException();
        change.enabled = true;

    }
}

public class AttackState : EnemyStates
{
    public override void ChangeState(MonoBehaviour change, EnemyStates state)
    {
        // throw new System.NotImplementedException();
           change.enabled = false;
    }

    public override void EnterState(MonoBehaviour change)
    {
        //  throw new System.NotImplementedException();
           change.enabled = true;
    }
}

public class AttackTowerState : EnemyStates
{
    public override void ChangeState(MonoBehaviour change, EnemyStates state)
    {
        // throw new System.NotImplementedException();
          change.enabled = false;
    }

    public override void EnterState(MonoBehaviour change)
    {
        //  throw new System.NotImplementedException();
         change.enabled = true;
    }
}

