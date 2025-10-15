using UnityEngine;

public class TowerHealth : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject Parent; 

     void OnTriggerEnter(Collider other) // if bomb has collided with the tower, the damaged will be migrated to the Weaponary Script 
    {
        if (other.CompareTag("EnemyBomb") && other != null)
        {
            Bombs bombs = other.GetComponent<Bombs>();

            if (bombs != null && Parent != null)
            {
            DamageObjectEvent dam = new DamageObjectEvent(Parent.GetInstanceID(), bombs.Damage);
            
            EventBus.Act(dam);
            
            
            Destroy(other.gameObject); // destorys the bomb colliding with the tower 
            }
           
        }
    }
}
