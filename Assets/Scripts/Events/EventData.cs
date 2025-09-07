using System.Collections.Generic;
using UnityEngine;

public class EventData
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float cooled { get; private set; }

    public EventData(float _cooled = 0)
    {
        cooled = _cooled;
    }


}


public class DamageObjectEvent : EventData // migrates the damage that was inflited and goes to scripts that need to decrea their health 
{

    public int name;

    public float Damage;

    public DamageObjectEvent(int _name, float _Damage)
    {
        name = _name;
        Damage = _Damage;
    }

}

public class ChangeStateEvent : EventData // called when the state needs to be changed on the enemy 
{

    public int name;

    public List<GameObject> obj;

    public ChangeStateEvent(int _name, List<GameObject> _obj)
    {
        name = _name;
        obj = _obj;

    }

}

public class GameManagerEvent : EventData // migrates data from game manager to ui mamanger so it can be displayed to the player, updates the health ui and gold ui
{

    public float changed;

    public StatsChange type;

    public GameManagerEvent(float _changed, StatsChange _type)
    {
        changed = _changed;
        type = _type;

    }

}

public class AmountEvent : EventData // used for the defence UI script to know how much player has whether they can spend money to get a turret 
{

    public float changed;


    public AmountEvent(float _changed)
    {
        changed = _changed;


    }

}

public class EndGameEvent : EventData // ends the game if the main tower has been destoryed 
{

    public StatsChange type;

    public EndGameEvent(StatsChange _type)
    {
        type = _type;

    }

}
