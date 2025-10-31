using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public enum UpgradeType
{
    Health,

    Speed,

    Damage,

    MainTowerHealth,

    MainTowerDamage, 
}


public class UpgradeManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    public GameObject UpgradeUI;

    public List<GameObject> Turretbuttons;

    public List<GameObject> MainTowerButton;

    public TMP_Text TowerName;

    public GameObject radius;

    public TMP_Text info;

    public Material HealthText;

    GameManager MainTower;

    GameObject Curturret;

    float Damage = 2f;

    float CoolDown = 0.1f;

    float Health = 50f;

    int finalUpgrade = 3;

    int layerMask;

    string oldMeshname = "OldMesh";

    int ignoredLayer;

    int Gold = 0;

    int HealthPrice = 50, DamagePrice = 60, SpeedPrice = 80,
     MainTowerDamagePrice = 250, MainTowerHealthPrice = 250;

    GameObject currentTurret;

    void OnEnable()
    {
        EventBus.Subscribe<AmountEvent>(getData);
        EventBus.Subscribe<GameManagerEvent>(getData);
    }

    void OnDisable()
    {
        EventBus.Unsubscribe<AmountEvent>(getData);
        EventBus.Unsubscribe<GameManagerEvent>(getData);
    }

    void getData(AmountEvent data)
    {
        Gold = (int)data.changed;
        Debug.Log(Gold);
    }
    
     void getData(GameManagerEvent data)
    {
        if (StatsChange.hideUpgrades == data.type)
        {
            HideUI();
        }
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
        SpawnRadius(turret.transform.position, true);
    }

    void OpenTab(GameObject turret)
    {
        info.text = "";
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
    
    IEnumerator DispalyUI(string text)
    {
        info.text = text;
        yield return new WaitForSeconds(1f);
        info.text = "";
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
            StartCoroutine(DispalyUI("Already Upgraded"));
            // Debug.Log("could not find new mesh");
            return;
        }

        if (oldMesh == null)
        {
            StartCoroutine(DispalyUI("Already Upgraded"));
            //  Debug.Log("could not find old mesh");
            return;
        }

        switch (upgrade)
        {
            case UpgradeType.Health: Currentturret.getHealth(Health); break;
            case UpgradeType.Damage: Currentturret.Damage += Damage; break;
            case UpgradeType.Speed: Currentturret.MaxcoolDown -= CoolDown; break;
        }

        if (Currentturret.counter <= finalUpgrade)
        {
            StartCoroutine(DispalyUI(newMesh.name + " Upgraded"));
            Currentturret.counter++;
            oldMesh.SetActive(false);
            Destroy(oldMesh);
            newMesh.SetActive(true);
            newMesh.name = oldMeshname;

        }

        MainTower.DecreaseMoney(price);

    }
    
    void UpgradeMainTower(GameObject newMesh, GameObject oldMesh, UpgradeType upgrade, int price)
    {
        if (newMesh == null)
        {
            StartCoroutine(DispalyUI("Already Upgraded"));
            // Debug.Log("could not find new mesh");
            return;
        }

        if (oldMesh == null)
        {
            StartCoroutine(DispalyUI("Already Upgraded"));
            //  Debug.Log("could not find old mesh");
            return;
        }

        if (upgrade == UpgradeType.MainTowerDamage)
        {
            StartCoroutine(DispalyUI("Cannons Upgraded"));
            oldMesh.SetActive(false);
            Destroy(oldMesh);
            newMesh.SetActive(true);
        }

        else if (upgrade == UpgradeType.MainTowerHealth)
        {
            MainTower.MainTowerHealth = 400f;
            GameManagerEvent HealthUI = new GameManagerEvent(MainTower.MainTowerHealth, StatsChange.HealthUI);
            EventBus.Act(HealthUI);

            StartCoroutine(DispalyUI("Health Upgraded"));
            oldMesh.SetActive(false);
            Destroy(oldMesh);
            newMesh.SetActive(true);
        }
        
          MainTower.DecreaseMoney(price);
    }

    public void UpgradeDamage()
    {

        if (currentTurret == null)
        {
            return;
        }

        if (Gold < DamagePrice)
        {
             StartCoroutine(DispalyUI("Not Enough Gold"));
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
            return;
        }

        if (Gold < SpeedPrice)
        {
             StartCoroutine(DispalyUI("Not Enough Gold"));
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
            
            StartCoroutine(DispalyUI("Not Enough Gold"));
            return;
        }
       
         UpgradeTower(currentTurret.GetComponent<Weaponary>(),
         currentTurret.transform.Find("Health")?.gameObject,
         currentTurret.transform.Find(oldMeshname)?.gameObject,
         UpgradeType.Health, HealthPrice);

    }

    public void MainUpgradeTower()
    {
        if (currentTurret == null)
        {
            return;
        }

        if (Gold < MainTowerDamagePrice)
        {
            Debug.Log(Gold);
            StartCoroutine(DispalyUI("Not Enough Gold"));
            return;
        }

        UpgradeMainTower(currentTurret.transform.Find("BigCannons")?.gameObject,
        currentTurret.transform.Find("LittleCannons")?.gameObject,
        UpgradeType.MainTowerDamage, MainTowerDamagePrice);

      //  currentTurret.transform.Find("Health")?.gameObject;
      

    }

    public void HealTower()
    {
        if (currentTurret == null)
        {
           
            return;
        }

         if (Gold < MainTowerHealthPrice)
        {
            StartCoroutine(DispalyUI("Not Enough Gold"));
            return;
        }

        UpgradeMainTower(currentTurret.transform.Find("HealthTower")?.gameObject,
        currentTurret.transform.Find("Tower")?.gameObject,
        UpgradeType.MainTowerHealth, MainTowerHealthPrice);

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
        layerMask = ~(1 << ignoredLayer);
        MainTower = GetComponent<GameManager>();
        MainTowerButtons(false);
        TurretButtons(false);
        UpgradeUI.SetActive(false);
        Gold = MainTower.Money;

    }

    void HideUI()
    {
        CloseTab();
        radius.SetActive(false);
    }
    
    void FindTurret(RaycastHit hit)
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
            Curturret = ob;
            OpenTab(ob);
         }

        
            
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, float.MaxValue, layerMask))
            {
                FindTurret(hit);
            }

        }

        if (Curturret == null)
        {
            HideUI();
        }
    }

    // Update is called once per frame

    
}
