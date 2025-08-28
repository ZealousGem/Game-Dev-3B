using System.Collections;
using UnityEngine;

public enum StatsChange
{

    Health,
    MonenyGained,

    MoneyLost,

    HealthUI,

    EnemyDead,

    ChangeWave,

    EndGame

}

public class GameManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public float MainTowerHealth = 200f;

    public int Money = 50;

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

    void DecreaseTowerHealth(float Damage)
    {
        if (MainTowerHealth > 0)
        {
            MainTowerHealth -= Damage;
            GameManagerEvent HealthUI = new GameManagerEvent(MainTowerHealth, StatsChange.HealthUI);
            EventBus.Act(HealthUI);
            if (MainTowerHealth <= 0)
            {
                EndGame();
            }
        }
        Debug.Log(MainTowerHealth);
    }

    void DecreaseMoney(float newAmount)
    {
        if (Money > 0)
        {
            Money -= (int)newAmount;
            AmountEvent money = new AmountEvent(Money);
            EventBus.Act(money);
        }

        else if (Money <= 0)
        {
            Money = 0;
            AmountEvent money = new AmountEvent(Money);
            EventBus.Act(money);
        }

         //Debug.Log(Money);
    }

    void IncreaseMoney(float newAmount)
    {
        Money += (int)newAmount;
        AmountEvent money = new AmountEvent(Money);
        EventBus.Act(money);
       // Debug.Log(Money);
    }

    void EndGame()
    {
        Debug.Log("Game Over");
        EndGameEvent end = new EndGameEvent(StatsChange.EndGame);
        EventBus.Act(end);
    }


}
