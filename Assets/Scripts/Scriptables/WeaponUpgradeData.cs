using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame
{

    [CreateAssetMenu(fileName = "WeaponUpgradeData", menuName = "MyScriptables/WeaponUpgradeData")]
    public class WeaponUpgradeData : ScriptableObject
    {
        public string id;
        public List<float> levelEffects;
        public WeaponData associatedWeapon;
    }
}