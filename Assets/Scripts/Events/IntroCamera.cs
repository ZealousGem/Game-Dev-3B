using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroCamera : MonoBehaviour
{
    // Start is called before the first frame update

    public Transform pos1; // points camera will move it
    public Transform pos2;

    public float Speed = 1f;
    bool MovingtoPos = true; // bool to determine if camera has reached one of the points
    // Update is called once per frame
    void LateUpdate()
    {
        MoveCamera();
    }

    void MoveCamera()
    {
        if (MovingtoPos) // if it is true means camera has reached one of the points and must move to the other point
        {
            gameObject.transform.position = Vector3.MoveTowards(transform.position, pos2.transform.position, Speed * Time.deltaTime); // will move unitl camera has reached point
            if (Vector3.Distance(transform.position, pos2.position) < 0.1f)
            {
               MovingtoPos=false;
            }
        }

        else
        {
            gameObject.transform.position = Vector3.MoveTowards(transform.position, pos1.transform.position, Speed * Time.deltaTime); // will move unitl camera has reached point
            if (Vector3.Distance(transform.position, pos1.position) < 0.1f)
            {
                MovingtoPos=true;
            }
        }
      

      
    }
}