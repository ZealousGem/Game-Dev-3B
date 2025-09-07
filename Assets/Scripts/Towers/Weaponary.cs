using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class Weaponary : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    // all the tower stats 
    public float Health = 100f;

    float maxHealth = 0f; 

    float currentHealth;

    public float Speed = 10f;

    public float MaxcoolDown = 3f;

    public float Damage = 20f;

    float coolDown = 0f;

    // all the tower stats 

    public GameObject Projectile; // bomb that tower will shoot 

    public List<Transform> locations; // location to where projectile will spawn 

    public Image HealthUI;

    public GameObject HealthCanvas;

    public GameObject Explosion; // explosion effect when tower health is at 0 

    bool IsOver = false;

    

    List<GameObject> targets = new List<GameObject>(); // list of targets that are within the towers radius 

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

        if (other.CompareTag("Enemy")) // if enemy is detected in the radius, enemy will be added to the list of targets 
        {

            GameObject target = other.gameObject;
            targets.Add(target);
            // Debug.Log("Enemy Found");
        }

    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy")) // if enemy is out of radius or is killed, enemy will be taking out of list of enemy target
        {

            //   Debug.Log("Enemy lost");
            targets.Remove(other.gameObject);
        }

    }

    void ShootEnemy() // similar to the enemy shooting script, activates if there is and enemiy in the list spawns projectile and adds froce to the target 
    {
        if (targets.Count > 0)
        {

            GameObject currentTarget = targets[0];


            if (currentTarget == null) // removes target if has been destoryed or moves to next one in the l;ist 
            {
                targets.RemoveAt(0);
                return;
            }

            transform.LookAt(currentTarget.transform.position); // aims at the enemy 

            if (coolDown >= MaxcoolDown) // will only spawn if cooldown is equal to max cool down
            {
                for (int i = 0; i < locations.Count; i++) // spawns the projectile and instatiate the damage on to the bomb then it adds force to the direction of the target 
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
    void EndTurret() // bool will set true of no more enemies in radius 
    {
        IsOver = true;
    }

    void Update() // cooldown made to make sure tower does not spam the projectile 
    {
        if (targets.Count > 0 && !IsOver)
        {
            coolDown += Time.deltaTime;
            ShootEnemy();
        }
    }
    
    void DecreaseHealth(float dam) // decreases towers health
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

    IEnumerator TowerUI() // healthUI changes once Health had decreased 
    {
        HealthCanvas.SetActive(true);
        HealthUI.fillAmount = Health / maxHealth;
        yield return new WaitForSeconds(1f);
        HealthCanvas.SetActive(false);
    }

    

    public void KillTower() // destorys Tower if health is 0 
    {
        Instantiate(Explosion, this.gameObject.transform.position, quaternion.identity);
        Destroy(this.gameObject);
        //Debug.Log("death"); 
    }
}
