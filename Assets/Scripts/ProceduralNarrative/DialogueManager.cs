using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;


public enum GameCondition
{
    Tower_Health_Less_Than_50,
    Tower_Health_Equals_100,

    Lots_Of_Gold,

    Not_alotOf_Gold
}

public class DialogueManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    int waveCounter = 1; // counter that will check which wave game is on to find dialogue for that wave

    int lastWave = 20; // final wave gialogue

    List<string> CharacterNames = new List<string>(); // will contain all character names from the json file

    Characters Characters = new Characters(); // contains the all dialogue from json file

    public List<string> dialogue = new List<string>(); // contain the current line of the current wave

    public DialogueSystem dialgoueSystem; // where dialogue will be displayed

    public string names;

    public string Picture; // picture of the pirate character t hat will be displayed 

    void Start()
    {
       
        StartCoroutine(LoadCharacters());
    }

     void OnEnable()
    {
        EventBus.Subscribe<DialogueEvent>(getData);
    }

    void OnDisable()
    {
         EventBus.Unsubscribe<DialogueEvent>(getData);

    }

    
    void getData(DialogueEvent data)
    {
        switch (data.type) // depeding on the players action one of the condition will be chosen to display that dialogue of the players actions
        {
            case GameCondition.Lots_Of_Gold: LoadNarrative(data.type.ToString()); waveCounter++; break;
            case GameCondition.Not_alotOf_Gold: LoadNarrative(data.type.ToString()); waveCounter++;break;
            case GameCondition.Tower_Health_Equals_100:LoadNarrative(data.type.ToString()); waveCounter++;break;
            case GameCondition.Tower_Health_Less_Than_50:LoadNarrative(data.type.ToString()); waveCounter++;break; 

        }
    }

    public IEnumerator LoadCharacters()
    {
        string filepath = Path.Combine(Application.streamingAssetsPath, "Narrative.txt"); // will find Json file through the streaming assets folder 
        yield return null;

        if (File.Exists(filepath))
        {
            string tempData = File.ReadAllText(filepath);  // checks if the the josn file exisits

            if (!string.IsNullOrEmpty(tempData))  // will check if data in josn file isn't null
            {
                Characters = JsonUtility.FromJson<Characters>(tempData);  // converts the json data to the variables in Characters
             
                RandomiseCharacter(); // randomly gnerates a character name
                String[] conditionRand = { GameCondition.Tower_Health_Less_Than_50.ToString(), GameCondition.Tower_Health_Equals_100.ToString() }; // finds a random condition since it is the start of the game
                System.Random random = new System.Random();
                string temp = conditionRand[random.Next(conditionRand.Length)];
              
                LoadNarrative(temp); // will load the first narrative dialogue in the game  
                waveCounter++; // increases the wave counter once dialogue is finished


            }

            else
            {
                Debug.Log("file not found");
            }

        }
        yield return null;
    }

    void LoadNarrative(string _cond) // this will find the specfic dialogue based on the characters actions 
    {
        bool dialougeloaded = false;
        Debug.Log(waveCounter);
        if (waveCounter > lastWave) // if the wavecounter is bigger than the final wave id then the wave will automaitcally start
        {
            StartCoroutine(StartWave());
            return;
        }

       

        string targetConditionString = _cond.ToString();

        List<LineEvent> wavetypes = Characters.pictureOfCharacter.type;

        for (int i = 0; i < wavetypes.Count; i++)
        {
            if (wavetypes[i].wavesID == waveCounter)
            {
                List<Condition> lines = wavetypes[i].condition;

                for (int j = 0; j < lines.Count; j++)
                {
                    //  Debug.Log(lines[j].conditions);
                    if (lines[j].conditions.ToString() == targetConditionString) // checks if the condition is equal to the player action
                    {
                        LoadDialogue(lines[j].waves); // loads the dialogue
                        dialougeloaded = true;
                        break;
                    }


                }
                break;
            }
        }

        if (!dialougeloaded)
        {
            StartCoroutine(StartWave());
            Debug.Log("No matching dialogue condition was found for wave " + waveCounter + " and condition " + _cond.ToString());
        }


    }

    IEnumerator StartWave() // starts the wave automatically
    {
        yield return new WaitForSeconds(0.1f);
        EndGameEvent WaveChange = new EndGameEvent(StatsChange.StartWave);
        EventBus.Act(WaveChange);
    }

    void LoadDialogue(List<Dialogue> _dialogue) // finds t he specific line from the condition 
    {
        dialogue.Clear();

        List<int> integer = new List<int>();
        foreach (Dialogue i in _dialogue)
        {
            integer.Add(i.DialogueID);
        }

        int num = UnityEngine.Random.Range(0, integer.Count);

        List<DialogueLines> tempL = _dialogue[num].Narration;
        //  Debug.Log(tempL[0].lines);

        foreach (DialogueLines i in tempL)
        {
            dialogue.Add(i.lines);
        }


      

        dialgoueSystem.StartDialogue(names, Picture, dialogue, waveCounter); // transfers line to the dialogue system so it can be displayed to the player
    }

    // Update is called once per frame

    void RandomiseCharacter() // randomly genreates a character name 
    {
        CharacterNames = Characters.names;
        int num = UnityEngine.Random.Range(0, CharacterNames.Count);
        names = CharacterNames[num];

        CharacterNames.Clear();

        CharacterNames = Characters.image;
        int num2 = UnityEngine.Random.Range(0, CharacterNames.Count);
        Picture = CharacterNames[num2];

    }

    public void NextButton() // custom editor then for debugging 
    {
        waveCounter++;
        String[] conditionRand = { GameCondition.Tower_Health_Less_Than_50.ToString(), GameCondition.Tower_Health_Equals_100.ToString() };
        System.Random random = new System.Random();
        string temp = conditionRand[random.Next(conditionRand.Length)];
                // Debug.Log(temp);
        LoadNarrative(temp);

    }


}

//  [CustomEditor(typeof(DialogueManager))]

//  public class Button : Editor
// {
    
//     public override void OnInspectorGUI()
//     {
//         DialogueManager land = (DialogueManager)target;
     

//         DrawDefaultInspector();
//         if (GUILayout.Button("Generate"))
//         {
//             land.NextButton();
//         }
//     }
// }
