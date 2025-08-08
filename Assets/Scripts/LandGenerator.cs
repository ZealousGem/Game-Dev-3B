using UnityEngine;
using System;
using System.Drawing;
using UnityEditor.TerrainTools;
using System.Collections.Generic;
using UnityEditor.Timeline;
using UnityEditor;
using System.Data;
using Mono.Cecil.Cil;
using NUnit.Framework.Internal;
using System.IO;
using System.Globalization;


public struct PointState
{
    public Vector3 coord;

    public States state;


    public PointState(Vector3 n, States s)
    {

        coord = n;
        state = s;
    }


}

public class AStarNode
{
    public int index;
    public AStarNode parent;
    public int gCost; 
    public int hCost; 
    public int fCost => gCost + hCost; 

    public AStarNode(int index)
    {
        this.index = index;
    }
}

public enum States
{
    Water,

    Land,

    Enemy,


}



public class LandGenerator : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created



    PointState[] points;

    public int scale = 0;

    [SerializeField]

    int xSize = 20;
    [SerializeField]

    int zSize = 20;

    Mesh mesh;

    public int num;

     System.Random random = new System.Random();

    void Start()
    {

        GenerateGrid();
        DetermineState();
         EnemyPath();
        MakePaths(num);
        ThicPath(num);
        CreateMesh();

    }

    public PointState[] GenerateGrid()
    {
       
        points = new PointState[(xSize + 1) * (zSize + 1)];
        for (int z = 0; z <= zSize; z++)
        {

            for (int x = 0; x <= xSize; x++)
            {
                int i = z * (xSize + 1) + x;

                States[] specificStates = { States.Water, States.Land };
             
               

               States temp = specificStates[random.Next(specificStates.Length)];

                Vector3 vertcie = new Vector3(x, gameObject.transform.position.y, z);

                PointState tempPoint = new PointState(vertcie, temp);
                points[i] = tempPoint;





            }

        }

        



        return points;

    }

   public PointState[] EnemyPath()
    {
        int Xcenter = xSize / 2;
        int Zcenter = zSize / 2;

        int IndexCenter = Zcenter * (xSize + 1) + Xcenter;

        if (IndexCenter >= 0 && IndexCenter < points.Length)
        {
            points[IndexCenter].state = States.Enemy;

            for (int zOff = -3; zOff <= 3; zOff++)
            {
                for (int xOff = -3; xOff <= 3; xOff++)
                {

                    if (zOff == 0 && xOff == 0) continue;

                    int Nx = Xcenter + xOff;
                    int Nz = Zcenter + zOff;

                    if (Nx >= 0 && Nx <= xSize && Nz >= 0 && Nz <= zSize)
                    {
                        int NeighbourIndex = Nz * (xSize + 1) + Nx;
                        points[NeighbourIndex].state = States.Enemy;
                    }

                }

                
            }
            
        }




         

        return points;
    }

    public void ThicPath(int pathWidth)
    {
         var pointsToWiden = new HashSet<int>();

        // First, find all existing enemy path tiles
        var existingEnemyPath = new List<int>();
        for (int i = 0; i < points.Length; i++)
        {
            if (points[i].state == States.Enemy)
            {
                existingEnemyPath.Add(i);
            }
        }

        // Now, iterate through the existing enemy path to find neighbors to widen
        foreach (int index in existingEnemyPath)
        {
            int x = index % (xSize + 1);
            int z = index / (xSize + 1);

            // Check all neighbors within the specified pathWidth
            for (int zOff = -pathWidth; zOff <= pathWidth; zOff++)
            {
                for (int xOff = -pathWidth; xOff <= pathWidth; xOff++)
                {
                    if (xOff == 0 && zOff == 0) continue;

                    int neighborX = x + xOff;
                    int neighborZ = z + zOff;

                    if (neighborX >= 0 && neighborX <= xSize && neighborZ >= 0 && neighborZ <= zSize)
                    {
                        int neighborIndex = neighborZ * (xSize + 1) + neighborX;
                        // Only widen into Land tiles, not water or other enemy tiles
                        if (points[neighborIndex].state == States.Land)
                        {
                            pointsToWiden.Add(neighborIndex);
                        }
                    }
                }
            }
        }

        
            

    }
    


    public PointState[] DetermineState()
    {
        PointState[] newPoints = new PointState[points.Length];
        Array.Copy(points, newPoints, points.Length);

        for (int it = 0; it < 2; it++)
        {
            for (int z = 0; z <= zSize; z++)
            {

                for (int x = 0; x <= xSize; x++)
                {

                    int i = z * (xSize + 1) + x;
                    if (points[i].state == States.Land)
                    {


                        continue;

                    }

                    if (points[i].state == States.Enemy)
                    {


                        continue;

                    }

                    int landNeighbours = 0;

                    for (int nZ = z - 1; nZ <= z + 1; nZ++)
                    {
                        for (int nX = x - 1; nX <= x + 1; nX++)
                        {
                            if (nX == x && nZ == z || nX < 0 || nX > xSize || nZ < 0 || nZ > zSize)
                            {

                                continue;
                            }
                            int curNeighbours = nZ * (xSize + 1) + nX;
                            if (points[curNeighbours].state == States.Land)
                            {
                                landNeighbours++;



                            }


                        }

                    }

                    if (points[i].state == States.Land)
                    {
                        if (landNeighbours < 1)
                        {
                            newPoints[i].state = States.Water;
                        }

                    }

                    else if (points[i].state == States.Water)
                    {
                        if (landNeighbours > scale)
                        {
                            newPoints[i].state = States.Land;
                        }
                    }


                }
            }
        }

        points = newPoints;
        newPoints = new PointState[points.Length];
        Array.Copy(points, newPoints, points.Length);
        return points;
    }

   public void MakePaths(int paths)
    {
        for (int i = 0; i < paths; i++)
        {
            AStarEnemyPathfiding();
        }
    }

    public void AStarEnemyPathfiding()
    {  
        
            int BeginX, BeginZ;
            int IndexCenter = (zSize / 2) * (xSize + 1) + (xSize / 2);

            int border = random.Next(0, 4);

            switch (border)
            {
                case 0:
                    BeginX = random.Next(0, xSize + 1);
                     BeginZ = 0;
                    break;
                case 1:
                    BeginX = xSize;
                    BeginZ = random.Next(0, zSize + 1);
                    break;
                case 2:
                    BeginX = random.Next(0, xSize + 1); BeginZ = zSize;
                    break;
                case 3:
                    BeginZ = 0; BeginX = random.Next(0, zSize + 1);
                    break;
                default:
                    BeginX = xSize / 2;
                    BeginZ = 0;
                    break;

            }

            int StartInd = BeginZ * (xSize + 1) + BeginX;

            var open = new List<AStarNode>();
            var close = new HashSet<int>();
            var allNodes = new Dictionary<int, AStarNode>();

            var NodeStart = new AStarNode(StartInd);
            NodeStart.gCost = 0;
            NodeStart.hCost = CalH(StartInd, IndexCenter);
            open.Add(NodeStart);
            allNodes.Add(StartInd, NodeStart);

            while (open.Count > 0)
            {

                AStarNode curNode = open[0];
                for (int l = 1; l < open.Count; l++)
                {
                    if (open[l].fCost < curNode.fCost || (open[l].fCost == curNode.fCost && open[l].hCost < curNode.hCost))
                    {
                        curNode = open[l];
                    }
                }

                open.Remove(curNode);
                close.Add(curNode.index);

                if (curNode.index == IndexCenter)
                {
                    ReconstructPath(curNode);
                    return;
                }

                int x = curNode.index % (xSize + 1);
                int z = curNode.index / (xSize + 1);

                for (int zOff = -1; zOff <= 1; zOff++)
                {
                    for (int xOff = -1; xOff <= 1; xOff++)
                    {

                        if (xOff == 0 && zOff == 0) continue;

                        int neighborX = x + xOff;
                        int neighborZ = z + zOff;

                        if (neighborX >= 0 && neighborX <= xSize && neighborZ >= 0 && neighborZ <= zSize)
                        {
                            int neighborIndex = neighborZ * (xSize + 1) + neighborX;

                            // Check if the neighbor is valid (e.g., not water) and not in the closed set
                            if (points[neighborIndex].state == States.Water || close.Contains(neighborIndex))
                            {
                                continue;
                            }

                            // Calculate the cost to move to the neighbor
                            int newGCost = curNode.gCost + ((xOff != 0 && zOff != 0) ? 14 : 10); // 14 for diagonal, 10 for cardinal

                            AStarNode neighborNode;
                            if (!allNodes.TryGetValue(neighborIndex, out neighborNode))
                            {
                                neighborNode = new AStarNode(neighborIndex);
                                allNodes.Add(neighborIndex, neighborNode);
                            }

                            // If a better path is found, update the neighbor's costs
                            if (newGCost < neighborNode.gCost || !open.Contains(neighborNode))
                            {
                                neighborNode.gCost = newGCost;
                                neighborNode.hCost = CalH(neighborIndex, IndexCenter);
                                neighborNode.parent = curNode;

                                if (!open.Contains(neighborNode))
                                {
                                    open.Add(neighborNode);
                                }
                            }
                        }

                    }

                }

            }



        


    }

    int CalH(int startId, int endId) {
        int stX = startId % (xSize + 1);
        int stZ = startId / (xSize + 1);
        int endX = endId % (xSize + 1);
        int endZ = endId / (xSize + 1);

        return Mathf.Abs(stX - endX) + Mathf.Abs(stZ - endZ);
    }

    private void ReconstructPath(AStarNode endNode)
    {
        
        AStarNode currentNode = endNode;
        while (currentNode != null)
        {
            points[currentNode.index].state = States.Enemy;
            currentNode = currentNode.parent;
        }

       
    }

    public void CreateMesh()
    {

        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        List<Vector3> meshVerts = new List<Vector3>();
        List<int> meshTriangles = new List<int>();
        Dictionary<int, int> landMap = new Dictionary<int, int>();

        for (int i = 0; i < points.Length; i++)
        {

            if (points[i].state == States.Land)
            {
                int NeighboursLand = 0;
                int z = i / (xSize + 1);
                int x = i % (xSize + 1);
                for (int nZ = z - 1; nZ <= z + 1; nZ++)
                {
                    for (int nX = x - 1; nX <= x + 1; nX++)
                    {
                        if (nX == x && nZ == z || nX < 0 || nX > xSize || nZ < 0 || nZ > zSize)
                        {

                            continue;
                        }
                        int curNeighbours = nZ * (xSize + 1) + nX;
                        if (points[curNeighbours].state == States.Land)
                        {
                            NeighboursLand++;
                        }


                    }
                }
                float height = 1f;

                if (NeighboursLand >= 7)
                {
                    height = 0f;
                }

                else if (NeighboursLand >= 4)
                {
                    height = 0f;
                }
                float y = Mathf.PerlinNoise(points[i].coord.x * 0.1f, points[i].coord.z * 0.1f) * height;
                Vector3 landVert = new Vector3(points[i].coord.x, y, points[i].coord.z);
                meshVerts.Add(landVert);
                landMap.Add(i, meshVerts.Count - 1);

                //  float y = Mathf.PerlinNoise(points[i].coord.x * 0.3f, points[i].coord.x * 0.1f) * 2f;


            }

        }


        for (int z = 0; z < zSize; z++)
        {

            for (int x = 0; x < xSize; x++)
            {
                int i = z * (xSize + 1) + x;

                if (points[i].state == States.Land)
                {

                    if (points[i].state == States.Land &&
                   points[i + 1].state == States.Land &&
                   points[i + xSize + 1].state == States.Land &&
                   points[i + xSize + 2].state == States.Land)
                    {
                        meshTriangles.Add(landMap[i]);
                        meshTriangles.Add(landMap[i + xSize + 1]);
                        meshTriangles.Add(landMap[i + 1]);

                        meshTriangles.Add(landMap[i + 1]);
                        meshTriangles.Add(landMap[i + xSize + 1]);
                        meshTriangles.Add(landMap[i + xSize + 2]);
                    }


                }
            }
        }

        mesh.Clear();
        mesh.vertices = meshVerts.ToArray();
        mesh.triangles = meshTriangles.ToArray();

        mesh.RecalculateNormals();
    }

     void OnDrawGizmos()
     {

         if (points == null) return;
         for (int i = 0; i < points.Length; i++)
         {
             Gizmos.DrawSphere(points[i].coord, .1f);
         }
     }

}



[CustomEditor(typeof(LandGenerator))]

public class Button : Editor
{
    
    public override void OnInspectorGUI()
    {
        LandGenerator land = (LandGenerator)target;
        int numb = land.num;

        DrawDefaultInspector();
        if (GUILayout.Button("Generate"))
        {
            land.GenerateGrid();
            land.DetermineState();
            land.EnemyPath();
             land.MakePaths(numb);
            land.ThicPath(numb);
            land.CreateMesh();
        }
    }
}




