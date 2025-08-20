using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Weaponary : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float Health = 100f;

    public float Speed = 10f;

    public float MaxcoolDown = 3f;

    float coolDown = 0f;

    public GameObject Projectile;

    public List<Transform> locations;

    List<GameObject> targets = new List<GameObject>();

    void Awake()
    {
        EventBus.Subscribe<DamageObjectEvent>(getDamage);
    }

    void OnDisable()
    {
          EventBus.Unsubscribe<DamageObjectEvent>(getDamage);
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


                    Vector3 direction = (currentTarget.transform.position - locations[i].position).normalized;
                    r.AddForce(direction * Speed, ForceMode.Impulse);
                }

                coolDown = 0f;
            }
        }
    }

    void Update()
    {
        if (targets.Count > 0)
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
            if (Health <= 0)
            {
                KillTower();
            }
        }

      
       
    }

    public void KillTower()
    {
        Destroy(this.gameObject);
        Debug.Log("death"); 
    }
}
