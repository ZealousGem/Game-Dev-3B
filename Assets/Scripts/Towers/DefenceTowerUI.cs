
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

    public Tower[] Towers; // Towers that player can spawn

    public Image[] TowerUI;  // UI of the Tower the player can spawn 

    Image tempTower;

    Tower newTower;

    float islandHeight = 1.1f;  // this is so the Tower does not sink into the map and make sure it is above

    List<Vector3> ori; // to return the ui image back to it's original location 

    Vector3 it;

    float Amount = 0; // amount required to buy tower

    float currentAmount = 0;  // curret amount player has 

    bool isPasued = false;

    void OnEnable()
    {
        EventBus.Subscribe<AmountEvent>(getData);
         EventBus.Subscribe<EndGameEvent>(getData2); 
    }

    void OnDisable()
    {
        EventBus.Unsubscribe<AmountEvent>(getData);
          EventBus.Unsubscribe<EndGameEvent>(getData2);
    }

    void getData2(EndGameEvent data)
    {
        if (data.type == StatsChange.PausedGame)
        {
            isPasued = true;

        }

        else if (data.type == StatsChange.UnPausedGame)
        {
            isPasued = false;
        }
        
     //    Debug.Log(isPasued);
    }

    void getData(AmountEvent data) // gets the amount of money player has
    {
        currentAmount = data.changed;
      //  Debug.Log(currentAmount);
    }

    void Start()
    {
        ori = new List<Vector3>();  // sets the original location of the image
        for (int i = 0; i < Towers.Length; i++)  // instaties the TowerUI Sprite from the Sprite in the scriptable object
        {

            TowerUI[i].sprite = Towers[i].TowerUI;
            ori.Add(TowerUI[i].rectTransform.anchoredPosition);

        }
    }

    // Update is called once per frame
    public void OnDrag(PointerEventData eventData)
    {
        //  throw new System.NotImplementedException();
        if (tempTower != null && !isPasued)
        {
            tempTower.transform.position = eventData.position; // will change the images position based on the players mouse co-orndaites

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit)) // use raycast to detect whether the player is placing the tower on the islands or the water
            {
                // If the ray hits, check if the object has the "island" tag
                if (hit.transform.CompareTag("island"))  // if it's casting  colliding the island mesh ui image will change to green , showing tower is placeable 
                {
                    if (currentAmount >= Amount)
                    {
                        tempTower.color = Color.green;
                    }


                }

                else if (!hit.transform.CompareTag("island")) // if it's not casting  colliding the island mesh ui image will change to red , showing tower is not placeable 
                {
                    tempTower.color = Color.red;
                }

            }

            else // if there are no raycast hit then colour will change to red 
            {
                tempTower.color = Color.red;
            }



        }
        
       
    }


    public void OnPointerDown(PointerEventData eventData) // used PointerEventData so player can drag the TowerImage then place it on the map 
    {

        if (eventData.button == PointerEventData.InputButton.Left && !isPasued)
        {
            GameObject ob = eventData.pointerCurrentRaycast.gameObject; // gets the mouses location youusing the pointer event
            tempTower = null;
            //  CaermaMovemtn.NoInv = true;
            //  List<GameObject> tempIamge = new List<GameObject>(); 

            for (int i = 0; i < Towers.Length; i++)
            {
                if (ob == TowerUI[i].gameObject)
                {
                    if (currentAmount >= Towers[i].reqAmount) // checks if player has enough money to buy the turret
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
                        StartCoroutine(NotEnoughMoney(TowerUI[i]));  // if not image will be red and not draggable 
                    }
                    // tempIamge.Add(item[i].gameObject);



                }

            }

            // Debug.Log(it);


        }

        else
        {
            for (int i = 0; i < Towers.Length; i++)
            {
                TowerUI[i].color = Color.white;  
            }
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
            if (Physics.Raycast(ray, out hit) && hit.transform.CompareTag("island") && currentAmount >= Amount) // checks if the raycast has hit the island mesh if not then the image will just reset and not instantiate the object
            {
                


                
                GameObject tempObj = newTower.Prefab; // if the ray cast has hit the island mesh and the player has enough money to purchase then the tower will spawn on the mouse location
                Vector3 coord = new Vector3(hit.point.x, islandHeight, hit.point.z);
                Instantiate(tempObj, coord, quaternion.identity); 
                GameManagerEvent loseMoney = new GameManagerEvent(Amount, StatsChange.MoneyLost);// money will be lost once the turret has spawned  
                EventBus.Act(loseMoney);


            }

            tempTower.color = Color.white; // resets once mouse key has been let go
            tempTower.rectTransform.anchoredPosition = it;
            Amount = 0;
            //  Debug.Log(it);
            tempTower.raycastTarget = true;
            tempTower = null;


        }

        else if (isPasued && currentAmount > Amount && tempTower != null) { // will reset if the game is paused 
            tempTower.color = Color.white;
            tempTower.rectTransform.anchoredPosition = it;
            Amount = 0;
            //  Debug.Log(it);
            tempTower.raycastTarget = true;
            tempTower = null;
        }

    }

}
