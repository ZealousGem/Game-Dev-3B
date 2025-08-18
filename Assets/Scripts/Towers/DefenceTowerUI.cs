
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Unity.Mathematics;


public class DefenceTowerUI : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public Tower[] Towers;

    public Image[] TowerUI;

    Image tempTower;

    Tower newTower;

    float islandHeight = 1.1f;

    List<Vector3> ori;

    Vector3 it;

    void Start()
    {
        ori = new List<Vector3>();
        for (int i = 0; i < Towers.Length; i++)
        {

            TowerUI[i].sprite = Towers[i].TowerUI;
            ori.Add(TowerUI[i].transform.position);

        }
    }

    // Update is called once per frame
    public void OnDrag(PointerEventData eventData)
    {
        //  throw new System.NotImplementedException();
        if (tempTower != null)
        {
            tempTower.transform.position = eventData.position;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                // If the ray hits, check if the object has the "island" tag
                if (hit.transform.CompareTag("island"))
                {
                    tempTower.color = Color.green;
                    Debug.Log("hit");
                }

                else if (!hit.transform.CompareTag("island"))
                {
                    tempTower.color = Color.red;
                }

            }

            else
            {
                 tempTower.color = Color.red;
            }



        }
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            GameObject ob = eventData.pointerCurrentRaycast.gameObject;
            //  CaermaMovemtn.NoInv = true;
            //  List<GameObject> tempIamge = new List<GameObject>(); 
            
            for (int i = 0; i < Towers.Length; i++)
            {
                if (ob == TowerUI[i].gameObject)
                {
                    // tempIamge.Add(item[i].gameObject);
                    tempTower = ob.GetComponent<Image>();
                    it = ori[i];
                    newTower = Towers[i];
                   
                    tempTower.raycastTarget = false;
                    
                    break;
                }
            }

           // Debug.Log(it);


        }
        //  throw new System.NotImplementedException();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (tempTower != null)
        {
            Vector3 temp = it;
            // Cast a ray from the camera to the mouse position in the 3D world
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Check if the ray hits a collider
            if (Physics.Raycast(ray, out hit) && hit.transform.CompareTag("island"))
            {
                // If the ray hits, check if the object has the "island" tag
                

                    // Instantiate the tower prefab at the hit position
                    GameObject tempObj = newTower.Prefab;
                    Vector3 coord = new Vector3(hit.point.x, islandHeight, hit.point.z);
                    Instantiate(tempObj, coord, quaternion.identity);
                
            }

            tempTower.color = Color.white;
            if (it != temp)
            {
                tempTower.transform.position = temp;
                Debug.Log("not good");
            }

            else
            {
                tempTower.transform.position = it;
                  Debug.Log("good");
            }
          
          //  Debug.Log(it);
            tempTower.raycastTarget = true;
            tempTower = null;

          
        }

    }

}
