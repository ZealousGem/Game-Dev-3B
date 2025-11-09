using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public GameObject startMenu;
    public GameObject OptionsMenu;

    public void StartGame()
    {
       
        SceneManager.LoadScene("Game");
    }

    public void Options()
    {
       
        startMenu.SetActive(false);
        OptionsMenu.SetActive(true);
    }

    public void BackToStart()
    {
     
        startMenu.SetActive(true);
        OptionsMenu.SetActive(false);
    }

    public void OnApplicationQuit()
    {
        
        Application.Quit();
    }
}