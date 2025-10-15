using System.Collections;
using Unity.Mathematics;
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

}

public class GameManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public float MainTowerHealth = 200f;

    public int Money = 50;

    public GameObject explosion; // main tower explosion effect 

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
            


        }
    }

    void DecreaseTowerHealth(float Damage) // decreases the main towers health evertyime enemy has reached it 
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

    void DecreaseMoney(float newAmount) // decreases the money player has once they have purchased a turret 
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
