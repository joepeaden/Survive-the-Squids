using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MyGame
{
    public class LevelUpScreen : MonoBehaviour
    {
        public static LevelUpScreen Instance;

        [SerializeField] Button continueButton;

        [SerializeField] Transform panelParent;
        List<LevelUpPanel> options = new List<LevelUpPanel>();

        private Player player;

        public List<UpgradeItemData> levelUpUpgrades = new List<UpgradeItemData>();

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            for (int i = 0; i < panelParent.childCount; i++)
            {
                Transform t = panelParent.GetChild(i);
                options.Add(t.GetComponent<LevelUpPanel>());
                t.gameObject.SetActive(false);
            }
        }

        private void OnDestroy()
        {
        }

        void OnEnable()
        {
            if (player == null)
            {
                player = Player.instance;
            }

            for (int i = 0; i < options.Count; i++)
            {
                int randomIndex = Random.Range(0, levelUpUpgrades.Count);
                options[i].SetItem(levelUpUpgrades[randomIndex]);
                options[i].gameObject.SetActive(true);
            }
        }

        public void PickUpgrade(UpgradeItemData upgrade)
        {
            switch (upgrade.upgradeType)
            {
                case UpgradeType.recruit:
                    //Player.instance.AddCharacter();
                    break;

                case UpgradeType.companyBuff:

                    //if (upgrade.associatedData 

                    break;

                case UpgradeType.characterBuff:
                    break;
            }

            gameObject.SetActive(false);

            Time.timeScale = 1;
        }
    }
}