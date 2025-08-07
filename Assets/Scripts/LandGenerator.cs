using UnityEngine;
using System;
using System.Drawing;
using UnityEditor.TerrainTools;
using System.Collections.Generic;
using UnityEditor.Timeline;
using UnityEditor;


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

  public enum States
{
    Water,

    Land, 
    
 
}



public class LandGenerator : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created



    PointState[] points;

    [SerializeField]

    int xSize = 20;
    [SerializeField]

    int zSize = 20;

    Mesh mesh;

    void Start()
    {

        GenerateGrid();
        DetermineState();
        CreateMesh();

    }

    PointState[] GenerateGrid()
    {
        points = new PointState[(xSize + 1) * (zSize + 1)];
        for (int z = 0; z <= zSize; z++)
        {

            for (int x = 0; x <= xSize; x++)
            {
                int i = z * (xSize + 1) + x;
                int numberOfStates = Enum.GetValues(typeof(States)).Length;


                int randomIndex = UnityEngine.Random.Range(0, numberOfStates);

                States temp = (States)randomIndex;

                Vector3 vertcie = new Vector3(x, 0, z);

                PointState tempPoint = new PointState(vertcie, temp);
                points[i] = tempPoint;





            }

        }

        return points;

    }

    PointState[] DetermineState()
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

                    else
                    {
                        if (landNeighbours > 4)
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

    void CreateMesh()
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

              //  float y = Mathf.PerlinNoise(points[i].coord.x * 0.3f, points[i].coord.x * 0.1f) * 2f;
                Vector3 landVert = new Vector3(points[i].coord.x, 0, points[i].coord.z);
                meshVerts.Add(landVert);
                landMap.Add(i, meshVerts.Count - 1);

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


    
}
