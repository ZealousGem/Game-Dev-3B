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

    public TMP_Text TowerName; // name of the specific tower

    public GameObject radius; // vertex shader that will be dispalyed when turret is selected

    public TMP_Text info;

    GameManager MainTower; // contains main tower logic

    GameObject Curturret; // current tower selected 

      // stats that will be increased
    float Damage = 2f; 

    float CoolDown = 0.1f;

    float Health = 50f;

    int finalUpgrade = 3;

    // stats that will be increased

    int layerMask;

    string oldMeshname = "OldMesh";

    int ignoredLayer;

    int Gold = 0; // current gold player has


    // prices of each upgrade
    int HealthPrice = 50, DamagePrice = 60, SpeedPrice = 80,
     MainTowerDamagePrice = 250, MainTowerHealthPrice = 250;
     
     // prices of each upgrade

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
        //Debug.Log(Gold);
    }
    
     void getData(GameManagerEvent data)
    {
        if (StatsChange.hideUpgrades == data.type)
        {
            HideUI();
        }
    }

    void getTurret(GameObject turret) // checks if the turret exisit in the map 
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
    
    void SpawnRadius(Vector3 coord, bool cond) // moves the UI radius to current turret
    {
        if (coord == null)
        {

            return;
        }
     
        radius.transform.position = coord;
    
        radius.SetActive(cond);
    }

    void getMainTower(GameObject turret) // gets the main Tower
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

    void OpenTab(GameObject turret) // displays the upgrade UI and displays depending on the type of turret
    {
        info.text = "";
        if (turret == null)
        {
            Debug.Log("destroyed");
            return;
        }
        
         if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySound("but");  
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

    public void CloseTab() // Hides Upgrade UI
    {
        MainTowerButtons(false);
        TurretButtons(false);
        UpgradeUI.SetActive(false);

    }
    
    IEnumerator DispalyUI(string text) // displays text ui to tell player what they boughot, also tells them if they dont have ennough gold or has already upgraded the tower
    {
        info.text = text;
        yield return new WaitForSeconds(1f);
        info.text = "";
    }

    public void UpgradeTower(Weaponary Currentturret, GameObject newMesh, GameObject oldMesh, UpgradeType upgrade, int price) // upgrades the current defence tower
    {
        if (Currentturret == null) // checks if turret still exists 
        {
            CloseTab();
            radius.SetActive(false);
            Debug.Log("turret desotroyed or turret fully upgraded");
            return;

        }

        if (newMesh == null) // checks if turret has the upgrade already
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

        switch (upgrade) // upgrades turret based on which button the player has pressed 
        {
            case UpgradeType.Health: Currentturret.getHealth(Health); break;
            case UpgradeType.Damage: Currentturret.Damage += Damage; break;
            case UpgradeType.Speed: Currentturret.MaxcoolDown -= CoolDown; break;
        }

        if (Currentturret.counter <= finalUpgrade) // changes the turrests mesh 
        {
            StartCoroutine(DispalyUI(newMesh.name + " Upgraded"));
            Currentturret.counter++;
            oldMesh.SetActive(false);
            Destroy(oldMesh);
            newMesh.SetActive(true);
            newMesh.name = oldMeshname;

        }
        if (SoundManager.Instance != null)
         {
               SoundManager.Instance.PlaySound("gold");  
         }

        MainTower.DecreaseMoney(price); // decreases the gold 

    }
    
    void UpgradeMainTower(GameObject newMesh, GameObject oldMesh, UpgradeType upgrade, int price)
    {
        if (newMesh == null)  // checks if Main Tower still exists 
        {
            StartCoroutine(DispalyUI("Already Upgraded"));
            // Debug.Log("could not find new mesh");
            return;
        }

        if (oldMesh == null) // checks if MainTower has the upgrade already
        {
            StartCoroutine(DispalyUI("Already Upgraded"));
            //  Debug.Log("could not find old mesh");
            return;
        }


        // determines which upgrade player has selected and changes the tower mesh
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
          if (SoundManager.Instance != null)
         {
               SoundManager.Instance.PlaySound("gold");  
         }
          MainTower.DecreaseMoney(price); // decreases gold 
    }

    public void UpgradeDamage() // Upgrades Defence Turrets Damage
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

    public void UpgradeSpeed() // Upgrades Defence Turrets Speed
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

     public void UpgradeHealth() // Upgrades Defence Turrets Health and Heals it as well
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

    public void MainUpgradeTower() // upgrades Main towers Damage
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

    public void HealTower() // Upgrades Main Towers Health and Heals Main Tower
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

    void TurretButtons(bool cond) // displays the defence turret upgrade UI
    {
        foreach (GameObject but in Turretbuttons)
        {
            but.SetActive(cond);
        }

       
    }

    void MainTowerButtons(bool cond) // displays the Main Tower upgrade UI
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

    void HideUI() // closes all upgrade UI
    {
        CloseTab();
        radius.SetActive(false);
    }
    
    void FindTurret(RaycastHit hit) // finds the slected Turret in the map using the raycast system
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
