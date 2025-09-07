
using UnityEngine;

[CreateAssetMenu(fileName = "Tower", menuName = "Scriptable Objects/Tower")]
public class Tower : ScriptableObject
{

    public Sprite TowerUI; // image of the tower

    public GameObject Prefab; 

    public int reqAmount; // amount needed to spawn tower


}
