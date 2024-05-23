using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame
{

    [CreateAssetMenu(fileName = "EnemyData", menuName = "MyScriptables/EnemyData")]
    public class EnemyData : ScriptableObject
    {
        public bool vulnToCrits;
        public float dmgResist;
        public int spawnGroupSize;
        public int hitPoints;
        public float moveSpeed;
        public float attackInterval;
        public int xpReward;
        public int damage;
        public Sprite bodySprite;
        public Sprite headSprite;
        public Sprite faceUSprite;
        public Sprite faceLSprite;
        public Sprite faceRSprite;
        public Sprite faceSprite;
        public Sprite headHitSprite;
        public Sprite bodyHitSprite;
        public Sprite headCritSprite;
        public Sprite bodyCritSprite;
        public Sprite deadSprite;
        /// <summary>
        /// What time the enemy is introduced into the game
        /// </summary>
        public int introTime;
        /// <summary>
        /// Out of 1, what is the spawn chance of this enemy?
        /// </summary>
        public float spawnChance = 1;
        /// <summary>
        /// Max population of this enemy type
        /// </summary>
        public int maxPop = 1000;

        public float chanceSpawnEnemyOnDeath;
        public EnemyData enemyToSpawnOnDeath;
    }
}