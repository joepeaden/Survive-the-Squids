using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame
{

    [CreateAssetMenu(fileName = "CharacterStatsData", menuName = "MyScriptables/CharacterStatsData")]
    public class CharacterStatsData : ScriptableObject
    {
        public float reflexSpeed;
        public float critChance;
        public int totalHitPoints;
        //public int moveSpeed;
        //public float attackInterval;
    }
}