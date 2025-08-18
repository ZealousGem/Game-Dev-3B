using UnityEngine;

public class Bombs : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public float Damage = 30f;

    public float MaxCounter;

    float Counter = 0f;

    // Update is called once per frame
    void Update()
    {
        Counter += Time.deltaTime;

        if (Counter >= MaxCounter)
        {
            Destroy(gameObject);    
        }
    }
}
