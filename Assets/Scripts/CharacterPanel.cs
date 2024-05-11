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

        private void Start()
        {
            replaceButton.onClick.AddListener(ReplaceCharacter);
        }

        private void OnDestroy()
        {
            replaceButton.onClick.RemoveListener(ReplaceCharacter);
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

            weaponImage.sprite = charInfo.weaponData.weaponSprite;
            //Player.instance.ActiveCharacters.Count >= Player.MAX_CHARACTERS;
            SetButtonHighlights(false);
            hasCharacter = true;
        }

        public void ReplaceCharacter()
        {
            UpgradeItemData theItem = ShopScreen.Instance.CurrentUpgradeItem;

            if (theItem != null)
            {
                //    switch (theItem.upgradeType)
                //    {
                //        case UpgradeType.weapon:
                //thisCharInfo.SetWeapon((WeaponData)theItem.associatedData, true);

                //CharacterBody newChar = Player.instance.ReplaceCharacter(thisCharInfo);
                thisCharInfo.SetWeapon((WeaponData)theItem.associatedData, true);
                //Player.instance.acti//ActiveCharacters(thisCharInfo.ID)
                //CharacterBody c = Player.instance.AddCharacter();

                SetCharacter(thisCharInfo);
                //weaponImage.sprite = theItem.image;

                //Player.instance.UpdateSamples(-theItem.cost);
                ShopScreen.Instance.SetCurrentUpgradeItem(null);

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
            GameManager.instance.StartNewRound();
        }

        public void SetButtonHighlights(bool enabled)
        {
            replaceHighlight.SetActive(enabled);
            replaceButton.interactable = enabled;
        }
    }
}