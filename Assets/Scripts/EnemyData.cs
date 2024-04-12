using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "MyScriptables/EnemyData")]
public class EnemyData : ScriptableObject
{
    public int hitPoints;
    public int moveSpeed;
    public float attackInterval;
    public int xpReward;
    public int damage;
}
