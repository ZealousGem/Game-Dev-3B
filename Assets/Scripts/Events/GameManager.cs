using UnityEngine;

public enum StatsChange
{

    Health,
    MonenyGained,

    MoneyLost

}

public class GameManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public float MainTowerHealth = 200f;

    public float Money = 100f;

    void OnEnable()
    {
       EventBus.Subscribe<GameManagerEvent>(getData);
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
            default: Debug.Log("no enum found"); break;


        }
    }

    void DecreaseTowerHealth(float Damage)
    {
        if (MainTowerHealth > 0)
        {
            MainTowerHealth -= Damage;
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
            Money -= newAmount;
        }

        else if (Money <= 0)
        {
            Money = 0f;
        }
    }

    void IncreaseMoney(float newAmount)
    {
        Money += newAmount;
    }

    void EndGame()
    {
        Debug.Log("Game Over");
    }


}
