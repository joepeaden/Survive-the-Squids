
using UnityEngine;

namespace MyGame
{

    [CreateAssetMenu(fileName = "ArmorData", menuName = "MyScriptables/ArmorData")]
    public class ArmorData : ScriptableObject
    {
        public string armorName;
        public Sprite image;
        public float reloadSpeedEffect;
    }
}