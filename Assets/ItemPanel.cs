using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

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

        public void SetItem(UpgradeItemData theItem)
        {
            TheUpgradeItem = theItem;

            title.text = theItem.name;
            image.sprite = theItem.image;
            cost.text = "$" + theItem.cost;

            defaultTextItem.gameObject.SetActive(false);

            foreach (string itemDetail in theItem.details)
            {
                TMP_Text newTextEntry = Instantiate(defaultTextItem, defaultTextItem.transform.parent).GetComponent<TMP_Text>();
                newTextEntry.gameObject.SetActive(true);
                newTextEntry.text = itemDetail;
            }

            purchaseButton.onClick.AddListener(() => UpgradeScreen.Instance.SetCurrentUpgradeItem(theItem));
        }

        private void OnDestroy()
        {
            purchaseButton.onClick.RemoveAllListeners();
        }
    }
}