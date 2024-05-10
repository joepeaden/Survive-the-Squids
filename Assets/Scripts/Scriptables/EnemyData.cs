using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame
{

    [CreateAssetMenu(fileName = "EnemyData", menuName = "MyScriptables/EnemyData")]
    public class EnemyData : ScriptableObject
    {
        public int hitPoints;
        public float moveSpeed;
        public float attackInterval;
        public int xpReward;
        public int damage;
        public Sprite bodySprite;
        public Sprite headSprite;
        public Sprite faceSprite;
        public Sprite headHitSprite;
        public Sprite bodyHitSprite;
    }
}