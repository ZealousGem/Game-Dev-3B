using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public List<GameObject> EnemyPrefabs;
    
    List<GameObject> Spawners;

    float counter = 0;

    public float maxCout = 1f;

    bool Spawned = false;

    bool isFound = false;


    void Start()
    {
        StartCoroutine(FindSpawerns());
       
       
    }

    IEnumerator FindSpawerns()
    {
        yield return new WaitForSeconds(0.002f);
        string tag = "Enemy";
        Spawners = new List<GameObject>();
        Spawners.AddRange(GameObject.FindGameObjectsWithTag(tag));
        isFound = true;

        SpawnEnemies();

    }

    void SpawnEnemies()
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
