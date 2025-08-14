using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;


public class CoNode
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

    PointState[] points;
    Vector3 Tower;

    Vector3 curPos;
    bool hasPath = false;

    CoNode CurPoint;

    List<CoNode> path;

    float speed;

    public void intPathing(PointState[] points, Vector3 Tower, Vector3 curPos, float spe)
    {
        this.points = points;
        this.Tower = Tower;
        this.curPos = curPos;
        speed = spe;
        MakePath();


    }

    public void StopMoving()
    {
        hasPath = false;
    }

    void MakePath()
    {

        PointState[] tempPoint = points;

        if (points == null)
        {

            Debug.Log("where the fuck is it bitch");
           
        }

        Dictionary<Vector3, PointState> map = new Dictionary<Vector3, PointState>();
        foreach (PointState dot in tempPoint)
        {
            map[dot.coord] = dot;
        }

        Dictionary<Vector3, CoNode> nodes = new Dictionary<Vector3, CoNode>();
        List<CoNode> open = new List<CoNode>();
        HashSet<Vector3> closed = new HashSet<Vector3>();

        CoNode firstN = new CoNode(curPos);
        firstN.gCost = 0;
        firstN.hCost = calDistOfTower(curPos, Tower);
        nodes[curPos] = firstN;
        open.Add(firstN);

        CoNode goalN = null;

        while (open.Count > 0)
        {
            CoNode cur = open.OrderBy(n => n.fCost).First();

            open.Remove(cur);
            closed.Add(cur.coord);


            if (cur.coord == Tower)
            {
                goalN = cur;
                break;
            }

            foreach (Vector3 Npos in getNeightbours(cur.coord))
            {
                if (!map.TryGetValue(Npos, out PointState neighborPoint))
                {
                    continue;
                }

                if (closed.Contains(Npos) || (neighborPoint.state != States.Enemy && Npos != Tower))
                {
                    continue;
                }
                int tentG = cur.gCost + 1;
                CoNode neight;

                if (!nodes.TryGetValue(Npos, out neight))
                {
                    neight = new CoNode(Npos);
                    nodes[Npos] = neight;
                    neight.gCost = tentG;
                    neight.hCost = calDistOfTower(Npos, Tower);
                    neight.parent = cur;
                    open.Add(neight);

                }

                else if (tentG < neight.gCost)
                {
                    neight.gCost = tentG;
                    neight.hCost = calDistOfTower(Npos, Tower);
                    neight.parent = cur;
                }


            }


        }

        if (goalN != null)
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
            Debug.Log("Path not Found");
            hasPath = false;


        }
        
    


    }

    List<Vector3> getNeightbours(Vector3 pos) {


        return new List<Vector3>
        {
            new Vector3(pos.x +1, pos.y, pos.z),
            new Vector3(pos.x - 1, pos.y, pos.z),
            new Vector3(pos.x, pos.y, pos.z + 1),
            new Vector3(pos.x, pos.y, pos.z-1)
        };
    }

    int calDistOfTower(Vector3 Start, Vector3 end)
    {
        return Mathf.Abs((int)Start.x - (int)end.x) + Mathf.Abs((int)Start.z - (int)end.z);
    }

    void MovePath()
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
                    Debug.Log("tower Reached");
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

        else
        {
            Debug.Log("no path");
        }

    }

}
