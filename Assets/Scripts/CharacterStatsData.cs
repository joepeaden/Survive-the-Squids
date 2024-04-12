using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterStatsData", menuName = "MyScriptables/CharacterStatsData")]
public class CharacterStatsData : ScriptableObject
{
    public float reflexSpeed;
    public float critChance;
    public int totalHitPoints;
    //public int moveSpeed;
    //public float attackInterval;
}
