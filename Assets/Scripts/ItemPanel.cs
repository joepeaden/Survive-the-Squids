using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

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

        [SerializeField] AudioSource characterAudioSource;

        [SerializeField] GameObject textImageObject;
        [SerializeField] TMP_Text textImageText;

        CharacterInfo affectedCharacter;

        void Start()
        {
            purchaseButton.onClick.AddListener(AttemptPurchaseItem);//ShopScreen.Instance.SetCurrentUpgradeItem(TheUpgradeItem));
        }

        public bool SetItem(UpgradeItemData theItem)
        {
            for (int i = 0; i < detailParent.childCount; i++)
            {

                if (detailParent.GetChild(i).gameObject != defaultTextItem.gameObject)
                {
                    Destroy(detailParent.GetChild(i).gameObject);
                }
            }

            TheUpgradeItem = theItem;

            if (theItem.upgradeType == UpgradeType.characterUpgrade)
            {
                CharacterInfo randomCharacter = Player.instance.ActiveCharacters[Random.Range(0, Player.instance.ActiveCharacters.Count)].CharInfo;
                CharacterUpgradeData charUpgrade = (CharacterUpgradeData)theItem.associatedData;

                // don't let the player pick the same upgrade twice
                if (charUpgrade.isLimited && randomCharacter.upgrades.Contains(charUpgrade.upgradeType))
                {
                    return false;
                }

                affectedCharacter = randomCharacter;

                title.text = affectedCharacter.charName + ": " + TheUpgradeItem.itemName + " Upgrade";
            }
            else
            {
                title.text = TheUpgradeItem.itemName;

            }

            // textImages are stuff like the +speed, etc. Icons.
            if (TheUpgradeItem.isTextImage)
            {
                textImageObject.SetActive(true);
                image.gameObject.SetActive(false);
                textImageText.text = TheUpgradeItem.textImageText;
            }
            else
            {
                image.gameObject.SetActive(true);
                textImageObject.SetActive(false);
                image.sprite = TheUpgradeItem.image;
            }

            image.rectTransform.sizeDelta = new Vector2(TheUpgradeItem.imageSizeX, TheUpgradeItem.imageSizeY);
            //cost.text = TheUpgradeItem.cost + " samples";

            defaultTextItem.gameObject.SetActive(false);

            foreach (string itemDetail in TheUpgradeItem.details)
            {
                TMP_Text newTextEntry = Instantiate(defaultTextItem, defaultTextItem.transform.parent).GetComponent<TMP_Text>();
                newTextEntry.gameObject.SetActive(true);
                newTextEntry.text = itemDetail;
            }

            RefreshPuchaseButton();

            return true;
        }

        public void RefreshPuchaseButton ()
        {
            purchaseButton.interactable = Player.instance.playerSamples >= TheUpgradeItem.cost;
            //purchaseButton.GetComponentInChildren<TMP_Text>().color = purchaseButton.interactable ? Color.green : Color.red;
        }

        void AttemptPurchaseItem()
        {
            //if (TheUpgradeItem.upgradeType == UpgradeType.weapon)// && Player.instance.ActiveCharacters.Count >= Player.MAX_CHARACTERS)
            //{
            //    // get yo drag on gurl
            //    EnableDrag();
            //}
            //else
            //{
                PurchaseItem();
            //}
        }

        //void EnableDrag()
        //{
        //    UpgradeScreen.Instance.SetCurrentUpgradeItem(TheUpgradeItem);
        //}

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

                        ////Player.instance.UpdateSamples(-TheUpgradeItem.cost);
                        ////ShopScreen.Instance.SetCurrentUpgradeItem(null);

                        //characterAudioSource.Play();

                        //ShopScreen.Instance.UpdateCharPanels();

                        //// for upgrades. probably temporary.
                        //if (TheUpgradeItem.itemName == "Rifle")
                        //{
                        //    ShopScreen.Instance.playerHasRifle = true;
                        //}
                        break;
                    case UpgradeType.weaponUpgrade:
                        //thisCharInfo.AddWeaponUpgrade((WeaponUpgradeData)theItem.associatedData);
                        //Player.instance.UpdateSamples(-theItem.cost);
                        //ShopScreen.Instance.SetCurrentUpgradeItem(null);
                        break;


                    case UpgradeType.companyBuff:
                        Player.instance.AddCompanyBuff((CompanyBuffData) TheUpgradeItem.associatedData);
                        break;

                    case UpgradeType.characterUpgrade:
                        CharacterUpgradeData upData = ((CharacterUpgradeData)TheUpgradeItem.associatedData);
                        affectedCharacter.AddCharacterUpgrade(upData);
                        //AddCompanyBuff((CompanyBuffData)TheUpgradeItem.associatedData);
                        break;

                    case UpgradeType.supportWeapon:
                        if (TheUpgradeItem.associatedPrefab == null)
                        {
                            Debug.LogWarning("Associated item is null!");
                            break;
                        }
                        else
                        {
                            Instantiate(TheUpgradeItem.associatedPrefab, Player.instance.transform);
                        }

                        break;
                    case UpgradeType.recruit:
                        Player.instance.AddCharacter();
                        break;
                }

                //StartCoroutine(ExitLevelScreen());

                GameplayManager.Instance.StartNewRound();
            }
        }



        //IEnumerator ExitLevelScreen()
        //{
        //    yield return new WaitForSecondsRealtime(1f);
        //    GameplayManager.Instance.StartNewRound();
        //}

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