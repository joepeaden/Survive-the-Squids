using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MyGame
{

    public class ItemPanel : MonoBehaviour
    {
        [SerializeField]
        TMP_Text title;
        [SerializeField]
        Image image;
        [SerializeField]
        TMP_Text cost;
        [SerializeField]
        TMP_Text defaultTextItem;
        [SerializeField]
        Button purchaseButton;

        [HideInInspector]
        public UpgradeItemData TheUpgradeItem;

        [SerializeField] Transform detailParent;

        void Start()
        {
            purchaseButton.onClick.AddListener(PurchaseItem);//ShopScreen.Instance.SetCurrentUpgradeItem(TheUpgradeItem));
        }

        public void SetItem(UpgradeItemData theItem)
        {
            for (int i = 0; i < detailParent.childCount; i++)
            {

                if (detailParent.GetChild(i).gameObject != defaultTextItem.gameObject)
                {
                    Destroy(detailParent.GetChild(i).gameObject);
                }
            }

            TheUpgradeItem = theItem;

            title.text = TheUpgradeItem.itemName;
            image.sprite = TheUpgradeItem.image;
            cost.text = TheUpgradeItem.cost + " samples";

            defaultTextItem.gameObject.SetActive(false);

            foreach (string itemDetail in TheUpgradeItem.details)
            {
                TMP_Text newTextEntry = Instantiate(defaultTextItem, defaultTextItem.transform.parent).GetComponent<TMP_Text>();
                newTextEntry.gameObject.SetActive(true);
                newTextEntry.text = itemDetail;
            }

            RefreshPuchaseButton();
        }

        public void RefreshPuchaseButton ()
        {
            purchaseButton.interactable = Player.instance.playerSamples >= TheUpgradeItem.cost;
            purchaseButton.GetComponentInChildren<TMP_Text>().color = purchaseButton.interactable ? Color.green : Color.red;
        }

        void PurchaseItem()
        {
            //UpgradeItemData theItem = ShopScreen.Instance.CurrentUpgradeItem;

            if (TheUpgradeItem != null)
            {
                switch (TheUpgradeItem.upgradeType)
                {
                    case UpgradeType.weapon:
                        //thisCharInfo.SetWeapon((WeaponData)TheUpgradeItem.associatedData, true);
                        //weaponImage.sprite = theUpgradeItem.image;
                        CharacterBody c = Player.instance.AddCharacter();
                        c.CharInfo.SetWeapon((WeaponData)TheUpgradeItem.associatedData, true);

                        Player.instance.UpdateSamples(-TheUpgradeItem.cost);
                        //ShopScreen.Instance.SetCurrentUpgradeItem(null);

                        // for upgrades. probably temporary.
                        if (TheUpgradeItem.itemName == "Rifle")
                        {
                            ShopScreen.Instance.playerHasRifle = true;
                        }
                        break;
                    case UpgradeType.weaponUpgrade:
                        //thisCharInfo.AddWeaponUpgrade((WeaponUpgradeData)theItem.associatedData);
                        //Player.instance.UpdateSamples(-theItem.cost);
                        //ShopScreen.Instance.SetCurrentUpgradeItem(null);
                        break;
                }

                GameManager.instance.StartNewRound();
            }
        }

        private void OnDisable()
        {
            //for (int i = 0; i < detailEntries.Count; i++)
            //{
            //    Destroy(detailEntries[i]);
            //}
        }

        private void OnDestroy()
        {
            purchaseButton.onClick.RemoveAllListeners();
        }
    }
}