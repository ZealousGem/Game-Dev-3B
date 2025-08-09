using System.Collections;
using System.Drawing;
using UnityEngine;

public class BuildingsGenerator : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public GameObject tower;

    public GameObject EnemySpawners;

    public LandGenerator land;

    PointState[] points;



    void Start()
    {
        StartCoroutine(SpawnTower());
    }

    IEnumerator SpawnTower()
    {
        yield return new WaitForSeconds(0.001f);
        points = land.getPointState();
        int centerX = land.xSize / 2;
        int centerZ = land.zSize / 2;

        int IndexCenter = centerZ * (land.xSize + 1) + centerX;

        if (IndexCenter >= 0 && IndexCenter < points.Length)
        {
            Vector3 pos = points[IndexCenter].coord;
            Instantiate(tower, pos, Quaternion.identity);
       }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
