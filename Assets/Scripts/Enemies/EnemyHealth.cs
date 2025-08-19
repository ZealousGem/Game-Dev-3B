using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public GameObject Parent; 

     void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bomb") && other != null)
        {
            Bombs bombs = other.GetComponent<Bombs>();

            if (bombs != null && Parent != null)
            {
            DamageObjectEvent dam = new DamageObjectEvent(Parent.GetInstanceID(), bombs.Damage);
            EventBus.Act(dam); 
            Destroy(other.gameObject); 
            }
           
        }
    }
    
}
