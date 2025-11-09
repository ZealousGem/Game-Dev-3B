using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public GameObject startMenu;
    public GameObject OptionsMenu;

    public void StartGame()
    {
         if (SoundManager.Instance != null)
          {
             SoundManager.Instance.PlaySound("but");  
          }
        SoundManager.Instance.StopMusic("theme");
        SoundManager.Instance.PlaySound("game");
        SoundManager.Instance.PlaySound("waves");
        SceneManager.LoadScene("Game");
    }

    public void Options()
    {
         if (SoundManager.Instance != null)
          {
             SoundManager.Instance.PlaySound("but");  
          }
        startMenu.SetActive(false);
        OptionsMenu.SetActive(true);
    }

    public void BackToStart()
    {
         if (SoundManager.Instance != null)
          {
             SoundManager.Instance.PlaySound("but");  
          }
        startMenu.SetActive(true);
        OptionsMenu.SetActive(false);
    }

    public void OnApplicationQuit()
    {
         if (SoundManager.Instance != null)
          {
             SoundManager.Instance.PlaySound("but");  
          }
        Application.Quit();
    }
}