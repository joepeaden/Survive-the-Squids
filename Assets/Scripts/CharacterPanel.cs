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
        Button weaponButton;
        [SerializeField]
        Button equipmentButton;
        [SerializeField]
        Image weaponImage;
        [SerializeField]
        Image equipmentImage;
        [SerializeField]
        GameObject weaponButtonHighlight;

        private CharacterInfo thisCharInfo;

        public void SetCharacter(CharacterInfo charInfo)
        {
            thisCharInfo = charInfo;
            charName.text = charInfo.charName;// + "  (LVL " + charInfo.level.ToString() + ")";
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
            weaponButton.onClick.AddListener(UpdateCharWeapon);
            SetButtonHighlights(false);
        }

        public void UpdateCharWeapon()
        {
            UpgradeItemData theItem = ShopScreen.Instance.CurrentUpgradeItem;

            if (theItem != null)
            {
                switch (theItem.upgradeType)
                {
                    case UpgradeType.weapon:
                        thisCharInfo.SetWeapon((WeaponData)theItem.associatedData, true);
                        weaponImage.sprite = theItem.image;
                        Player.instance.UpdateSamples(-theItem.cost);
                        ShopScreen.Instance.SetCurrentUpgradeItem(null);

                        // for upgrades. probably temporary.
                        if (theItem.itemName == "Rifle")
                        {
                            ShopScreen.Instance.playerHasRifle = true;
                        }
                        break;
                    case UpgradeType.weaponUpgrade:
                        thisCharInfo.AddWeaponUpgrade((WeaponUpgradeData)theItem.associatedData);
                        Player.instance.UpdateSamples(-theItem.cost);
                        ShopScreen.Instance.SetCurrentUpgradeItem(null);
                        break;
                }
            }
        }

        public void SetButtonHighlights(bool enabled)
        {
            weaponButtonHighlight.SetActive(enabled);
        }
    }
}