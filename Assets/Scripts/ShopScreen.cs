using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace MyGame
{
    public class ShopScreen : MonoBehaviour
    {
        public static ShopScreen Instance;

        public GameObject characterPanelPrefab;
        public GameObject upgradeItemPanelPrefab;
        public Player player;

        [SerializeField]
        Transform itemParent;
        [SerializeField]
        Transform charParent;

        public bool playerHasRifle;

        public UpgradeItemData penetratorUpgrade;
        public UpgradeItemData slamUpgrade;
        public UpgradeItemData stunUpgrade;
        public List<UpgradeItemData> shopItems = new List<UpgradeItemData>();

        public UpgradeItemData CurrentUpgradeItem => currentUpgradeItem;
        UpgradeItemData currentUpgradeItem;

        List<ItemPanel> itemPanels = new List<ItemPanel>();
        List<CharacterPanel> charPanels = new List<CharacterPanel>();

        [SerializeField]
        GameObject itemMouseFollowPlaceholder;
        [SerializeField]
        GameObject blackoutPanel;
        [SerializeField]
        TMP_Text playerSamplesText;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            for (int i= 0; i < itemParent.childCount; i++)
            {
                Transform t = itemParent.GetChild(i);
                itemPanels.Add(t.GetComponent<ItemPanel>());
                t.gameObject.SetActive(false);
            }

            for (int i = 0; i < charParent.childCount; i++)
            {
                Transform t = charParent.GetChild(i);
                charPanels.Add(t.GetComponent<CharacterPanel>());
                t.gameObject.SetActive(false);
            }

            itemMouseFollowPlaceholder.SetActive(false);
        }

        private void Update()
        {
            if (Input.GetButtonDown("Fire2"))
            {
                if (currentUpgradeItem != null)
                {
                    SetCurrentUpgradeItem(null);
                }
            }
        }

        void OnEnable()
        {
            if (player == null)
            {
                player = Player.instance;
            }

            List<UpgradeItemData> items = new List<UpgradeItemData>();
            items.AddRange(shopItems);
            if (playerHasRifle)
            {
                items.Add(penetratorUpgrade);
                items.Add(slamUpgrade);
                items.Add(stunUpgrade);
            }

            for (int i = 0; i < itemPanels.Count; i++)
            {
                int randomIndex = Random.Range(0, items.Count);
                itemPanels[i].SetItem(items[randomIndex]);
                itemPanels[i].gameObject.SetActive(true);
            }

            int charPanelIndex = 0;
            for (int i = 0; i < player.ActiveCharacters.Count; i++)
            {
                CharacterBody charBody = player.ActiveCharacters[i];
                if (charBody.isActiveAndEnabled)
                {
                    CharacterPanel charPanel = charPanels[charPanelIndex];
                    charPanel.SetCharacter(charBody.CharInfo);
                    charPanel.gameObject.SetActive(true);
                    charPanelIndex++;
                }
            }

            playerSamplesText.text = Player.instance.playerSamples.ToString();
        }

        private void OnDisable()
        {
            for (int i = 0; i < itemPanels.Count; i++)
            {
                itemPanels[i].gameObject.SetActive(false);
            }

            for (int i = 0; i < charPanels.Count; i++)
            {
                charPanels[i].gameObject.SetActive(false);
            }
        }

        public void SetCurrentUpgradeItem(UpgradeItemData theItem)
        {
            currentUpgradeItem = theItem;
            itemMouseFollowPlaceholder.SetActive(theItem != null);
            blackoutPanel.SetActive(theItem != null);
            if (theItem != null)
            {
                itemMouseFollowPlaceholder.transform.GetChild(0).GetComponent<Image>().sprite = theItem.image;
            }

            for (int i = 0; i < charPanels.Count; i++)
            {
                charPanels[i].SetButtonHighlights(theItem != null);
            }

            if (theItem == null)
            {
                playerSamplesText.text = Player.instance.playerSamples.ToString();
                foreach (ItemPanel panel in itemPanels)
                {
                    panel.RefreshPuchaseButton();
                }
            }
        }

    }
}