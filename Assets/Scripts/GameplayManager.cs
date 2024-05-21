using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using UnityEngine.SceneManagement;

namespace MyGame
{

    public class GameplayManager : MonoBehaviour
    {
        public int BaseSamplesToLevel;

        // Singleton
        public static GameplayManager Instance => _instance;
        private static GameplayManager _instance;

        // Events
        public static UnityEvent OnGameStart = new UnityEvent();
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
        public List<Enemy> enemies = new List<Enemy>();
        float timeRemainingInRound;

        [Header("UI")]
        public GameObject startUI;
        public Button startButton;
        public GameObject shopUI;
        public GameObject pauseMenu;
        public TMP_Text enemiesKilledText;
        public TMP_Text samplesText;
        [SerializeField] TMP_Text waveTimerText;
        [SerializeField] TMP_Text waveNumberText;
        [SerializeField] TMP_Text waveCompletedText;
        [SerializeField] TMP_Text playerScoreText;
        [SerializeField] TMP_Text extractionTimerText;
        public float extractionTimerBase;
        float extractionTimer;

        bool gameIsStarted;

        private void Awake()
        {
            _instance = this;
        }

        private void Start()
        {
            //startButton.onClick.AddListener(StartGame);
            //startRoundButton.onClick.AddListener(StartNewRound);
            StartGame();
        }

        private void OnDestroy()
        {
            //startButton.onClick.RemoveListener(StartGame);
            //startRoundButton.onClick.RemoveListener(StartNewRound);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                TogglePauseScreen(!pauseMenu.activeSelf);
            }

            extractionTimer -= Time.deltaTime;
            extractionTimerText.text = Mathf.CeilToInt(extractionTimer).ToString();
            if (extractionTimer <= 0)
            {
                GameOver();
            }
        }

        int playerScore;
        public void AddPlayerScore(int toAdd)
        {
            playerScore += toAdd;
            playerScoreText.text = playerScore.ToString();
        }

        public void TogglePauseScreen(bool isEnabled)
        {
            pauseMenu.SetActive(isEnabled);
            Time.timeScale = isEnabled ? 0 : 1;
        }

        public void QuitGame()
        {
            Application.Quit();
        }

        public void GameOver()
        {
            SceneManager.LoadScene("Menu");
            //startUI.SetActive(true);
            //inMenu = true;
            //Time.timeScale = 0;
        }

        public void RoundEnd()
        {
            shopUI.SetActive(true);
            inMenu = true;
        }

        public void StartNewRound()
        {
            OnNewRound.Invoke();
            shopUI.SetActive(false);
            enemiesKilledThisRound = 0;
            inMenu = false;
            wave++;
            Time.timeScale = 1;
        }

        public void StartGame()
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
            Enemy.EnemiesAlive = 0;

            UpdateSamples(0, true);
            Time.timeScale = 1;
            extractionTimer = extractionTimerBase;
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
            //if (playerLevel < 7)
            //{
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
            //}

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