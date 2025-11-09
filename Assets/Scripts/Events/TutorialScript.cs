using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Video; // *** IMPORTANT: Add this for VideoPlayer ***

public class TutorialScript : MonoBehaviour
{
    // ... (Your existing variables)
    public GameObject startScreen;
    public GameObject dialoguePanel;
    public GameObject button;
    public TMP_Text DialogueText;
    public string[] dialouge;
    private int element;
    private bool PlayerInView = false;
    public float Speed;
    
    // *** NEW/MODIFIED VIDEO VARIABLES ***
    public RawImage rawImage; // Replaces Image
    public VideoPlayer videoPlayer; // The component to play the video/GIF
    public VideoClip[] videoClips; // Replaces Sprite[]
    public RenderTexture renderTexture; // Output target for the VideoPlayer
    // ***********************************

    void Start()
    {
        // Initialize the VideoPlayer output
        videoPlayer.targetTexture = renderTexture;
        rawImage.texture = renderTexture; 
        videoPlayer.isLooping = true; // GIFs usually loop, set this if needed
    }

    // ... (Your existing Update method)
    void Update()
    {
        if (PlayerInView && !dialoguePanel.activeInHierarchy)
        {
            dialoguePanel.SetActive(true);
            startScreen.SetActive(false);
            StartCoroutine(TypeDialoguew());
            UpdateImage(); // Now updates the video/GIF
        }

        if (DialogueText.text == dialouge[element])
        {
            button.SetActive(true);
        }
    }

    public void ActivateButton()
    {
        PlayerInView = true;
    }

    public void UpdateImage()
    {
        // Check if there's a clip for the current element
        if (element < videoClips.Length && videoClips[element] != null)
        {
            videoPlayer.clip = videoClips[element];
            videoPlayer.Prepare();
            // Wait for preparation before playing if needed, but often Play() works fine
            videoPlayer.Play();
            rawImage.gameObject.SetActive(true); // Show the raw image
        }
        else
        {
            videoPlayer.Stop();
            rawImage.gameObject.SetActive(false); // Hide the raw image
        }
    }

    public void NextLine()
    {
        button.SetActive(false);
        if (element < dialouge.Length - 1)
        {
            element++;
            DialogueText.text = "";
            StartCoroutine(TypeDialoguew());
            UpdateImage(); // Play the next video/GIF
        }

        else
        {
            noText();
            PlayerInView = false;
        }
    }

    public void noText()
    {
        videoPlayer.Stop(); // Stop the video when the tutorial ends
        DialogueText.text = "";
        element = 0;
        dialoguePanel.SetActive(false);
        startScreen.SetActive(true);
    }

    IEnumerator TypeDialoguew()
    {
        // ... (Your existing TypeDialoguew implementation)
        foreach (var dial in dialouge[element].ToCharArray())
        {
            DialogueText.text += dial;
            yield return new WaitForSeconds(Speed);
        }
    }
}