using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

public enum StatsChange // enums used to call event bus and create specfic event 
{

    Health,
    MonenyGained,

    MoneyLost,

    HealthUI,

    EnemyDead,

    ChangeWave,

    EndGame,

    PausedGame,

    UnPausedGame,

    EnemieLeft,


    StartWave,

    hideUpgrades,

}

public class GameManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public float MainTowerHealth = 200f;

    public int Money = 50;

    public GameObject explosion;

    void Start()
    {

    } // main tower explosion effect 

    void OnEnable()
    {
        EventBus.Subscribe<GameManagerEvent>(getData);
        AmountEvent money = new AmountEvent(Money);
        EventBus.Act(money);
    }

    void OnDisable()
    {
        EventBus.Unsubscribe<GameManagerEvent>(getData);
    }


    void getData(GameManagerEvent data)
    {
        switch (data.type)
        {
            case StatsChange.Health: DecreaseTowerHealth(data.changed); break;
            case StatsChange.MonenyGained: IncreaseMoney(data.changed); break;
            case StatsChange.MoneyLost: DecreaseMoney(data.changed); break;
            case StatsChange.ChangeWave: if (MainTowerHealth > 0) { DisplayDialogue(); } break;



        }
    }

    void DisplayDialogue()
    {
        List<GameCondition> lists = new List<GameCondition>();
        if (MainTowerHealth > 100f)
        {
            lists.Add(GameCondition.Tower_Health_Equals_100);
        }

        if (MainTowerHealth < 100f)
        {
            lists.Add(GameCondition.Tower_Health_Less_Than_50);
        }

        if (Money > 100)
        {
            lists.Add(GameCondition.Lots_Of_Gold);
        }

        if (Money < 100)
        {
            lists.Add(GameCondition.Not_alotOf_Gold);
        }


        System.Random random = new System.Random();
        GameCondition temp = lists[random.Next(lists.Count)];
        DialogueEvent dialogue = new DialogueEvent(temp);
        EventBus.Act(dialogue);

    }

    public void DecreaseTowerHealth(float Damage) // decreases the main towers health evertyime enemy has reached it 
    {
        if (MainTowerHealth > 0)
        {
            MainTowerHealth -= Damage;
            GameManagerEvent HealthUI = new GameManagerEvent(MainTowerHealth, StatsChange.HealthUI);
            EventBus.Act(HealthUI);
            GameManagerEvent EnemyKilled = new GameManagerEvent(1, StatsChange.EnemyDead);
            EventBus.Act(EnemyKilled);
            if (MainTowerHealth <= 0)
            {
                EndGame();
            }
        }
        Debug.Log(MainTowerHealth);
    }

    public void DecreaseMoney(float newAmount) // decreases the money player has once they have purchased a turret 
    {
        if (Money > 0)
        {
            Money -= (int)newAmount;
        }

        else if (Money <= 0)
        {
            Money = 0;
        }

        AmountEvent money = new AmountEvent(Money);
        EventBus.Act(money);
        //ShowMoney();
        //Debug.Log(Money);
    }

    void ShowMoney()
    {
        Debug.Log(Money);
    }

    void IncreaseMoney(float newAmount) // increases money if the player has kiiled and enemy 
    {
        Money += (int)newAmount;
        AmountEvent money = new AmountEvent(Money);
        EventBus.Act(money);
        // ShowMoney();
        // Debug.Log(Money);
    }

    void EndGame()  // ends the game if the main towers health is 0 
    {
        Debug.Log("Game Over");
        EndGameEvent end = new EndGameEvent(StatsChange.EndGame);
        EventBus.Act(end);
        GameObject obj = GameObject.FindGameObjectWithTag("Tower");
        if (obj != null)
        {
            Instantiate(explosion, obj.gameObject.transform.position, quaternion.identity);
            Destroy(obj);
        }




    }


}

 [CustomEditor(typeof(GameManager))]

 public class EndButton : Editor
{
    
    public override void OnInspectorGUI()
    {
        GameManager land = (GameManager)target;
     

        DrawDefaultInspector();
        if (GUILayout.Button("Generate"))
        {
            land.DecreaseTowerHealth(200);
        }
    }
}
