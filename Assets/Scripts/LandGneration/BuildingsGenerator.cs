using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingsGenerator : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public GameObject tower;  // tower prefab

    public GameObject EnemySpawnersObj;  // enemy spanwer prefabs enemy will spawn 

    public LandGenerator land; // gets the terrain map 

    HashSet<Vector3> EnemySpawnerPos = new HashSet<Vector3>();  // used haset to place in the list of spawners 

    List<GameObject> obj;

    GameObject towerobj;

    PointState[] points;

   



    void Start()
    {
        StartCoroutine(SpawnTower());
        StartCoroutine(EnemeySpawners());
    }

    public void Spawn() // spawns objects above 
    {
       string[] tags = { "Tower", "Enemy", "DefenceTower" }; // destorys any exisiting objects based on the tag
       obj = new List<GameObject>();

        foreach (string t in tags)
        {

            obj.AddRange(GameObject.FindGameObjectsWithTag(t));
           

        }

        for (int i = 0; i < obj.Count; i++)
        {
            DestroyImmediate(obj[i]);
        }

        EnemySpawnerPos.Clear(); // clears old spawner co-orndate to allow new ones to spawn 

        StartCoroutine(SpawnTower());
        StartCoroutine(EnemeySpawners());
    }

    IEnumerator SpawnTower()  // spawns the tower in the centre the map 
    {
        yield return new WaitForSeconds(0.001f);
        points = land.getPointState();
        int centerX = land.xSize / 2;
        int centerZ = land.zSize / 2;

        int IndexCenter = centerZ * (land.xSize + 1) + centerX;

        
        if (IndexCenter >= 0 && IndexCenter < points.Length)
        {
            Vector3 pos = points[IndexCenter].coord;
            towerobj = Instantiate(tower, pos, Quaternion.identity, gameObject.transform);
        }
    }

    IEnumerator EnemeySpawners()
    {

        yield return new WaitForSeconds(0.001f);
        points = land.getPointState();
        int centerX = land.xSize / 2;
        int centerZ = land.zSize / 2;
        Vector3 pos = new Vector3(0, 0, 0);

        int IndexCenter = centerZ * (land.xSize + 1) + centerX;
        if (IndexCenter >= 0 && IndexCenter < points.Length)
        {
            pos = points[IndexCenter].coord;

        }

        for (int i = 0; i < land.zSize; i++) // loops the map
        {
            for (int j = 0; j < land.xSize; j++)
            {

                if (i == 0 || i == land.zSize - 1 || j == 0 || j == land.xSize - 1)
                {
                    int ind = i * (land.xSize + 1) + j; // finds the current node loop is on
                    if (j > 0)
                    {
                        if (ind >= 0 && ind < points.Length && points[ind].state == States.Enemy) // will be added if the node state is an enemy 
                        {
                            EnemySpawnerPos.Add(points[ind].coord);
                        }
                        
                    }
                        

                     if ( j < land.xSize)
                    {
                        if (ind >= 0 && ind < points.Length && points[ind].state == States.Enemy) // will be added if the node state is an enemy 
                        {
                            EnemySpawnerPos.Add(points[ind].coord);
                        }
                    }
                         
                    
                    
                 }

            }

        }

        // Quaternion block = Quaternion.LookRotation(pos);

        

        foreach (Vector3 x in EnemySpawnerPos) // isntate spawners
        {

         GameObject temp = Instantiate(EnemySpawnersObj, x, Quaternion.identity, gameObject.transform);
          


        }
         

    }
    

}
