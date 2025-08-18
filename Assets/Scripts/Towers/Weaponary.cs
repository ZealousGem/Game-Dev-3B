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

    bool foundTarget;

    GameObject target;

    Rigidbody r;

   

    void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Enemy"))
        {
            foundTarget = true;
            target = other.gameObject;
            Debug.Log("Enemy Found");
        }
       
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        { 
        foundTarget = false;
         Debug.Log("Enemy lost");
        target = null;
        }
        
    }

    void ShootEnemy()
    {
        if (target != null)
        {
            transform.LookAt(target.transform.position);
            if (coolDown >= MaxcoolDown)
            {
                for (int i = 0; i < locations.Count; i++)
                {
                    GameObject temp = Instantiate(Projectile, locations[i].position, locations[i].rotation);
                    r = temp.GetComponent<Rigidbody>();
                    r.AddForce((target.transform.position - locations[i].position).normalized * Speed, ForceMode.Impulse);
                }

                coolDown = 0f;
                
            }
        }
    }

    void Update()
    {
        if (foundTarget)
        {
            coolDown += Time.deltaTime;
            ShootEnemy();
        }
    }
}
