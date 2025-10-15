using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class CoNode // this node will be used to calculcate the enemies path to the centre once done it uses the nodes in the genrates and adds it to a linked list to create fuild movement
{
    public CoNode parent;
    public int gCost;

    public CoNode next;

    public Vector3 coord;
    public int hCost;
    public int fCost => gCost + hCost;

    public CoNode(Vector3 coord)
    {
        this.coord = coord;
        next = null;
    }
}

public class Pathing : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    PointState[] points;  // grid that will retrived from the land generator script 
    Vector3 Tower; // co-ordinates of the Tower in the middle of the map

    Vector3 curPos;  // current postion of the enemy in the map 
    bool hasPath = false;    // bool used to determine whether path has been found allow enemy to move 

    CoNode CurPoint;  // currentnode enemy is heading to

    List<CoNode> path;  // generate linked list enemies uses to get to tower

    float speed;

    public void intPathing(PointState[] points, Vector3 Tower, Vector3 curPos, float spe) // method used to set all the needed variables the enemy will need to get to the tower
    {
        this.points = points;
        this.Tower = Tower;
        // Debug.Log(Tower);
        // Debug.Log(curPos);
        this.curPos = curPos;
        speed = spe;
        MakePath();


    }

    public void StopMoving() // meothd used to stop enemy from moving to tower if the tower is destroyed or defence tower is close by 
    {
        hasPath = false;
        
        
    }

    void MakePath() // another astar fucntion to calculate the enemies path neededt o the tower to make sure the enemy gets the right path from the grid map, then sets that path into the linked list
    {

        PointState[] tempPoint = points;

    if (points == null)
    {
        Debug.Log("where the fuck is it bitch");
    }

   
    Dictionary<Vector3, PointState> map = new Dictionary<Vector3, PointState>(); // gets the maps node and it's cornates 
    foreach (PointState dot in tempPoint) // instataties co-rndaates into the dictionary 
    {
        Vector3 key = new Vector3(Mathf.Round(dot.coord.x), Mathf.Round(dot.coord.y), Mathf.Round(dot.coord.z));
        map[key] = dot;
    }

    Dictionary<Vector3, CoNode> nodes = new Dictionary<Vector3, CoNode>();  // used to crea the path from start point to the tower
    List<CoNode> open = new List<CoNode>();
    HashSet<Vector3> closed = new HashSet<Vector3>();
    
    
    Vector3 startKey = new Vector3(Mathf.Round(curPos.x), Mathf.Round(curPos.y), Mathf.Round(curPos.z)); // starting point of the path 
    CoNode firstN = new CoNode(startKey); // adds vector to the first node calculate distnace to centre
    firstN.gCost = 0;
    firstN.hCost = calDistOfTower(startKey, Tower); // calculates the h cost finding the shortest distance between the two points 
    nodes[startKey] = firstN;
    open.Add(firstN);

    CoNode goalN = null;
    Vector3 roundedTower = new Vector3(Mathf.Round(Tower.x), Mathf.Round(Tower.y), Mathf.Round(Tower.z));// tower co-rd

    while (open.Count > 0) // will loop until there are no more nodes in the open set meaning the fucntion has l
    {
       // Debug.Log(open.Count);
        CoNode cur = open.OrderBy(n => n.fCost).First();

        open.Remove(cur);
        closed.Add(cur.coord);

        if (cur.coord == roundedTower)
        {
            goalN = cur; // if the currentnode has reached the goal Neighbour which is the tower loop can break 
          //  Debug.Log("path is there");
            break;
        }

        foreach (Vector3 Npos in getNeightbours(cur.coord))
        {
           
            if (!map.TryGetValue(Npos, out PointState neighborPoint)) // moves to next iteration if map is out of bounds or the pointstate co-nrdate is is not an enemy node
            {
               // Debug.Log("nope"); 
                continue;
            }

            if (closed.Contains(Npos) || (neighborPoint.state != States.Enemy && Npos != roundedTower))
            {
              //  Debug.Log("no states");
                continue;
            }
            
            int tentG = cur.gCost + 1;
            CoNode neight;

            if (!nodes.TryGetValue(Npos, out neight)) //  finds the distance of the nighbour node to calculate the h and g cost
            {
                neight = new CoNode(Npos);
                nodes[Npos] = neight;
                neight.gCost = tentG;
                neight.hCost = calDistOfTower(Npos, Tower);
                neight.parent = cur;
                open.Add(neight);

            }

            else if (tentG < neight.gCost)  // sets neigbours h and g to tentative g if it is less
            {
                neight.gCost = tentG;
                neight.hCost = calDistOfTower(Npos, Tower);
                neight.parent = cur;
            }
        }
    }
        if (goalN != null) // genrates the linked list from the tower to the start then revereses it so the start can be the first node and the tower can be the last node
        {

            path = new List<CoNode>();
            CoNode cur = goalN;

            while (cur != null)
            {
                path.Add(cur);
                cur = cur.parent;
            }

            path.Reverse();

            for (int i = 0; i < path.Count - 1; i++)
            {
                path[i].next = path[i + 1];
            }

            CurPoint = path[0];
            hasPath = true;


        }

        else
        {
          //  Debug.Log("Path not Found");
            hasPath = false;


        }
        
    


    }

    List<Vector3> getNeightbours(Vector3 pos) { // gets neighbours surrounding the cureent postion 


       List<Vector3> neighbors = new List<Vector3>();
    for (int zOff = -1; zOff <= 1; zOff++) {
        for (int xOff = -1; xOff <= 1; xOff++) {
            if (xOff == 0 && zOff == 0) continue;
            neighbors.Add(new Vector3(pos.x + xOff, pos.y, pos.z + zOff));
        }
    }
    return neighbors;
    }

    int calDistOfTower(Vector3 Start, Vector3 end) // calcualtes distance
    {
        return Mathf.Abs((int)Start.x - (int)end.x) + Mathf.Abs((int)Start.z - (int)end.z);
    }

    void MovePath() // moves the enemy to the tower using the linkedlist
    {
        if (CurPoint != null)
        {
            transform.LookAt(CurPoint.coord);
            transform.position = Vector3.MoveTowards(transform.position, CurPoint.coord, speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, CurPoint.coord) < 0.1f)
            {

                if (CurPoint.next != null)
                {
                    CurPoint = CurPoint.next;
                }

                else
                {
                    Destroy(gameObject); // once at final node enemy is destoryed
                  //  Debug.Log("tower Reached");
                }
                
            }


            
        }
    }


    void Update()
    {
        if (hasPath)
        {
            MovePath();
        }

    }

}
