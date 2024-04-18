using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MyGame
{
    public class LevelUpPanel : MonoBehaviour
    {
        [SerializeField]
        TMP_Text title;
        [SerializeField]
        Image image;
        [SerializeField]
        TMP_Text defaultTextItem;
        [SerializeField]
        Button button;

        [HideInInspector]
        public UpgradeItemData TheUpgradeItem;

        [SerializeField] Transform detailParent;

        //List<GameObject> detailEntries = new List<GameObject>();

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

            defaultTextItem.gameObject.SetActive(false);

            foreach (string itemDetail in TheUpgradeItem.details)
            {
                TMP_Text newTextEntry = Instantiate(defaultTextItem, defaultTextItem.transform.parent).GetComponent<TMP_Text>();
                newTextEntry.gameObject.SetActive(true);
                newTextEntry.text = itemDetail;
            }

            button.onClick.AddListener(() => LevelUpScreen.Instance.PickUpgrade(TheUpgradeItem));
        }

        private void OnDisable()
        {
            button.onClick.RemoveAllListeners();// (() => LevelUpScreen.Instance.PickUpgrade(TheUpgradeItem));
        }

        private void OnDestroy()
        {
            button.onClick.RemoveAllListeners();
        }
    }
}
