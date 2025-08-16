using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;

[CreateAssetMenu(fileName = "Tower", menuName = "Scriptable Objects/Tower")]
public class Tower : ScriptableObject
{

    public Sprite TowerUI;

    public GameObject Prefab;

    public int reqAmount; 


}
