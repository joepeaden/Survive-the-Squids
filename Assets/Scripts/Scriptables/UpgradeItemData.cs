using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame
{

    [CreateAssetMenu(fileName = "UpgradeItemData", menuName = "MyScriptables/UpgradeItemData")]
    public class UpgradeItemData : ScriptableObject
    {
        public string itemName;
        public Sprite image;
        public int cost;
        public List<string> details = new List<string>();
        public UpgradeType upgradeType;
        public ScriptableObject associatedData;
        public GameObject associatedPrefab;
    }

    public enum UpgradeType { weapon, armor, support, recruit, companyBuff, characterBuff, weaponUpgrade, supportWeapon };
}
