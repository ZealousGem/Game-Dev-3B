using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using System.Collections;


public class UIManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

  // you should know what these variables do marker 
    public GameObject TowerUI;

    bool Displayed = false;

    public TMP_Text GoldUI;

    public Image TowerHealth;

    public GameObject inGameUI;

    public GameObject GameOverUI;

    public GameObject PauseMenuUI;

    public TMP_Text waveUI;

    int WaveCounter = 1;

    int amountUI;

    float MaxTowerHealth = 200f;

    bool isPaused = false;

    bool isGameover = false;


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

    void ChangeWaveCounter() // changes the wave counter UI
    {
        WaveCounter += 1;
        waveUI.text = WaveCounter.ToString();
    }

    public void RestartGame() // Restarts the game in the scene 
    {
        SceneManager.LoadScene("Game");
        if (isPaused)
        {
            Time.timeScale = 1f;
            isPaused = false;
        }
    }

    void EndGame()  // dsiaplys game over UI
    {
        inGameUI.SetActive(false);
        GameOverUI.SetActive(true);
        isGameover = true;
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

    void UpdateHealthUI(float health) // updates HealthUI
    {
        TowerHealth.fillAmount = health / MaxTowerHealth;
       // Debug.Log("UI Health" + TowerHealth.fillAmount);
    }

    void Start()
    {
        TowerUI.SetActive(false);
        GameOverUI.SetActive(false);
        PauseMenuUI.SetActive(false);
        TowerHealth.fillAmount = 1f;
        
    }

    void Update()
    {
        if (!isGameover)
        {
            if (Input.GetKeyDown(KeyCode.Escape))  // pauses the game based on the bool, true game is pasued , false game is not paused 
            {
                if (isPaused)
                {
                    
                    Resume();
                }

                else
                {
                    EndGameEvent pause = new EndGameEvent(StatsChange.PausedGame);
                    EventBus.Act(pause);
                    StartCoroutine(PauseGame());
                }
            }
            
            
        }
    }

    public void QuitGame() // quits application 
    {
        Application.Quit();
    }

    public IEnumerator PauseGame()  // pauses game and displays pasue menu 
    {
        yield return new WaitForSeconds(0.1f);
        inGameUI.SetActive(false);
        PauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void Resume()// hides pausemenu, dispalys ingame UI and unpauses game
    { 

        inGameUI.SetActive(true);
        PauseMenuUI.SetActive(false);
        isPaused = false;
        Time.timeScale = 1f;
        EndGameEvent Unpause = new EndGameEvent(StatsChange.UnPausedGame);
        EventBus.Act(Unpause);

    }

    // Update is called once per frame

    public void ShowTowerUI() // if button is clicked TowerUI will pop up allowing the player to buy turrets , true dispalyed, false hidden 
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

    void ChangeGoldUI(int amount) // changes gold UI 
    {
        GoldUI.text = amount.ToString();
        amountUI = amount;
    }
    
}
