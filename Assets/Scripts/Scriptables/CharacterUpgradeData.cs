using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame
{

    [CreateAssetMenu(fileName = "CharacterUpgradeData", menuName = "MyScriptables/CharacterUpgradeData")]
    public class CharacterUpgradeData : ScriptableObject
    {
        public CharacterUpgrade upgradeType;
        public float value;
    }

    public enum CharacterUpgrade
    {
        Damage,
        FireRate,
        ReloadSpeed,
        Range,
        ShotgunRounds
    }
}