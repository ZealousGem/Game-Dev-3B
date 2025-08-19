using UnityEngine;

public class AttackTower : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    

    public void BlowUpTower(float Damage)
    {

        
        Destroy(this.gameObject);

    }

}
