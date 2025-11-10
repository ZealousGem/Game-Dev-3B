using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Sprites;
using UnityEngine.Rendering.VirtualTexturing;
using UnityEngine.TextCore.Text;
using System.IO;
using TMPro;


public class DialogueSystem : MonoBehaviour
{
    // Start is called before the first frame update
    private Queue<string> lines;
    Sprite images;
    string names;

    public GameObject Dialogue;
    public GameObject Button;
    public Image image;

    public List<string> login;
    public TMP_Text Charname;
    public bool isAutomatic;
    public TMP_Text description;
    public int counter = 0;
    public bool end;
    public float Speed;

    public GameObject WonScreen;

    int currentWaveCounter = 0;
    int MaxWave = 20;
   

    private void Start()
    {
        if (WonScreen != null)
        {
            WonScreen.SetActive(false);
        }
        
        Dialogue.SetActive(false);
        lines = new Queue<string>(); // creates a Queue string for the dialogue 
        end = true;

    }

    public void StartDialogue(string ChacterName, string Image, List<string> Lines, int counter) // method that will start the dialogue using the characters name in the string array from diaslogue manager
    {
        
        Dialogue.SetActive(true);
        end = false;
          
        lines.Clear(); 
           
            // clears any elements that are in the queue

        foreach (string sent in Lines)
         {
           lines.Enqueue(sent); // will add all the text elements from object into the queue to create a sequential order
         }

        images = LoadSprite(Image); // finds the file path of image

        names = ChacterName;
        currentWaveCounter = counter;

        DisplayNextSentence(); // this will activate to start the elemtns to be removed from the queue


       
        
        
           
        
        
    }

    public Sprite LoadSprite(string file) // reads the path to find the image to display
    {

        string imagePath = Path.Combine(Application.streamingAssetsPath, file);
        
        if (File.Exists(imagePath)) // checks if image from the file path is found
        {
            byte[] filesInfo = File.ReadAllBytes(imagePath); // reads the bytes of image
            Texture2D texture = new Texture2D(2,2); // sets texture size
            texture.LoadImage(filesInfo); // loads the image using the bytes from read file 
            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f)); // returns the newly created read sprite image to make the sprite be loaded into the queue
        }

        else
        {
            Debug.Log("no image");
            return null; // will return null if file couldn't be read
        }
        
    }

    public void AutomaticClick()
    {
        DisplayNextSentence();
    }

    public void DisplayNextSentence() // this method is activated by the next button input
    {
        //Debug.Log("next dialogue");
        if (lines.Count == 0) // if all the elements have been dequed the dailogue will end
        {
            EndDialogue();
            return; // returns method
        }
        string sentence = lines.Dequeue(); // everytime the diplsay is pressed the element in front will be removed and makes the element behind in fron of the Queue
        Sprite Nextimage = images;
        string CharName = names;


        Charname.text = CharName; 
        image.sprite = Nextimage;
        Button.SetActive(false);
        StartCoroutine(TypeDialogue(sentence)); // displays the dialogue text in a courtieine to have text pop up in a timed sequence 
        login.Add(sentence);
        counter += 1; // keeps track fo the front position element in the queue
       
    }

    public IEnumerator TypeDialogue(string sentence)
    {
        if (SoundManager.Instance != null)
        {
           SoundManager.Instance.PlaySound("yar");  
        }
        description.text = "";
        foreach (var T in sentence.ToCharArray()) // will loop the text string through it's characters
        {
            description.text += T; // will dsiplay each character at a certain timed update to create dialogue text animation 
            yield return new WaitForSeconds(0.03f);
        }

        if (isAutomatic) // bool will check if the dialogue is an automatic for the button not to be displayed
        {
            yield return new WaitForSeconds(2f);
            AutomaticClick();
        }

        else // will be displayed for player to progress to next element in queue if bool is false 
        {
            Button.SetActive(true);
        }

    }
    
    public void ConitueButton()
    {
        if (SoundManager.Instance != null)
          {
             SoundManager.Instance.PlaySound("but");  
          }
        WonScreen.SetActive(false);
        EndGameEvent WaveChange = new EndGameEvent(StatsChange.StartWave);
        EventBus.Act(WaveChange);
    }

    void EndDialogue() // will end the dialogue by setting bool to false allowing the for loop in dialogue manager to move the i to the next position
    {
        
        Charname.text = "";
        description.text = "";
        end = true;
        counter = 0;
        login.Clear();
        Dialogue.SetActive(false);
        
        if (MaxWave == currentWaveCounter && WonScreen != null)
        {
            WonScreen.SetActive(true);
        }

        else
        {
            EndGameEvent WaveChange = new EndGameEvent(StatsChange.StartWave);
            EventBus.Act(WaveChange);
        }
       
    }

    


}