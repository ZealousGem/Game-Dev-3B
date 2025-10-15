using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyWaves", menuName = "Scriptable Objects/EnemyWaves")]
public class EnemyWaves : ScriptableObject
{

    public List<GameObject> Enemies; // Enemeis types contained in wave


}
