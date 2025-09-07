using UnityEngine;
using System.Collections.Generic;

public class AttackDefenceTowers : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created


     public float Speed = 10f; // speed projectile will go

    public float MaxcoolDown = 3f; // max cooldown of weapon

    float coolDown = 0f;

    float Damage; // damage the projectile or bomb will inflict 

    public GameObject Projectile; // the bomb enemy will shoot 

    public List<Transform> locations; // where projectile will spawn 

    List<GameObject> targets = new List<GameObject>(); // target the enemy will be aiming 

    bool foundEnemy = false;


    public void getEnemies(List<GameObject> towers, float _Damage) // allows enemy script to access variables if enemy has been found allowing script to shoot porjectile at the target 
    {
        targets = towers;
        foundEnemy = true;
        Damage = _Damage;
    }

    void NoMoreEnemies() // allow enemy script to reomve target tower if there are no more enemies in view 
    {

        foundEnemy = false;
        ChangeStateEvent changeStateEvent = new ChangeStateEvent(gameObject.GetInstanceID(), targets);
        EventBus.Act(changeStateEvent);
       

    }

    void shootTower() // spawns projectile and laucnhes it at the defence tower 
    {
        if (targets.Count > 0)
        {
            GameObject currentTarget = targets[0]; // sets current target 


            if (currentTarget == null)
            {
                targets.RemoveAt(0);
                return;
            }

            transform.LookAt(currentTarget.transform.position); // aims at the tower

            if (coolDown >= MaxcoolDown) // cooldown so enemy can shoot brief periods instead of constant
            {
                for (int i = 0; i < locations.Count; i++)
                {

                    
                    GameObject temp = Instantiate(Projectile, locations[i].position, locations[i].rotation, gameObject.transform);
                    Rigidbody r = temp.GetComponent<Rigidbody>();
                    if (temp.GetComponent<Bombs>()) // sets the projectiles damagae at the tower 
                    {
                        Bombs b = temp.GetComponent<Bombs>();
                        b.Damage = Damage; 
                    }
                    Vector3 Cannon = new Vector3(currentTarget.transform.position.x, currentTarget.transform.position.y + 1, currentTarget.transform.position.z);
                    Vector3 direction = (Cannon - locations[i].position).normalized;
                    r.AddForce(direction * Speed, ForceMode.Impulse); //adds the force to create speed to the projectile once spawned 
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
