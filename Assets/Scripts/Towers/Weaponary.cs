using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class Weaponary : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float Health = 100f;

    float maxHealth = 0f; 

    float currentHealth;

    public float Speed = 10f;

    public float MaxcoolDown = 3f;

    public float Damage = 20f;

    float coolDown = 0f;

    public GameObject Projectile;

    public List<Transform> locations;

    public Image HealthUI;

    public GameObject HealthCanvas;

    bool IsOver = false;

    

    List<GameObject> targets = new List<GameObject>();

    void Awake()
    {
        EventBus.Subscribe<DamageObjectEvent>(getDamage);
        EventBus.Subscribe<EndGameEvent>(getEndDate);
        maxHealth = Health;
        HealthCanvas.SetActive(false);
    }

    void OnDisable()
    {
        EventBus.Unsubscribe<DamageObjectEvent>(getDamage);
        EventBus.Unsubscribe<EndGameEvent>(getEndDate);
    }

    void getEndDate(EndGameEvent data)
    {
         if (data.type == StatsChange.EndGame)
        {
            EndTurret();
        }
    }

     void getDamage(DamageObjectEvent data)
    {
        if (data.name == gameObject.GetInstanceID())
        {
            DecreaseHealth(data.Damage);  
        }
    }

    void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Enemy"))
        {

            GameObject target = other.gameObject;
            targets.Add(target);
            // Debug.Log("Enemy Found");
        }

    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {

            //   Debug.Log("Enemy lost");
            targets.Remove(other.gameObject);
        }

    }

    void ShootEnemy()
    {
        if (targets.Count > 0)
        {

            GameObject currentTarget = targets[0];


            if (currentTarget == null)
            {
                targets.RemoveAt(0);
                return;
            }

            transform.LookAt(currentTarget.transform.position);

            if (coolDown >= MaxcoolDown)
            {
                for (int i = 0; i < locations.Count; i++)
                {
                    GameObject temp = Instantiate(Projectile, locations[i].position, locations[i].rotation, gameObject.transform);
                    Rigidbody r = temp.GetComponent<Rigidbody>();
                     if (temp.GetComponent<Bombs>())
                    {
                        Bombs b = temp.GetComponent<Bombs>();
                        b.Damage = Damage; 
                    }

                    Vector3 direction = (currentTarget.transform.position - locations[i].position).normalized;
                    r.AddForce(direction * Speed, ForceMode.Impulse);
                }

                coolDown = 0f;
            }
        }
    }
    void EndTurret()
    {
        IsOver = true;
    }

    void Update()
    {
        if (targets.Count > 0 && !IsOver)
        {
            coolDown += Time.deltaTime;
            ShootEnemy();
        }
    }
    
    void DecreaseHealth(float dam)
    {

        if (Health > 0)
        {
            
         
            Health -= dam;
            StartCoroutine(TowerUI());
           
            if (Health <= 0)
            {
                KillTower();
            }
        }

      
       
    }

    IEnumerator TowerUI()
    {
        HealthCanvas.SetActive(true);
        HealthUI.fillAmount = Health / maxHealth;
        yield return new WaitForSeconds(1f);
        HealthCanvas.SetActive(false);
    }

    

    public void KillTower()
    {
        Destroy(this.gameObject);
        //Debug.Log("death"); 
    }
}
