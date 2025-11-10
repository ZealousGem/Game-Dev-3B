
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;

[System.Serializable]
public class Characters // contains all the dialogue, character names and pictures
{
    public List<string> names;

    public List<string> image;

    public CharacterType pictureOfCharacter;

}

[System.Serializable]
public class CharacterType // contains all the lines the characters can say
{
    public List<LineEvent> type;
   
}

[System.Serializable]
public class LineEvent // all the lines the characteer will say for each wave
{
    public int wavesID;

    public List<Condition> condition;
      

}

[System.Serializable]
public class Condition // contains the player actions lines in each wave
{
    public string conditions;

    public List<Dialogue> waves;
}

[System.Serializable]
public class Dialogue // random dialogueID
{

    public int DialogueID;
    public List<DialogueLines> Narration;

}

[System.Serializable]
public class DialogueLines // dialogue
{
    public string lines;
}