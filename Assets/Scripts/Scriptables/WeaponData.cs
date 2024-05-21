using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame
{

    [CreateAssetMenu(fileName = "WeaponData", menuName = "MyScriptables/WeaponData")]
    public class WeaponData : ScriptableObject
    {
        public string weaponName;
        public float attackInterval;
        public int damage;
        public float range;
        public bool useProjPhys;
        public int projectileVelocity;
        public int projSpreadAngle;
        public int projPerShot;
        public float stunTime;
        public float knockBack;
        public bool causesBleed;
        public int bleedDamage;
        public float bleedTime;
        public float reloadTime;
        public int magSize;
        public Sprite projSprite;
        public Sprite weaponSprite;
        public List<AudioClip> weaponFireSounds;
        public int levelReq;
        public float rotationSpeedPenalty;
    }
}