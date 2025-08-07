using System.Collections.Generic;
using UnityEngine;



public class Pathing 
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    List<GameObject> path = new List<GameObject>();

    List<GameObject> topTiles = new List<GameObject>();
    List<GameObject> bottomTiles = new List<GameObject>();
    private int radius, curIndex;

    bool reachedX;

    bool reachedZ;

    GameObject startTile;

    GameObject endTile;

    public List<GameObject> GetGeneratedPath => path;

    public Pathing(int _radius)
    {
        radius = _radius;
    }

   public void AssignTopAndBottomTiles(int z, GameObject tile)
    {
        if (z == 0)
        {
            topTiles.Add(tile);
        }
        if (z == radius - 1)
        {
            bottomTiles.Add(tile);
        }
    }

    bool AssignChecStartTile()
    {
        var xIndex = Random.Range(0, topTiles.Count - 1);
        var zIndex = Random.Range(0, bottomTiles.Count);

        startTile = topTiles[xIndex];
        endTile = bottomTiles[zIndex];

        return startTile != null && endTile != null;
    }

   public void GeneratePath()
    {

        if (AssignChecStartTile())
        {
            GameObject curTile = startTile;

            var safeBreakX = 0;
            while (!reachedX)
            {
                safeBreakX++;

                if (safeBreakX > 100) break;


                if (curTile.transform.position.x > endTile.transform.position.x) moveDown(ref curTile);

                else if (curTile.transform.position.x < endTile.transform.position.x) moveUp(ref curTile);
                else reachedX = true;


            }
            
             var safeBreakZ = 0;
        while (!reachedZ)
        {
            if (safeBreakZ > 100) break;


            if (curTile.transform.position.z > endTile.transform.position.z) moveRight(ref curTile);

            else if (curTile.transform.position.z < endTile.transform.position.z) moveLeft(ref curTile);
            else reachedZ = true;

        }
        }

       

    }

    void moveDown(ref GameObject curTile)
    {
        path.Add(curTile);
      //  curIndex = WorldGenManager.GeneratedTiles.IndexOf(curTile);
        int n = curIndex - radius;
      //  curTile = WorldGenManager.GeneratedTiles[n];
    }

    void moveUp(ref GameObject curTile)
    {
        path.Add(curTile);
       // curIndex = WorldGenManager.GeneratedTiles.IndexOf(curTile);
        int n = curIndex + radius;
       // curTile = WorldGenManager.GeneratedTiles[n];
    }


    void moveLeft(ref GameObject curTile)
    {
        path.Add(curTile);
      //  curIndex = WorldGenManager.GeneratedTiles.IndexOf(curTile);
        curIndex++;
       // curTile = WorldGenManager.GeneratedTiles[curIndex];
    }

    void moveRight(ref GameObject curTile)
    {
        path.Add(curTile);
      //  curIndex = WorldGenManager.GeneratedTiles.IndexOf(curTile);
         curIndex--;
       // curTile = WorldGenManager.GeneratedTiles[curIndex];
    }
}
