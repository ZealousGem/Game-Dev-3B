using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public enum UpgradeType
{
    Health,

    Speed,

    Damage,
}


public class UpgradeManager : MonoBehaviour, IPointerDownHandler
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    public GameObject UpgradeUI;

    public List<GameObject> Turretbuttons;

    public List<GameObject> MainTowerButton;

    public TMP_Text TowerName;

    public GameObject radius;

    GameManager MainTower;

    GameObject Curturret;

    float Damage = 2f;

    float CoolDown = 0.1f;

    float Health = 50f;

    int finalUpgrade = 3;

    int layerMask;

    string oldMeshname = "OldMesh";

  //   string TowerCannons = "LittleCannons";

    int ignoredLayer;

    int Gold = 0;

    int HealthPrice = 50, DamagePrice = 60, SpeedPrice = 80,
     MainTowerDamagePrice = 250, MainTowerHealthPrice = 250;

    GameObject currentTurret;

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
        Gold = (int)data.changed;
    }

    void getTurret(GameObject turret)
    {
        if (turret == null)
        {
            Debug.Log("destroyed");
            return;
        }


        currentTurret = turret;
        TowerName.text = currentTurret.name.Replace("(Clone)", "");
        SpawnRadius(turret.transform.position, true);


    }
    
    void SpawnRadius(Vector3 coord, bool cond)
    {
        if (coord == null)
        {

            return;
        }
     //   Vector3 newPosition = new Vector3(coord.x, 10.0f, coord.z);
        radius.transform.position = coord;
    
    radius.SetActive(cond);
    }

    void getMainTower(GameObject turret)
    {
         if (turret == null)
        {
            Debug.Log("destroyed");
            return;
        }


        currentTurret = turret;
        TowerName.text = currentTurret.name.Replace("(Clone)", "");
    }

    void OpenTab(GameObject turret)
    {
        if (turret == null)
        {
            Debug.Log("destroyed");
            return;
        }
        
        
        UpgradeUI.SetActive(true);


        if (turret.CompareTag("DefenceTower"))
        {
            getTurret(turret);
            MainTowerButtons(false);
            TurretButtons(true);
        }

        else if (turret.CompareTag("Tower"))
        {
            getMainTower(turret);
            MainTowerButtons(true);
            TurretButtons(false);
        }
    }
    
    public void CloseTab()
    {
        MainTowerButtons(false);
        TurretButtons(false);
        UpgradeUI.SetActive(false);
        
    }

    public void UpgradeTower(Weaponary Currentturret, GameObject newMesh, GameObject oldMesh, UpgradeType upgrade, int price)
    {
        if (Currentturret == null)
        {
            CloseTab();
            radius.SetActive(false);
            Debug.Log("turret desotroyed or turret fully upgraded");
            return;

        }

        if (newMesh == null)
        {
            Debug.Log("could not find new mesh");
            return;
        }

        if (oldMesh == null)
        {
             Debug.Log("could not find old mesh");
            return;
        }

        switch (upgrade)
        {
            case UpgradeType.Health: Currentturret.getHealth(Health); break;
            case UpgradeType.Damage:  Currentturret.Damage += Damage;break;
            case UpgradeType.Speed:  Currentturret.MaxcoolDown -= CoolDown;break;
        }

        if (Currentturret.counter <= finalUpgrade)
        {
            Currentturret.counter++;
            oldMesh.SetActive(false);
            Destroy(oldMesh);
            newMesh.SetActive(true);
            newMesh.name = oldMeshname;
            
        }

         MainTower.DecreaseMoney(price);

    }

    public void UpgradeDamage()
    {

        if (currentTurret == null)
        {
            CloseTab();
            radius.SetActive(false);
            return;
        }

        if (Gold < DamagePrice)
        {
             return;
        }

       
        UpgradeTower(currentTurret.GetComponent<Weaponary>(),
        currentTurret.transform.Find("Damage")?.gameObject,
        currentTurret.transform.Find(oldMeshname)?.gameObject,
        UpgradeType.Damage, DamagePrice);

    }

    public void UpgradeSpeed()
    {
        if (currentTurret == null)
        {
            CloseTab();
            radius.SetActive(false);
            return;
        }

        if (Gold < SpeedPrice)
        {
             return;
        }

        
       
        UpgradeTower(currentTurret.GetComponent<Weaponary>(),
        currentTurret.transform.Find("Speed")?.gameObject,
        currentTurret.transform.Find(oldMeshname)?.gameObject,
        UpgradeType.Speed, SpeedPrice);

    }

     public void UpgradeHealth()
    {

        if (currentTurret == null)
        {
        
            return;
        }


        if (Gold < HealthPrice)
        {
            return;
        }
       
         UpgradeTower(currentTurret.GetComponent<Weaponary>(),
         currentTurret.transform.Find("Health")?.gameObject,
         currentTurret.transform.Find(oldMeshname)?.gameObject,
         UpgradeType.Health, HealthPrice);

    }

    public void MainUpgradeTower()
    {
        if (currentTurret == null || Gold < MainTowerDamagePrice)
        {
            return;
        }


       
       MainTower.DecreaseMoney(MainTowerDamagePrice);

    }

    public void HealTower()
    {
        if (currentTurret == null || Gold < MainTowerHealthPrice)
        {
            return;
        }

        MainTower.DecreaseMoney(MainTowerDamagePrice);
        MainTower.MainTowerHealth = 400f;
        GameManagerEvent HealthUI = new GameManagerEvent(MainTower.MainTowerHealth, StatsChange.HealthUI);
        EventBus.Act(HealthUI);

    }

    void TurretButtons(bool cond)
    {
        foreach (GameObject but in Turretbuttons)
        {
            but.SetActive(cond);
        }

       
    }

    void MainTowerButtons(bool cond)
    {
        foreach (GameObject but in MainTowerButton)
        {
            but.SetActive(cond);
        }
    }

    void Start()
    {
        ignoredLayer = LayerMask.NameToLayer("islands");
         layerMask = ~ (1 << ignoredLayer);
        MainTower = GetComponent<GameManager>();
        MainTowerButtons(false);
        TurretButtons(false);
        UpgradeUI.SetActive(false);

    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, float.MaxValue, layerMask))
            {
                if (hit.transform.CompareTag("DefenceTower"))
                {
                    GameObject ob = hit.collider.gameObject;
                    TowerHealth tower = ob.GetComponent<TowerHealth>();
                    Curturret = tower.Parent;
                    //  Debug.Log("clicked");
                    OpenTab(tower.Parent);
                }

                else if (hit.transform.CompareTag("Tower"))
                {
                    GameObject ob = hit.collider.gameObject;
                    OpenTab(ob);
                }

                else
                {
                    CloseTab();
                    radius.SetActive(false);
                }

            }

            else
            {
                radius.SetActive(false);
            }


        }

        if (Curturret == null)
        {
              CloseTab();
              radius.SetActive(false);
        }
    }

    // Update is called once per frame

    public void OnPointerDown(PointerEventData eventData)
    {
      //  throw new System.NotImplementedException();
      
    }
}
