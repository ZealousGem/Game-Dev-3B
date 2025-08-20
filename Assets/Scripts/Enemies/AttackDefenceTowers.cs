using UnityEngine;
using System.Collections.Generic;

public class AttackDefenceTowers : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created


     public float Speed = 10f;

    public float MaxcoolDown = 3f;

    float coolDown = 0f;

    public GameObject Projectile;

    public List<Transform> locations;

    List<GameObject> targets = new List<GameObject>();

    bool foundEnemy = false;


    public void getEnemies(List<GameObject> towers)
    {
        targets = towers;
        foundEnemy = true; 
    }

    void NoMoreEnemies()
    {

        foundEnemy = false;
        ChangeStateEvent changeStateEvent = new ChangeStateEvent(gameObject.GetInstanceID(), targets);
        EventBus.Act(changeStateEvent);
       

    }

    void shootTower()
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

                    Vector3 Cannon = new Vector3(currentTarget.transform.position.x, currentTarget.transform.position.y + 1, currentTarget.transform.position.z);
                    Vector3 direction = (Cannon - locations[i].position).normalized;
                    r.AddForce(direction * Speed, ForceMode.Impulse);
                }

                coolDown = 0f;
            }
        }

        else if (targets.Count == 0)
        {
            NoMoreEnemies();
        }
            
           
    }

    // Update is called once per frame
    void Update()
    {
        if (foundEnemy)
        {
            coolDown += Time.deltaTime;
            shootTower();
        }
    }
}
