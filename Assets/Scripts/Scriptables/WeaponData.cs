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
        public bool useProjPhys;
        public int projectileVelocity;
        public int projSpreadAngle;
        public int projPerShot;
        public float stunTime;
        public float knockBack;
        public Sprite projSprite;
    }
}