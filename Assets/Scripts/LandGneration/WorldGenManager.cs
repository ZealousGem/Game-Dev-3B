using System.Collections.Generic;
using System.Drawing;
using System.IO;

using Unity.VisualScripting;
using UnityEngine;





public class WorldGenManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    Mesh mesh;
    Vector3[] vertcies; // co-orndates of each point in the map
    int[] triangles; // triangles that will be generated based on the vertices co-nrdates 

   

    [SerializeField]

    int xSize = 20;
    [SerializeField]

    int zSize = 20;

  
    public float Xoffset = 100f;
    public float Yoffset = 100f;

 // this scripts creates the ocean of the map 
    void Start() // creates a ew mesh once it has started 
    {

        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        Xoffset = Random.Range(0f, 9999f); // randomly genrates the x and Y offset to generate the waves of the water
        Yoffset = Random.Range(0f, 9999f);
        
        CreateShape();
        UpdateMesh();

    }


    void Update() // changes offsets to create wave effect
    {


        Xoffset += Time.deltaTime * 1f;
        Yoffset += Time.deltaTime * 1f;

        Xoffset %= 10000;
        Yoffset %= 10000;

        UpdateVert();
       
    }

    void UpdateVert() // updates noewly changed vertices 
    {
        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float y = CalHeight(z, x);
                vertcies[i].y = y;
                i++;
            }

        }

        mesh.vertices = vertcies;
        mesh.RecalculateNormals();
    }


    void CreateShape() // creates the map grid of the ocean terrain and genrates the mesh 
    {
        vertcies = new Vector3[(xSize + 1) * (zSize + 1)];

        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {

                float y = CalHeight(z, x);
                vertcies[i] = new Vector3(x, y, z);

                i++;
            }


        }

        triangles = new int[xSize * zSize * 6];
        int vert = 0; int tris = 0;

        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {

                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + xSize + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + xSize + 1;
                triangles[tris + 5] = vert + xSize + 2;

                vert++;
                tris += 6; // increases counter by 6 to make sure it does not update alreay set triangles 

            }

            vert++;
        }







    }

    float CalHeight(int x, int y) // calculates the height to genrates a height map for the perilin noise 
    {
        float xC = (float)x / zSize * 10f + Xoffset;
        float yC =(float)y / xSize * 10f + Yoffset;

        return Mathf.PerlinNoise(xC, yC);
    }

    void UpdateMesh() // sets the vertices and new triangles to the mesh 
    {
        mesh.Clear();
        mesh.vertices = vertcies;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();

    }
    // void OnDrawGizmos()
    // {

    //     if (vertcies == null) return;
    //     for (int i = 0; i < vertcies.Length; i++)
    //     {
    //         Gizmos.DrawSphere(vertcies[i], .1f);
    //     }
    // }

    // Update is called once per frame

}
