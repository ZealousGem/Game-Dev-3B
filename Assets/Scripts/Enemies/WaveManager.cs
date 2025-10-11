using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public enum Enemytypes
{
    Health,
    Damage,

    TowerDamage,

    Speed,


}

public struct EnemyWeights
{
    GameObject Enemy;

    int weight;

    public EnemyWeights(GameObject _Enemy, int _weight)
    {
        Enemy = _Enemy;
        weight = _weight;
    }

    public GameObject getEnemy()
    {
        return Enemy;
    }
    
    public int getWeight()
    {
        return weight;
    }
}

public class WaveManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public List<EnemyWaves> EnemyPrefabs;

    List<EnemyWeights> Enemies = new List<EnemyWeights>();

    List<GameObject> CurrentEnemies = new List<GameObject>();

    public List<GameObject> EnemiesToSpawn = new List<GameObject>(); 

    List<GameObject> Spawners;

    float counter = 0;

    int botskilled = 0;

    int ChangeWaveSet = 0;

    int waveIndex = 0;

    int maxbotKilled = 10;

    int currentWave = 1;

    public float maxCout = 1f;

    int SpawnEnemiesCounter = 0;

    bool Spawned = false;

    bool isFound = false;

    int currentDamageIncrease = 0;

    int currentHealthIncrease = 0;

    int increaseMoney = 0;


    void Start()
    {
        StartCoroutine(FindSpawerns());
    }

    void OnEnable()
    {
        EventBus.Subscribe<GameManagerEvent>(getData);
        EventBus.Subscribe<EndGameEvent>(getEndDate);
    }

    void OnDisable()
    {
        EventBus.Unsubscribe<EndGameEvent>(getEndDate);
        EventBus.Unsubscribe<GameManagerEvent>(getData);
    }

    void getData(GameManagerEvent data)
    {
        if (StatsChange.EnemyDead == data.type)
        {
            IncreaseBotKilledCount((int)data.changed);
        }
    }

    void getEndDate(EndGameEvent data)
    {
        if (data.type == StatsChange.EndGame)
        {
            EndGame();
        }

    }

    void CalEnemyWieghts()
    {
        // CurrentEnemies.Clear();
        Enemies.Clear();
        EnemiesToSpawn.Clear();
        int totalWieght = 0;

     if (CurrentEnemies == null || CurrentEnemies.Count == 0)
    {
        Debug.LogError("CurrentEnemies list is empty! Cannot calculate weights.");
        return;
    }
        foreach (GameObject enemy in CurrentEnemies)
        {
            int randomWeight = UnityEngine.Random.Range(1, 6);
            EnemyWeights en = new EnemyWeights(enemy, randomWeight);
            Enemies.Add(en);
            totalWieght += randomWeight;
            


        }
        int RandomEnemyAmount = UnityEngine.Random.Range(2, 8);
        for (int i = 0; i < RandomEnemyAmount; i++)
        {
           int RandomWeight = UnityEngine.Random.Range(0, totalWieght);
           foreach (EnemyWeights e in Enemies)
           {
            RandomWeight -= e.getWeight();
            if (RandomWeight < 0)
            {
                GameObject gameObject = e.getEnemy();
                EnemiesToSpawn.Add(gameObject);
                break;  
            }
          }
        }
       
      


    }

    void EndGame()
    {
        isFound = false;
    }

    void IncreaseBotKilledCount(int num)
    {
        botskilled += num;
        Debug.Log(botskilled);
        if (botskilled >= maxbotKilled)
        {
            botskilled = 0;
            maxbotKilled += 4;
            EndGameEvent WaveChange = new EndGameEvent(StatsChange.ChangeWave);
            EventBus.Act(WaveChange);
            currentWave += 1;
            StartCoroutine(ChangeWave());
        }

        // Debug.Log(botskilled);
    }

    void ChangeWavetype()
    {
        if (currentWave == ChangeWaveSet && waveIndex < EnemyPrefabs.Count)
        {
            CurrentEnemies = EnemyPrefabs[waveIndex].Enemies;
            int rand = UnityEngine.Random.Range(currentWave, currentWave + 4);
            ChangeWaveSet = rand;
            waveIndex++;
            Debug.Log(ChangeWaveSet);

        }

        else
        {
            return;
        }
    }

    IEnumerator ChangeWave()
    {
        isFound = false;

        if (maxCout > 0.2)
        {
            maxCout -= 0.4f;
        }

        else
        {
            maxCout = 0.2f;
        }

        Debug.Log("changed Wave " + currentWave);
        SpawnEnemiesCounter = 0;
        ChangeWavetype();
        IncreaseStats();
        CalEnemyWieghts();
        yield return new WaitForSeconds(10f);
        isFound = true;
    }

    void IncreaseStats()
    {

        if (currentWave == 5)
        {
            currentDamageIncrease = 3;
            currentHealthIncrease = 5;
            increaseMoney = 1;

        }

        else if (currentWave > 5)
        {
            int newDamage = UnityEngine.Random.Range(1, 3);
            int newHealth = UnityEngine.Random.Range(1, 5);
            int newMoney = UnityEngine.Random.Range(2, 6);

            currentDamageIncrease += newDamage;
            currentHealthIncrease += newHealth;
            increaseMoney += newMoney;
        }
    }

    public void RegenSpawners()
    {
        isFound = false;
        StartCoroutine(FindSpawerns());

    }

    IEnumerator FindSpawerns()
    {
        yield return new WaitForSeconds(5f);
        string tag = "Enemy";
        Spawners = new List<GameObject>();
        Spawners.AddRange(GameObject.FindGameObjectsWithTag(tag));
        isFound = true;
        CurrentEnemies = EnemyPrefabs[0].Enemies;
        waveIndex++;
        ChangeWaveSet = 4;
        //ChangeWaveSet = 2;
        Debug.Log("Wave type: " + ChangeWaveSet);
        CalEnemyWieghts();
        SpawnEnemies();

    }

    void SpawnEnemies()
    {
        if (!Spawned && SpawnEnemiesCounter <= maxbotKilled)
        {
            int random = UnityEngine.Random.Range(0, EnemiesToSpawn.Count);
            int randomSpawner = UnityEngine.Random.Range(0, Spawners.Count);
            GameObject Enemy = EnemiesToSpawn[random];
            GameObject Spawer = Spawners[randomSpawner];

            GameObject intst = Instantiate(Enemy, Spawer.transform.position, quaternion.identity);
            Enemy script = intst.GetComponent<Enemy>();
            if (script != null)
            {
                script.Health += currentHealthIncrease;
                script.Damage += currentDamageIncrease;
                script.Money += increaseMoney;

            }
            SpawnEnemiesCounter++;
            Spawned = true;

        }

        else
        {
            Debug.Log("no more enemies");
        }



    }

    // Update is called once per frame
    void Update()
    {


        if (isFound)
        {
            counter += Time.deltaTime;
            if (counter >= maxCout)
            {
                Spawned = false;
                counter = 0f;
                SpawnEnemies();
            }



        }

    }
}
