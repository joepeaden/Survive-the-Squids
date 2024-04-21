using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

namespace MyGame
{

    public class GameManager : MonoBehaviour
    {
        public int BaseSamplesToLevel;

        // Singleton
        public static GameManager instance => _instance;
        private static GameManager _instance;

        // Events
        public UnityEvent OnGameStart = new UnityEvent();
        public UnityEvent OnNewRound = new UnityEvent();

        // Other Stuff
        public int upperBoundary;
        public int lowerBoundary;
        public int leftBoundary;
        public int rightBoundary;
        public int enemiesKilled;
        private int enemiesKilledThisRound;
        private int playerLevel = 0;
        private int samplesToLevelUp;
        private int samplesSinceLastLevelUp;
        public bool inMenu = true;
        public int wave = 1;
        public List<WeaponData> weapons = new List<WeaponData>();

        [Header("UI")]
        public GameObject startUI;
        public Button startButton;
        public GameObject shopUI;
        public GameObject controlsUI;
        public Button startRoundButton;
        public TMP_Text enemiesKilledText;
        public TMP_Text samplesText;
        //public TMP_Text characterName;
        //public TMP_Text currentWeapon;

        private void Awake()
        {
            _instance = this;
        }

        private void Start()
        {
            startButton.onClick.AddListener(StartGame);
            startRoundButton.onClick.AddListener(StartNewRound);
        }

        private void OnDestroy()
        {
            startButton.onClick.RemoveListener(StartGame);
            startRoundButton.onClick.RemoveListener(StartNewRound);
        }

        public void GameOver()
        {
            startUI.SetActive(true);
            inMenu = true;
        }

        public void RoundEnd()
        {
            shopUI.SetActive(true);
            controlsUI.SetActive(false);
            inMenu = true;
        }

        public void StartNewRound()
        {
            OnNewRound.Invoke();
            shopUI.SetActive(false);
            controlsUI.SetActive(true);
            enemiesKilledThisRound = 0;
            inMenu = false;
            wave++;
            Time.timeScale = 1;
        }

        private void StartGame()
        {
            OnGameStart.Invoke();
            startUI.SetActive(false);
            enemiesKilled = 0;
            enemiesKilledThisRound = 0;
            inMenu = false;
            wave = 1;
            playerLevel = 0;
            samplesToLevelUp = BaseSamplesToLevel;
            enemiesKilledText.text = enemiesKilled.ToString();

            UpdateSamples(0, true);
        }

        public void EnemyKilled()
        {
            enemiesKilled++;
            enemiesKilledThisRound++;

            //if (enemiesKilledThisRound >= GetEnemyCountToSpawnThisRound())
            //{
            //    RoundEnd();
            //}

            enemiesKilledText.text = enemiesKilled.ToString();
        }

        public void UpdateSamples(int samples, bool initializing = false)
        {
            /// right now there's no content past level 7
            if (playerLevel < 7)
            {
                if (!initializing)
                {
                    samplesSinceLastLevelUp++;
                    // level the player up if necessary
                    if (samplesSinceLastLevelUp >= samplesToLevelUp)
                    {
                        samplesSinceLastLevelUp = 0;
                        playerLevel++;
                        samplesToLevelUp = 10 * playerLevel;
                        Time.timeScale = 0;
                        shopUI.SetActive(true);
                    }
                }

                PlayerBar.Instance.HandleXP(samplesSinceLastLevelUp, samplesToLevelUp);
            }

            samplesText.text = samples.ToString();
        }


        public bool WithinBounds(Vector2 position)
        {
            if (position.y > lowerBoundary
                && position.y < upperBoundary
                && position.x < rightBoundary
                && position.x > leftBoundary)
            {
                return true;
            }

            return false;
        }

        //public void UpdateCharacterUI(CharacterBody controlledCharacter)
        //{
        //    characterName.text = controlledCharacter.charName;
        //    currentWeapon.text = controlledCharacter.CurrentWeapon.weaponName;
        //}
    }
}