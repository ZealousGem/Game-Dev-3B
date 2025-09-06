using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting.Antlr3.Runtime.Misc;


public class UIManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public GameObject TowerUI;

    bool Displayed = false;

    public TMP_Text GoldUI;

    public Image TowerHealth;

    public GameObject inGameUI;

    public GameObject GameOverUI;

    public TMP_Text waveUI;

    int WaveCounter = 1;

    int amountUI;

    float MaxTowerHealth = 200f;


    void OnEnable()
    {
        EventBus.Subscribe<AmountEvent>(getData);
        EventBus.Subscribe<GameManagerEvent>(getDataUI);
        EventBus.Subscribe<EndGameEvent>(getEndDate);
    }

    void OnDisable()
    {
        EventBus.Unsubscribe<AmountEvent>(getData);
        EventBus.Unsubscribe<GameManagerEvent>(getDataUI); 
        EventBus.Unsubscribe<EndGameEvent>(getEndDate);
    }

    void getEndDate(EndGameEvent data)
    {

        switch (data.type)
        {
            case StatsChange.EndGame: EndGame(); break;
            case StatsChange.ChangeWave: ChangeWaveCounter(); break;    
        }
        
    }

    void ChangeWaveCounter()
    {
        WaveCounter += 1;
        waveUI.text = WaveCounter.ToString();
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("Game");
    }

    void EndGame()
    {
        inGameUI.SetActive(false);
        GameOverUI.SetActive(true);
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
       // Debug.Log("UI Health" + TowerHealth.fillAmount);
    }

    void Start()
    {
        TowerUI.SetActive(false);
        GameOverUI.SetActive(false);
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
            AmountEvent money = new AmountEvent(amountUI);
            EventBus.Act(money);

        }
    }

    void ChangeGoldUI(int amount)
    {
        GoldUI.text = amount.ToString();
        amountUI = amount;
    }
    
}
