using UnityEngine;
using System;
using System.Collections.Generic;

public static class EventBus
{

    private static Dictionary<Type, Delegate> SubbedActions = new();

    public static void Act(EventData data)
    {

        Type newType = data.GetType();

        if (SubbedActions.TryGetValue(newType, out Delegate ActionExists))
        {

            ActionExists.DynamicInvoke(data);
        }

        else
        {
            Debug.Log("event type: " + newType.Name + " not found");
        }

    }

    public static void Subscribe<T>(Action<T> act) where T : EventData
    {
        Type type = typeof(T);

        if (SubbedActions.ContainsKey(type))
        {
            SubbedActions[type] = Delegate.Combine(SubbedActions[type], act);
        }

        else
        {
            SubbedActions[type] = act;
        }
    }

    public static void Unsubscribe<T>(Action<T> act) where T : EventData
    {
        Type type = typeof(T);

        if (SubbedActions.ContainsKey(type))
        {
            Delegate curDel = SubbedActions[type];
            Delegate newDel = Delegate.Remove(curDel, act);

            if (newDel == null)
            {

                SubbedActions.Remove(type);

            }

            else
            {
                SubbedActions[type] = newDel; 
           }

        }


    }
    

    
}