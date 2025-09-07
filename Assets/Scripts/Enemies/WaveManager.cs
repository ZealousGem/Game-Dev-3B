using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public List<GameObject> EnemyPrefabs; // enemies wavemanager will spawn 
    
    List<GameObject> Spawners; // spawners located in map

    float counter = 0; // cooldown timer

    int botskilled = 0; // counter to track how many enemies have been

    int maxbotKilled = 10; // used to changed wave

    public float maxCout = 1f; // minuium cooldown for spawner

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

    void IncreaseBotKilledCount(int num) // increases the enemiy kill count everytime enemy has been killed 
    {
        botskilled += num;
        if (botskilled >= maxbotKilled)
        {
            botskilled = 0;
            maxbotKilled += 4;
            EndGameEvent WaveChange = new EndGameEvent(StatsChange.ChangeWave);
            EventBus.Act(WaveChange);
            StartCoroutine(ChangeWave());
        }

       // Debug.Log(botskilled);
    }

    IEnumerator ChangeWave() // changes the wave if enemies have reached max amount of enemies killed 
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
        
        Debug.Log("changed Wave" + maxCout);
        yield return new WaitForSeconds(10f);
        isFound = true; 
    }

    public void RegenSpawners()
    {
        isFound = false;
        StartCoroutine(FindSpawerns());

    }

    IEnumerator FindSpawerns() // finds spawners on the terrain 
    {
        yield return new WaitForSeconds(5f);
        string tag = "Enemy";
        Spawners = new List<GameObject>();
        Spawners.AddRange(GameObject.FindGameObjectsWithTag(tag));
        isFound = true;

        SpawnEnemies();

    }

    void SpawnEnemies() //  instates the enemy on the random spawner pciked 
    {
        if (!Spawned)
        {
            int random = UnityEngine.Random.Range(0, EnemyPrefabs.Count);
            int randomSpawner = UnityEngine.Random.Range(0, Spawners.Count);
            GameObject Enemy = EnemyPrefabs[random];
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
