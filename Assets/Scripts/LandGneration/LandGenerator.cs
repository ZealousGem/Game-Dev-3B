using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEditor;
using Unity.Mathematics;



public struct PointState // struct node containing the co-orndates of the grid and what enum state the node is to check whether it is water land or enemy
{
    public Vector3 coord;

    public States state;

  

    public PointState(Vector3 n, States s)
    {

        coord = n;
        state = s;
    }


}

public class AStarNode  // astar node used to calucalte path to the center of the tower using heautric formlua
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

public enum States // enums using to differantie nodes in the gird to create proceurdal map 
{
    Water,

    Land,

    Enemy,


}



public class LandGenerator : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created



    PointState[] points; // map gird

    public int scale = 0; // used to determine how many similar land neigbours are suroounding a water node to change its state to a land 

    [SerializeField]

   public int xSize = 20;  // size of the map
    [SerializeField]

   public  int zSize = 20;

    Mesh mesh; // mesh that will create the islands once neighbours and vertices have been determined 

    public int num; // variable used to create the enemy paths

    public int Wide;

    public List<GameObject> jungleobjects;  // objects tat will spawn on the islands 

     GameObject curjungleobjects;


    Vector3 middle; // middle co-orndiates of the map

    MeshCollider Landcollider;

     System.Random random = new System.Random(); // used to randomly generate the enum states of the nodes to create proceudral islands 
    

    public PointState[] getPointState()
    {
        return points;
    }

    public Vector3 getMiddle()
    {
        return middle;
    }

    void Start()
    {
        Landcollider = GetComponent<MeshCollider>();
        GenerateGrid();
        DetermineState();
        TowerPath();
        MakePaths(num);
        ThicPath(Wide);
        CreateMesh();

    }

    public PointState[] GenerateGrid() // creates the map grid and randomised the enum states to each node to create a randomised proceuderadl terrain
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

   public PointState[] TowerPath() // this setts the middle of the map which is 9x9 grid setting all the states to enemy to set the enemies final destination as well as making sure islands do not be created in this centre grid
    {
        int Xcenter = xSize / 2;
        int Zcenter = zSize / 2;

        int IndexCenter = Zcenter * (xSize + 1) + Xcenter;

        if (IndexCenter >= 0 && IndexCenter < points.Length)
        {
            points[IndexCenter].state = States.Enemy;
            middle = points[IndexCenter].coord;
            

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

    public void ThicPath(int pathWidth) // makes the enemy path thicker by setting any land enum states to water if a land state next to the enemy enum state  
    {
         var pointsToWiden = new HashSet<int>();

       
        var existingEnemyPath = new List<int>();
        for (int i = 0; i < points.Length; i++)
        {
            if (points[i].state == States.Enemy)
            {
                existingEnemyPath.Add(i);
            }
        }

        
        foreach (int index in existingEnemyPath)
        {
            int x = index % (xSize + 1);
            int z = index / (xSize + 1);

            
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
                        
                        if (points[neighborIndex].state == States.Land)
                        {
                            pointsToWiden.Add(neighborIndex);
                        }
                    }
                }
            }
        }

        foreach (int ind in pointsToWiden)
        {

            points[ind].state = States.Water;
        }
            

    }
    


    public PointState[] DetermineState() // this function detects neighbours surrouding its node co-rodniate and changes to the desired state based on the rules below
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
                        if (landNeighbours < 1)  // if there are less than 1 land neighobur the land node will change to water 
                        {
                            newPoints[i].state = States.Water;
                        }

                    }

                    else if (points[i].state == States.Water)
                    {
                        if (landNeighbours > scale) //  if there are more than 3 land neighbours, the water node will change to land 
                        {
                            newPoints[i].state = States.Land;
                        }
                    }


                }
            }
        }

        points = newPoints;
        newPoints = new PointState[points.Length];  // overiddes the previous grid map using the new grid map creating using Moore Neighbourhood formula 
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

    public void AStarEnemyPathfiding() // this function uses astar to generate the enemy path the enemy player will traverse it uses astar from the edgoe of the map and calucaltes the lowest cost to get to the centre
    {  
        
            int BeginX, BeginZ;
            int IndexCenter = (zSize / 2) * (xSize + 1) + (xSize / 2);

            int border = random.Next(0, 4);

            switch (border) // finds a random edge in the map
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

            while (open.Count > 0)  // will find the best path from the edge to the centre by finind the lowest H
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

                        if (neighborX >= 0 && neighborX <= xSize && neighborZ >= 0 && neighborZ <= zSize) // make sure grid is not out of bounds
                        {
                            int neighborIndex = neighborZ * (xSize + 1) + neighborX;

                            // Check if the neighbor is valid 
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

    int CalH(int startId, int endId) { // calculates the H in the Heaursitic foromula , h(n)
        int stX = startId % (xSize + 1);
        int stZ = startId / (xSize + 1);
        int endX = endId % (xSize + 1);
        int endZ = endId / (xSize + 1);

        return Mathf.Abs(stX - endX) + Mathf.Abs(stZ - endZ);
    }

    private void ReconstructPath(AStarNode endNode) // makes path if the current nodes index has reached the center hence a path is made 
    {
        
        AStarNode currentNode = endNode;
        while (currentNode != null)
        {
            points[currentNode.index].state = States.Enemy;
            currentNode = currentNode.parent;
        }

       
    }

    public void CreateMesh() // creates the mesh islands once the grid map islands and enemy paths have been set
    {
        
        string[] tags = { "Jungle" };
        List<GameObject> obj = new List<GameObject>();

        foreach (string t in tags)
        {

            obj.AddRange(GameObject.FindGameObjectsWithTag(t));
           

        }

        for (int i = 0; i < obj.Count; i++)
        {
            DestroyImmediate(obj[i]);
        }
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        List<Vector3> meshVerts = new List<Vector3>();  // list that will store the co-ordnates for the vertices
        List<int> meshTriangles = new List<int>(); // triangles that will be made from the vertices to generate mesh
        List<int> BeachTriangles = new List<int>();  // genrates trianlges located on the edge of the isalnds 
        Dictionary<int, int> landMap = new Dictionary<int, int>(); // dictionary used to make sure vertices are laid out similar to the pointstate grid 
        float islandHeight = 1.1f;

        for (int i = 0; i < points.Length; i++) // will only create verticies, if the node's enum state is a land to genreate islands
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
                float height = 0f;

                if (NeighboursLand >= 7)
                {
                    height = 0f;
                }

                else if (NeighboursLand >= 4)
                {
                    height = 0f;
                }
                float y = Mathf.PerlinNoise(points[i].coord.x * 0.1f, points[i].coord.z * 0.1f) * height; // creates the height of the island using perilins noise
                Vector3 landVert = new Vector3(points[i].coord.x, y, points[i].coord.z);
                meshVerts.Add(landVert);
                landMap.Add(i, meshVerts.Count - 1);

                //  float y = Mathf.PerlinNoise(points[i].coord.x * 0.3f, points[i].coord.x * 0.1f) * 2f;


            }

        }

        
        for (int z = 0; z < zSize; z++) // adds all the vertices into the mesh triangle mesh and checks if node and its three neighbours surrounding is are land nodes to be able to create a square with 2 triangles
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
                        meshTriangles.Add(landMap[i]);  // adds the verticle to the traingles to create a square 
                        meshTriangles.Add(landMap[i + xSize + 1]);
                        meshTriangles.Add(landMap[i + 1]);

                        meshTriangles.Add(landMap[i + 1]);
                        meshTriangles.Add(landMap[i + xSize + 1]);
                        meshTriangles.Add(landMap[i + xSize + 2]);

                        bool isBeachVert = false; // checks whether the vertices is located close to water nodes

                        int[] quadCornerInd = { i, i + 1, i + xSize + 1, i + xSize + 2 };


                        foreach (int cornerIDx in quadCornerInd)
                        {
                            int cx = cornerIDx % (xSize + 1);
                            int cz = cornerIDx / (xSize + 1);

                            for (int nZ = cz - 1; nZ <= cz + 1; nZ++) // loops through the grid 
                            {
                                for (int nX = cx - 1; nX <= cx + 1; nX++)
                                {
                                    if (nX == cx && nZ == cz) continue;
                                    if (nX >= 0 && nX <= xSize && nZ >= 0 && nZ <= zSize)
                                    {

                                        int NeighbourIndex = nZ * (xSize + 1) + nX;
                                        if (points[NeighbourIndex].state == States.Water) // if a water node is next  to the land node the vertice will alos be added to the beach traingles 
                                        {
                                        //    Debug.Log("through");
                                            isBeachVert = true;
                                            break;
                                        }
                                    }

                                }
                                if (isBeachVert) break;
                            }
                            if (isBeachVert) break;
                        }

                        if (isBeachVert) // adds the vertice and it's neighbours in the beach traingle list 
                        {
                            BeachTriangles.Add(landMap[i]);
                            BeachTriangles.Add(landMap[i + xSize + 1]);
                            BeachTriangles.Add(landMap[i + 1]);
                            BeachTriangles.Add(landMap[i + 1]);
                            BeachTriangles.Add(landMap[i + xSize + 1]);
                            BeachTriangles.Add(landMap[i + xSize + 2]);

                        }
                        int num = UnityEngine.Random.Range(0, 8); // randomises the number to determine which object will spawn on the node

                        if (num == 2) // spawns a tree
                        {

                            curjungleobjects = jungleobjects[0];
                            Vector3 pos = new Vector3(points[i].coord.x, islandHeight, points[i].coord.z);
                            Instantiate(curjungleobjects, pos, quaternion.identity, gameObject.transform);

                        }

                        else
                        {
                            int num2 = UnityEngine.Random.Range(0, 50);
                     
                            if (num2 == 39 && !isBeachVert) // spawns a temple
                            {
                                curjungleobjects = jungleobjects[1];
                                Vector3 pos = new Vector3(points[i].coord.x, islandHeight, points[i].coord.z);
                                Instantiate(curjungleobjects, pos, quaternion.identity, gameObject.transform);


                            }

                            
                        }

                       
                            
                        
                       

                    }


                }
            }
        }

        
          // makes the mesh once all the triangles have been set above and generates the mesh 
        mesh.Clear();
        mesh.vertices = meshVerts.ToArray();
        mesh.subMeshCount = 2;
        mesh.SetTriangles(meshTriangles.ToArray(), 0); // genrates the island
        mesh.SetTriangles(BeachTriangles.ToArray(), 1);  // genrates the beach surrounding the islands 

        Landcollider.sharedMesh = mesh;
        mesh.RecalculateNormals();
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();


      Material[] materials = meshRenderer.sharedMaterials;
      materials[0].renderQueue = 2000;  
      materials[1].renderQueue = 2002;

        
    }

    // void OnDrawGizmos()
    // {

    //     if (points == null) return;
    //     for (int i = 0; i < points.Length; i++)
    //     {
    //         if (points[i].state == States.Enemy)
    //         {
    //             Color gizmoColour = Color.red;
    //             Gizmos.color = gizmoColour;
    //             Gizmos.DrawSphere(points[i].coord, .1f);

    //         }

    //         else if (points[i].state == States.Land)
    //         {
    //             Color gizmoColour = Color.green;
    //             Gizmos.color = gizmoColour;
    //             Gizmos.DrawSphere(points[i].coord, .1f);
    //         }

    //         else
    //         {
    //             Color gizmoColour = Color.gray;
    //             Gizmos.color = gizmoColour;
    //             Gizmos.DrawSphere(points[i].coord, .1f);
    //         }


    //     }
    // }

}



// [CustomEditor(typeof(LandGenerator))]

// public class Button : Editor
// {
    
//     public override void OnInspectorGUI()
//     {
//         LandGenerator land = (LandGenerator)target;
//         BuildingsGenerator build = GameObject.FindGameObjectWithTag("Respawn").GetComponent<BuildingsGenerator>();
//         WaveManager Spawners = GameObject.FindGameObjectWithTag("Respawn").GetComponent<WaveManager>();
//         int numb = land.num;
//         int wd = land.Wide;

//         DrawDefaultInspector();
//         if (GUILayout.Button("Generate"))
//         {
//             land.GenerateGrid();
//             land.DetermineState();
//             land.TowerPath();
//             land.MakePaths(numb);
//             land.ThicPath(wd);
//             land.CreateMesh();
//             build.Spawn();
//             Spawners.RegenSpawners();
//         }
//     }
// }




