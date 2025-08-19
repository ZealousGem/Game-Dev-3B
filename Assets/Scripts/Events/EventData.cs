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


public class DamageObjectEvent : EventData
{

    public int name;

    public float Damage;

    public DamageObjectEvent(int _name, float _Damage)
    {
        name = _name;
        Damage = _Damage;
    }

}
