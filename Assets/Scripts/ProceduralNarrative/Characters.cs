
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;

[System.Serializable]
public class Characters
{
    public List<string> names;

    public List<string> image;

    public CharacterType pictureOfCharacter;

}

[System.Serializable]
public class CharacterType
{
    public List<LineEvent> type;
   
}

[System.Serializable]
public class LineEvent
{
    public int wavesID;

    public List<Condition> condition;
      

}

[System.Serializable]
public class Condition
{
    public string conditions;

    public List<Dialogue> waves;
}

[System.Serializable]
public class Dialogue
{

    public int DialogueID;
    public List<DialogueLines> Narration;

}

[System.Serializable]
public class DialogueLines
{
    public string lines;
}