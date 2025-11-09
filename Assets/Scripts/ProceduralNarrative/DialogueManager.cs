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

    int waveCounter = 1;

    int lastWave = 20;

    List<string> CharacterNames = new List<string>();

    Characters Characters = new Characters();

    public List<string> dialogue = new List<string>();

    public DialogueSystem dialgoueSystem;

    public string names;

    public string Picture;

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
        switch (data.type)
        {
            case GameCondition.Lots_Of_Gold: LoadNarrative(data.type.ToString()); waveCounter++; break;
            case GameCondition.Not_alotOf_Gold: LoadNarrative(data.type.ToString()); waveCounter++;break;
            case GameCondition.Tower_Health_Equals_100:LoadNarrative(data.type.ToString()); waveCounter++;break;
            case GameCondition.Tower_Health_Less_Than_50:LoadNarrative(data.type.ToString()); waveCounter++;break; 

        }
    }

    public IEnumerator LoadCharacters()
    {
        string filepath = Path.Combine(Application.streamingAssetsPath, "Narrative.txt");
        yield return null;

        if (File.Exists(filepath))
        {
            string tempData = File.ReadAllText(filepath);

            if (!string.IsNullOrEmpty(tempData))
            {
                Characters = JsonUtility.FromJson<Characters>(tempData);
                // FindCharacter(Characters);
                RandomiseCharacter();
                String[] conditionRand = { GameCondition.Tower_Health_Less_Than_50.ToString(), GameCondition.Tower_Health_Equals_100.ToString() };
                System.Random random = new System.Random();
                string temp = conditionRand[random.Next(conditionRand.Length)];
                // Debug.Log(temp);
                LoadNarrative(temp);
                waveCounter++;


            }

            else
            {
                Debug.Log("file not found");
            }

        }
        yield return null;
    }

    void LoadNarrative(string _cond)
    {
        bool dialougeloaded = false;
        Debug.Log(waveCounter);
        if (waveCounter > lastWave)
        {
            StartCoroutine(StartWave());
          //  Debug.Log("here");
            return;
        }

        // List<Condition> lines = new List<Condition>();
        //   List<LineEvent>wavetypes = new List<LineEvent>();

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
                    if (lines[j].conditions.ToString() == targetConditionString)
                    {
                        LoadDialogue(lines[j].waves);
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

    IEnumerator StartWave()
    {
        yield return new WaitForSeconds(0.1f);
        EndGameEvent WaveChange = new EndGameEvent(StatsChange.StartWave);
        EventBus.Act(WaveChange);
    }

    void LoadDialogue(List<Dialogue> _dialogue)
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


       // Debug.Log($"Dialogue loaded for wave {waveCounter}. Speaker: {names}. Line: {dialogue[0]}");

        dialgoueSystem.StartDialogue(names, Picture, dialogue, waveCounter);
    }

    // Update is called once per frame

    void RandomiseCharacter()
    {
        CharacterNames = Characters.names;
        int num = UnityEngine.Random.Range(0, CharacterNames.Count);
        names = CharacterNames[num];

        CharacterNames.Clear();

        CharacterNames = Characters.image;
        int num2 = UnityEngine.Random.Range(0, CharacterNames.Count);
        Picture = CharacterNames[num2];

    }

    public void NextButton()
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
