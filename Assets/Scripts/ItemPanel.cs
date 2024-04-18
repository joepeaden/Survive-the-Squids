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
            purchaseButton.onClick.AddListener(() => ShopScreen.Instance.SetCurrentUpgradeItem(TheUpgradeItem));
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