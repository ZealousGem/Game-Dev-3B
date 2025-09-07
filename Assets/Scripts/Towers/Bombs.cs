using UnityEngine;

public class Bombs : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

     [HideInInspector]
    public float Damage = 30f; // amount of Damage will contain to inflict turrets or enemies

    public float MaxCounter;

    float Counter = 0f;

    // Update is called once per frame
    void Update()
    {
        Counter += Time.deltaTime; // once counter has been reached bomb will despawn to save peformance, this is only done if the turret or enemy miises their shot 

        if (Counter >= MaxCounter)
        {
            Destroy(gameObject);    
        }
    }
}
