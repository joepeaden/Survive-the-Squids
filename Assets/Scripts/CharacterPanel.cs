using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.UI;

namespace MyGame
{

    public class CharacterPanel : MonoBehaviour
    {
        public TMP_Text charName;
        public TMP_Text rankText;
        public TMP_Text killsText;
        public TMP_Text hpText;
        public TMP_Text aimText;
        public TMP_Text crdText;
        public TMP_Text strText;
        public TMP_Text miniHpText;
        public TMP_Text miniAimText;
        public TMP_Text miniCrdText;
        public TMP_Text miniStrText;
        //public TMP_Text weaponName;
        //public TMP_Text reflexSpeed;
        //public TMP_Text defaultTextItem;

        [SerializeField]
        Button replaceButton;
        [SerializeField]
        Button equipmentButton;
        [SerializeField]
        Image weaponImage;
        [SerializeField]
        Image charImage;
        [SerializeField]
        Image equipmentImage;
        [SerializeField]
        GameObject replaceHighlight;
        [SerializeField] AudioSource audioSource;

        private CharacterInfo thisCharInfo;
        public bool hasCharacter;

        [SerializeField] Transform xpParent;
        [SerializeField] GameObject xpPip;

        [SerializeField] Button addHpBtn;
        [SerializeField] Button addStrBtn;
        [SerializeField] Button addCrdBtn;
        [SerializeField] Button addAimBtn;
        [SerializeField] GameObject statsGO;
        [SerializeField] GameObject miniStatsGO;

        public bool showStats;

        public void UpdateXP(int xpCount, int xpToLevel)
        {
            // can optimize this later it can just add pips instead of deleting them all
            if (xpToLevel != xpParent.childCount)
            {
                for (int i = 0; i < xpParent.childCount; i++)
                {
                    Destroy(xpParent.GetChild(i).gameObject);
                }

                for (int i = 0; i < xpToLevel; i++)
                {
                    GameObject pip = Instantiate(xpPip, xpParent);
                    if (xpCount > i)
                    {
                        pip.GetComponent<Image>().color = Color.yellow;
                    }
                }
            }
        }

        private void Start()
        {
            replaceButton.onClick.AddListener(ReplaceCharacterGun);
            addHpBtn.onClick.AddListener(AddHp);
            addStrBtn.onClick.AddListener(AddStr);
            addCrdBtn.onClick.AddListener(AddAim);
            addAimBtn.onClick.AddListener(AddCrd);
        }

        void UpdateButtonVisbiility()
        {
            addHpBtn.gameObject.SetActive(thisCharInfo.pendingLevelUps > 0);
            addStrBtn.gameObject.SetActive(thisCharInfo.pendingLevelUps > 0);
            addCrdBtn.gameObject.SetActive(thisCharInfo.pendingLevelUps > 0);
            addAimBtn.gameObject.SetActive(thisCharInfo.pendingLevelUps > 0);
        }

        void AddHp()
        {
            thisCharInfo.UpgradeStat(StatType.HP, 1);
            UpdateStatsDisplay();
            UpdateButtonVisbiility();
        }

        void AddStr()
        {
            thisCharInfo.UpgradeStat(StatType.STR, 1);
            UpdateStatsDisplay();
            UpdateButtonVisbiility();
        }

        void AddAim()
        {
            thisCharInfo.UpgradeStat(StatType.AIM, 1);
            UpdateStatsDisplay();
            UpdateButtonVisbiility();
        }

        void AddCrd()
        {
            thisCharInfo.UpgradeStat(StatType.CRD, 1);
            UpdateStatsDisplay();
            UpdateButtonVisbiility();
        }

        private void OnDestroy()
        {
            replaceButton.onClick.RemoveListener(ReplaceCharacterGun);
            addHpBtn.onClick.RemoveListener(AddHp);
            addStrBtn.onClick.RemoveListener(AddStr);
            addCrdBtn.onClick.RemoveListener(AddAim);
            addAimBtn.onClick.RemoveListener(AddCrd);
        }

