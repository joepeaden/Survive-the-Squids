using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace MyGame
{
    public class UpgradeScreen : MonoBehaviour
    {
        public static UpgradeScreen Instance;

        public GameObject characterPanelPrefab;
        public GameObject upgradeItemPanelPrefab;
        public Player player;

        [SerializeField]
        Transform itemParent;
        [SerializeField]
        Transform charParent;

        public List<UpgradeItemData> upgradeItems = new List<UpgradeItemData>();

        UpgradeItemData currentUpgradeItem;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        void OnEnable()
        {
            if (player == null)
            {
                player = Player.instance;
            }

            for (int i = 0; i < 3; i++)
            {
                int randomIndex = Random.Range(0, upgradeItems.Count);
                ItemPanel itemPanel = Instantiate(upgradeItemPanelPrefab, itemParent).GetComponent<ItemPanel>();
                itemPanel.SetItem(upgradeItems[randomIndex]);
            }

            // DESTROY THE CHILDREN!
            //for (int i = 0; i < transform.childCount; i++)
            //{
            //    Transform child = transform.GetChild(i);
            //    Destroy(child.gameObject);
            //}

            foreach (CharacterBody charBody in player.CharacterBodies)
            {
                CharacterPanel charPanel = Instantiate(characterPanelPrefab, charParent).GetComponent<CharacterPanel>();
                charPanel.SetCharacter(charBody.CharInfo);
            }
        }

        public void SetCurrentUpgradeItem(UpgradeItemData theItem)
        {
            currentUpgradeItem = theItem;
        }

    }
}