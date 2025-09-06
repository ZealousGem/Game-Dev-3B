
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Unity.Mathematics;
using System.Collections;


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

    float Amount = 0;

    float currentAmount = 0;

    void OnEnable()
    {
         EventBus.Subscribe<AmountEvent>(getData);
    }

    void OnDisable()
    {
         EventBus.Unsubscribe<AmountEvent>(getData);
    }

    void getData(AmountEvent data)
    {
        currentAmount = data.changed;
       // Debug.Log(currentAmount);
    }

    void Start()
    {
        ori = new List<Vector3>();
        for (int i = 0; i < Towers.Length; i++)
        {

            TowerUI[i].sprite = Towers[i].TowerUI;
            ori.Add(TowerUI[i].rectTransform.anchoredPosition);

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
                    if (currentAmount >= Amount)
                    {
                          tempTower.color = Color.green;
                    }
                  
                   
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
            tempTower = null;
            //  CaermaMovemtn.NoInv = true;
            //  List<GameObject> tempIamge = new List<GameObject>(); 

            for (int i = 0; i < Towers.Length; i++)
            {
                if (ob == TowerUI[i].gameObject)
                {
                    if (currentAmount >= Towers[i].reqAmount)
                    {
                        tempTower = ob.GetComponent<Image>();
                        it = ori[i];
                        newTower = Towers[i];
                        //  Debug.Log(it);
                        tempTower.raycastTarget = false;
                        Amount = Towers[i].reqAmount;
                        break;
                    }

                    else
                    {
                        StartCoroutine(NotEnoughMoney(TowerUI[i]));
                    }
                    // tempIamge.Add(item[i].gameObject);



                }
      
            }

            // Debug.Log(it);


        }
        //  throw new System.NotImplementedException();
    }

    IEnumerator NotEnoughMoney(Image tower)
    {
        tower.color = Color.red;
        yield return new WaitForSeconds(0.5f);
        tower.color = Color.white;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (tempTower != null)
        {

            // Cast a ray from the camera to the mouse position in the 3D world
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Check if the ray hits a collider
            if (Physics.Raycast(ray, out hit) && hit.transform.CompareTag("island") && currentAmount >= Amount)
            {
                // If the ray hits, check if the object has the "island" tag


                // Instantiate the tower prefab at the hit position
                GameObject tempObj = newTower.Prefab;
                Vector3 coord = new Vector3(hit.point.x, islandHeight, hit.point.z);
                Instantiate(tempObj, coord, quaternion.identity);
                GameManagerEvent loseMoney = new GameManagerEvent(Amount, StatsChange.MoneyLost);
                EventBus.Act(loseMoney);


            }

            tempTower.color = Color.white;
            tempTower.rectTransform.anchoredPosition = it;
            Amount = 0;
            //  Debug.Log(it);
            tempTower.raycastTarget = true;
            tempTower = null;


        }

    }

}