        public void SetCharacter(CharacterInfo charInfo)
        {
            thisCharInfo = charInfo;
            charName.text = charInfo.charName;
            weaponImage.color = Color.white;
            charImage.color = Color.white;
            charName.color = Color.white;
            // + "  (LVL " + charInfo.level.ToString() + ")";
            //weaponName.text = charInfo.weaponData.weaponName;
            //reflexSpeed.text = "Reflex Speed: " + charInfo.ReflexSpeed.ToString();

            //defaultTextItem.gameObject.SetActive(false);

            //foreach (CharacterInfo.CharTraits theTrait in charInfo.traits)
            //{
            //    TMP_Text newTextEntry = Instantiate(defaultTextItem, defaultTextItem.transform.parent).GetComponent<TMP_Text>();
            //    newTextEntry.gameObject.SetActive(true);
            //    newTextEntry.text = Player.instance.traitEnumToData[theTrait].traitName;
            //}

            killsText.text = thisCharInfo.kills + " Kills";
            rankText.text = "Level " + thisCharInfo.level;

            UpdateStatsDisplay();

            weaponImage.sprite = charInfo.weaponData.weaponSprite;
            //Player.instance.ActiveCharacters.Count >= Player.MAX_CHARACTERS;
            SetButtonHighlights(false);
            hasCharacter = true;

            
            UpdateXP(thisCharInfo.xp, thisCharInfo.currentXPThreshold);

            statsGO.SetActive(showStats);
            miniStatsGO.SetActive(!showStats);
            if (showStats)
            {
                UpdateButtonVisbiility();
            }
        }

        void UpdateStatsDisplay()
        {
            hpText.text = "HP: " + thisCharInfo.HpStat;
            strText.text = "STR: " + thisCharInfo.StrStat;
            aimText.text = "AIM: " + thisCharInfo.AimStat;
            crdText.text = "CRD: " + thisCharInfo.CrdStat;
            miniHpText.text = "HP: " + thisCharInfo.HpStat;
            miniStrText.text = "STR: " + thisCharInfo.StrStat;
            miniAimText.text = "AIM: " + thisCharInfo.AimStat;
            miniCrdText.text = "CRD: " + thisCharInfo.CrdStat;
        }

        public void ReplaceCharacterGun()
        {
            UpgradeItemData theItem = UpgradeScreen.Instance.CurrentUpgradeItem;

            if (theItem != null)
            {
                //    switch (theItem.upgradeType)
                //    {
                //        case UpgradeType.weapon:
                //thisCharInfo.SetWeapon((WeaponData)theItem.associatedData, true);

                //CharacterBody newChar = Player.instance.ReplaceCharacter(thisCharInfo);
                bool result = thisCharInfo.SetWeapon((WeaponData)theItem.associatedData, true);

                // character didn't meet stat requirements
                if (result == false)
                {
                    return;
                }

                //Player.instance.acti//ActiveCharacters(thisCharInfo.ID)
                //CharacterBody c = Player.instance.AddCharacter();

                SetCharacter(thisCharInfo);
                //weaponImage.sprite = theItem.image;

                //Player.instance.UpdateSamples(-theItem.cost);
                UpgradeScreen.Instance.SetCurrentUpgradeItem(null);

                //            // for upgrades. probably temporary.
                //            if (theItem.itemName == "Rifle")
                //            {
                //                ShopScreen.Instance.playerHasRifle = true;
                //            }
                //            break;
                //        case UpgradeType.weaponUpgrade:
                //            thisCharInfo.AddWeaponUpgrade((WeaponUpgradeData)theItem.associatedData);
                //            Player.instance.UpdateSamples(-theItem.cost);
                //            ShopScreen.Instance.SetCurrentUpgradeItem(null);
                //            break;
                //    }
                audioSource.Play();

                StartCoroutine(ExitLevelScreen());
            }
        }

        IEnumerator ExitLevelScreen()
        {
            yield return new WaitForSecondsRealtime(1f);
            GameplayManager.Instance.StartNewRound();
        }

        public void SetButtonHighlights(bool enabled)
        {
            replaceHighlight.SetActive(enabled);
            replaceButton.interactable = enabled;
        }
    }
}