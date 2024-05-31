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
        public float imageSizeX;
        public float imageSizeY;
        /// <summary>
        /// If there's a text/image combo this is true
        /// </summary>
        public bool isTextImage;
        public string textImageText;
    }

    public enum UpgradeType { weapon, armor, support, recruit, companyBuff, characterUpgrade, weaponUpgrade, supportWeapon };
}
