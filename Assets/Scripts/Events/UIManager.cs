using UnityEngine;

public class UIManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public GameObject TowerUI;

    bool Displayed = false;


    void Start()
    {
        TowerUI.SetActive(false);
    }

    // Update is called once per frame

    public void ShowTowerUI()
    {
        if (Displayed)
        {

            TowerUI.SetActive(false);
            Displayed = false;

        }

        else
        {
            TowerUI.SetActive(true);
            Displayed = true;
        }
    }
    
}
