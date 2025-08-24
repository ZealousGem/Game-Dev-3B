using Unity.Mathematics;
using UnityEngine;

public class AttackTower : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public GameObject Explosion;

    public void BlowUpTower(float Damage)
    {

        GameManagerEvent decreaseHealth = new GameManagerEvent(Damage, StatsChange.Health);
        EventBus.Act(decreaseHealth);
        Instantiate(Explosion, this.gameObject.transform.position, quaternion.identity);
        Destroy(this.gameObject);

    }

}
