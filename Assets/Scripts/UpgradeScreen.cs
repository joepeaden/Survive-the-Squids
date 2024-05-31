using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

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
        [SerializeField]
        AudioSource audioSource;

        public bool playerHasRifle;

        public UpgradeItemData penetratorUpgrade;
        public UpgradeItemData slamUpgrade;
        public UpgradeItemData stunUpgrade;
        
        public List<UpgradeItemData> shopItems = new List<UpgradeItemData>();
        public List<UpgradeItemData> characterStartItems = new List<UpgradeItemData>();

        public UpgradeItemData CurrentUpgradeItem => currentUpgradeItem;
        UpgradeItemData currentUpgradeItem;

        List<ItemPanel> itemPanels = new List<ItemPanel>();
        [SerializeField] CharacterPanel charLvlUpPanel;
        List<CharacterPanel> charPanels = new List<CharacterPanel>();

        [SerializeField]
        GameObject itemMouseFollowPlaceholder;
        [SerializeField]
        GameObject blackoutPanel;
        [SerializeField]
        TMP_Text playerSamplesText;

        [SerializeField] Button nextCharLvlUpButton;
        [SerializeField] GameObject charLvlUpStuff;
        [SerializeField] GameObject shopStuff;

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
            }

            itemMouseFollowPlaceholder.SetActive(false);

            //nextCharLvlUpButton.onClick.AddListener(CycleLevelUpChar);
        }

        private void Start()
        {
            if (player == null)
            {
                player = Player.instance;
            }
        }

        private void OnDestroy()
        {
            //nextCharLvlUpButton.onClick.RemoveListener(CycleLevelUpChar);
        }

        //void CycleLevelUpChar()
        //{

        //    bool isLevelingUp = false;

        //    for (int i = 0; i < player.ActiveCharacters.Count; i++)
        //    {
        //        CharacterBody charBody = player.ActiveCharacters[i];
        //        if (charBody.isActiveAndEnabled && charBody.CharInfo.pendingLevelUps > 0)
        //        {
        //            charLvlUpPanel.SetCharacter(charBody.CharInfo);
        //            isLevelingUp = true;
        //            break;
        //        }
        //    }

        //    charLvlUpStuff.SetActive(isLevelingUp);
        //    shopStuff.SetActive(!isLevelingUp);
        //    if (!isLevelingUp)
        //    {
        //        SetupShopScreen();
        //    }
        //}

        public void SetupShopScreen(bool pickingUpCharacter)
        {
            List<UpgradeItemData> items = new List<UpgradeItemData>();

            List<UpgradeItemData> itemsToInclude = new List<UpgradeItemData>();
            if (pickingUpCharacter)
            {
                itemsToInclude.AddRange(characterStartItems);
                charParent.gameObject.SetActive(false);
            }
            else
            {
                itemsToInclude.AddRange(shopItems);
                charParent.gameObject.SetActive(true);
            }

            items.AddRange(itemsToInclude);
            //if (playerHasRifle)
            //{
            //    items.Add(penetratorUpgrade);
            //    items.Add(slamUpgrade);
            //    items.Add(stunUpgrade);
            //}

            for (int i = 0; i < itemPanels.Count; i++)
            {
                int randomIndex = Random.Range(0, items.Count);
                bool validUpgrade = itemPanels[i].SetItem(items[randomIndex]);
                if (validUpgrade)
                {
                    itemPanels[i].gameObject.SetActive(true);
                }
                else
                {
                    // if it's not a valid upgrade try setting the upgrade again
                    i--;
                }
            }

            UpdateCharPanels();
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
            //CycleLevelUpChar();
            audioSource.Play();


           
//            playerSamplesText.text = Player.instance.playerSamples.ToString();
        }

        public void UpdateCharPanels()
        {
            for (int i = 0; i < charPanels.Count; i++)
            {
                charPanels[i].Reset();
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
        }

        public void Close()
        {
            GameplayManager.Instance.StartNewRound();
        }

        private void OnDisable()
        {
            for (int i = 0; i < itemPanels.Count; i++)
            {
                itemPanels[i].gameObject.SetActive(false);
            }

            for (int i = 0; i < charPanels.Count; i++)
            {
                //charPanels[i].gameObject.SetActive(false);
            }
        }

        public void SetCurrentUpgradeItem(UpgradeItemData theItem)
        {
            currentUpgradeItem = theItem;
            itemMouseFollowPlaceholder.SetActive(theItem != null);
            //blackoutPanel.SetActive(theItem != null);
            if (theItem != null)
            {
                itemMouseFollowPlaceholder.transform.GetChild(0).GetComponent<Image>().sprite = theItem.image;
            }

            for (int i = 0; i < charPanels.Count; i++)
            {
                if (charPanels[i].hasCharacter)
                {
                    charPanels[i].SetButtonHighlights(theItem != null);
                }
            }

            //if (theItem == null)
            //{
            //    playerSamplesText.text = Player.instance.playerSamples.ToString();
            //    foreach (ItemPanel panel in itemPanels)
            //    {
            //        panel.RefreshPuchaseButton();
            //    }
            //}
        }

    }
}