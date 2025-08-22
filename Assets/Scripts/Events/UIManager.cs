using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class UIManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public GameObject TowerUI;

    bool Displayed = false;

    public TMP_Text GoldUI;

    public Image TowerHealth;

    float MaxTowerHealth = 200f;


    void OnEnable()
    {
        EventBus.Subscribe<AmountEvent>(getData);
        EventBus.Subscribe<GameManagerEvent>(getDataUI);
    }

    void OnDisable()
    {
        EventBus.Unsubscribe<AmountEvent>(getData);
        EventBus.Unsubscribe<GameManagerEvent>(getDataUI); 
    }

    void getData(AmountEvent data)
    {
        ChangeGoldUI((int)data.changed);
    }

    void getDataUI(GameManagerEvent data)
    {
        if (data.type == StatsChange.HealthUI)
        {
            UpdateHealthUI(data.changed);
        }
    }

    void UpdateHealthUI(float health)
    {
        TowerHealth.fillAmount = health / MaxTowerHealth;
        Debug.Log("UI Health" + TowerHealth.fillAmount);
    }

    void Start()
    {
        TowerUI.SetActive(false);
        TowerHealth.fillAmount = 1f;
        
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

    void ChangeGoldUI(int amount)
    {
        GoldUI.text = amount.ToString();
    }
    
}
