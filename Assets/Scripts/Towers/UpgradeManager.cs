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

    public GameManager MainTower;

    float Damage = 2f;

    float CoolDown = 0.5f;

    float Health = 50f;

    int finalUpgrade = 3;

    string oldMeshname = "OldMesh";

    int Gold = 0;

    int HealthPrice = 50, DamagePrice = 60, SpeedPrice = 80,
     MainTowerDamagePrice = 250, MainTowerHealthPrice = 250;

    GameObject fullUpgradeMesh;

    GameObject currentTurret;

    void getTurret(GameObject turret)
    {
        if (turret == null)
        {
            Debug.Log("destroyed");
            return;
        }


        currentTurret = turret;
        TowerName.text = currentTurret.name.Replace("(Clone)", "");
        fullUpgradeMesh = currentTurret.transform.Find("full")?.gameObject;

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

    public void UpgradeTower(Weaponary Currentturret, GameObject newMesh, GameObject oldMesh, UpgradeType upgrade)
    {
        if (Currentturret == null || fullUpgradeMesh == null || newMesh == null || oldMesh == null || newMesh.name == oldMesh.name)
        {
            Debug.Log("turret desotroyed or turret fully upgraded");
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
            newMesh.SetActive(true);
            newMesh.name = oldMeshname;
            
        }

        else
        {
            oldMesh.SetActive(false);
            newMesh.SetActive(false);
            fullUpgradeMesh.SetActive(true);
        }


    }

    public void UpgradeDamage()
    {

        if (currentTurret == null || Gold < DamagePrice)
        {
            return;
        }

        Gold -= DamagePrice;
        UpgradeTower(currentTurret.GetComponent<Weaponary>(),
        currentTurret.transform.Find("Damage")?.gameObject,
        currentTurret.transform.Find(oldMeshname)?.gameObject,
        UpgradeType.Damage);

    }

    public void UpgradeSpeed()
    {
        if (currentTurret == null || Gold < SpeedPrice)
        {
            Debug.Log("turret destroyed");
            return;
        }

        Gold -= SpeedPrice;
        UpgradeTower(currentTurret.GetComponent<Weaponary>(),
        currentTurret.transform.Find("Speed")?.gameObject,
        currentTurret.transform.Find(oldMeshname)?.gameObject,
        UpgradeType.Speed);

    }

     public void UpgradeHealth()
    {

        if (currentTurret == null || Gold < HealthPrice)
        {
            return;
        }

         Gold -= HealthPrice;
         UpgradeTower(currentTurret.GetComponent<Weaponary>(),
         currentTurret.transform.Find("Health")?.gameObject,
         currentTurret.transform.Find(oldMeshname)?.gameObject,
         UpgradeType.Health);

    }

    public void MainUpgradeTower()
    {
        if (currentTurret == null || Gold < MainTowerDamagePrice)
        {
            return;
        }


        Gold -= MainTowerDamagePrice;
    }

    public void HealTower()
    {
        if (currentTurret == null || Gold < MainTowerHealthPrice)
        {
            return;
        }

        Gold -= MainTowerHealthPrice;
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
        MainTowerButtons(false);
        TurretButtons(false);
        UpgradeUI.SetActive(false);
       
    }

    // Update is called once per frame

    public void OnPointerDown(PointerEventData eventData)
    {
      //  throw new System.NotImplementedException();
      if (eventData.button == PointerEventData.InputButton.Left)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
            if (hit.transform.CompareTag("DefenceTower") ||  hit.transform.CompareTag("Tower"))
            {
                GameObject ob = hit.collider.gameObject;
                OpenTab(ob);
            }
            
            }
            

        }
    }
}
