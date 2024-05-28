using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

namespace MyGame
{
    public class MenuManager : MonoBehaviour
    {
        [Header("StartScreen")]
        [SerializeField] GameObject startScreen;
        [SerializeField] Button startButton;
        [SerializeField] Button tutorialButton;
        [SerializeField] Button upgradesButton;

        [Header("tutorial Screen")]
        [SerializeField] GameObject tutorialScreen;
        [SerializeField] Button backButton2;

        [Header("UpgradeScreen")]
        [SerializeField] GameObject upgradeScreen;
        [SerializeField] Button backButton;
        [SerializeField] TMP_Text survivorsSaved;
        [SerializeField] TMP_Text samplesCollected;
        [SerializeField] TMP_Text squidsKilled;
        [SerializeField] TMP_Text points;
        [SerializeField] Button experimentalWeaponsButton;
        [SerializeField] Button droneHangarButton;
        [SerializeField] Button trainingHallButton;
        [SerializeField] TMP_Text experimentalWeaponsCost;
        [SerializeField] TMP_Text droneHangarCost;
        [SerializeField] TMP_Text trainingHallCost;

        private void Awake()
        {
            //experimentalWeaponsButton.onClick.AddListener(UnlockExperimentals);
            //droneHangarButton.onClick.AddListener(UnlockDrones);
            //trainingHallButton.onClick.AddListener(UnlockTrainingHall);
            startButton.onClick.AddListener(StartGame);
            upgradesButton.onClick.AddListener(ShowUpgradesScreen);
            backButton.onClick.AddListener(ShowStartScreen);
            tutorialButton.onClick.AddListener(ShowTutorialScreen);
            backButton2.onClick.AddListener(ShowStartScreen);
        }

        private void Start()
        {
            //if (PlayerProgression.Instance.HasUnlockedDrones)
            //{
            //    droneHangarCost.text = "Unlocked";
            //    droneHangarCost.color = Color.green;
            //    droneHangarButton.interactable = false;
            //}
            //if (PlayerProgression.Instance.HasUnlockedExperimentals)
            //{
            //    experimentalWeaponsCost.text = "Unlocked";
            //    experimentalWeaponsCost.color = Color.green;
            //    experimentalWeaponsButton.interactable = false;
            //}
            //if (PlayerProgression.Instance.HasUnlockedTrainingHall)
            //{
            //    trainingHallCost.text = "Unlocked";
            //    trainingHallCost.color = Color.green;
            //    trainingHallButton.interactable = false;
            //}
        }

        private void OnDestroy()
        {
            //experimentalWeaponsButton.onClick.RemoveListener(UnlockExperimentals);
            //droneHangarButton.onClick.RemoveListener(UnlockDrones);
            //trainingHallButton.onClick.RemoveListener(UnlockTrainingHall);
            startButton.onClick.RemoveListener(StartGame);
            upgradesButton.onClick.RemoveListener(ShowUpgradesScreen);
            backButton.onClick.RemoveListener(ShowStartScreen);
            tutorialButton.onClick.RemoveListener(ShowTutorialScreen);
            backButton2.onClick.RemoveListener(ShowStartScreen);
        }

        void StartGame()
        {
            SceneManager.LoadScene("Game");
        }

        void ShowStartScreen()
        {
            startScreen.SetActive(true);
            upgradeScreen.SetActive(false);
            tutorialScreen.SetActive(false);
        }
        
        void ShowTutorialScreen()
        {
            startScreen.SetActive(false);
            tutorialScreen.SetActive(true);
        }

        void ShowUpgradesScreen()
        {
            startScreen.SetActive(false);
            upgradeScreen.SetActive(true);
        }

        //void UnlockExperimentals()
        //{
        //    PlayerProgression.Instance.UnlockExperimentals();
        //    experimentalWeaponsCost.text = "Unlocked";
        //    experimentalWeaponsCost.color = Color.green;
        //    experimentalWeaponsButton.interactable = false;
        //}

        //void UnlockDrones()
        //{
        //    PlayerProgression.Instance.UnlockDrones();
        //    droneHangarCost.text = "Unlocked";
        //    droneHangarCost.color = Color.green;
        //    droneHangarButton.interactable = false;
        //}

        //void UnlockTrainingHall()
        //{
        //    PlayerProgression.Instance.UnlockTrainingHall();
        //    trainingHallCost.text = "Unlocked";
        //    trainingHallCost.color = Color.green;
        //    trainingHallButton.interactable = false;
        //}
    }
}