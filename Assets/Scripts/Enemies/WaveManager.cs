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

public class WaveManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public List<EnemyWaves> EnemyPrefabs;

     List<GameObject> CurrentEnemies = new List<GameObject>();

  //  public List<Enemytypes> Upgrade; 

    List<GameObject> Spawners;

    float counter = 0;

    int botskilled = 0;

    int ChangeWaveSet = 0;

    int waveIndex = 0;

    int maxbotKilled = 10;

    int currentWave = 1;

    public float maxCout = 1f;

    bool Spawned = false;

    bool isFound = false;


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

    void EndGame()
    {
        isFound = false;
    }

    void IncreaseBotKilledCount(int num)
    {
        botskilled += num;
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
            ChangeWaveSet++;
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

        Debug.Log("changed Wave" + currentWave);
        ChangeWavetype();
        yield return new WaitForSeconds(10f);
        isFound = true;
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
         ChangeWaveSet =  UnityEngine.Random.Range(currentWave + 2, 4);
        //ChangeWaveSet = 2;
        Debug.Log(ChangeWaveSet);
        SpawnEnemies();

    }

    void SpawnEnemies()
    {
        if (!Spawned)
        {
            int random = UnityEngine.Random.Range(0, CurrentEnemies.Count);
            int randomSpawner = UnityEngine.Random.Range(0, Spawners.Count);
            GameObject Enemy = CurrentEnemies[random];
            GameObject Spawer = Spawners[randomSpawner];

            Instantiate(Enemy, Spawer.transform.position, quaternion.identity);

            Spawned = true;

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
