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
        private int playerLevel = 0;
        private int samplesToLevelUp;
        private int samplesSinceLastLevelUp;
        public bool inMenu = true;
        public int wave = 1;
        public List<WeaponData> weapons = new List<WeaponData>();
        public List<Enemy> enemies = new List<Enemy>();

        [Header("UI")]
        public Button startButton;
        public GameObject shopUI;
        public GameObject pauseMenu;
        public GameObject pauseMenuItems;
        public GameObject tutorial;

        public TMP_Text enemiesKilledText;
        public TMP_Text samplesText;
        [SerializeField] TMP_Text waveTimerText;
        [SerializeField] TMP_Text waveNumberText;
        [SerializeField] TMP_Text waveCompletedText;
        [SerializeField] TMP_Text playerScoreText;
        [SerializeField] TMP_Text gameTimerText;
        public float gameTimerBase;
        float gameTimer;
        /// <summary>
        /// time starting from zero that game has been running
        /// </summary>
        public float CurrentGameTime => (gameTimerBase - gameTimer);

        bool gameIsStarted;

        [SerializeField] TMP_Text finalScoreText;
        [SerializeField] GameObject gameOverScreen;
        [SerializeField] GameObject gameplayUI;

        [SerializeField] GameObject objMarker;

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

            gameTimer -= Time.deltaTime;
            //gameTimerText.text = Mathf.CeilToInt(gameTimer).ToString();
            //if (gameTimer <= 0)
            //{
            //    GameOver();
            //}
        }

        public void AddObjMarker(Transform t)
        {
            GameObject marker = Instantiate(objMarker, gameplayUI.transform);
            marker.GetComponent<ObjectiveMarker>().SetData(t, "Survivor");
        }

        public void ShowAddCharacterScreen()
        {
            shopUI.SetActive(true);
            shopUI.GetComponent<UpgradeScreen>().SetupShopScreen(true);
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

        public void RevertToPauseMenu()
        {
            pauseMenuItems.SetActive(true);
            tutorial.SetActive(false);
        }

        public void ShowTutorial()
        {
            pauseMenuItems.SetActive(false);
            tutorial.SetActive(true);
        }

        public void QuitGame()
        {
            Application.Quit();
        }

        public void GameOver()
        {
            Time.timeScale = 0;
            finalScoreText.text = "Final Score: " + playerScore;
            gameOverScreen.SetActive(true);

            //inMenu = true;
            //Time.timeScale = 0;
        }

        public void ReturnToMenu()
        {
            SceneManager.LoadScene("Menu");
        }

        //public void RoundEnd()
        //{
        //    gameplayUI.SetActive(false);
        //    shopUI.SetActive(true);
        //    inMenu = true;
        //}



        public void StartNewRound()
        {
            OnNewRound.Invoke();
            gameplayUI.SetActive(true);
            shopUI.SetActive(false);
            inMenu = false;
            wave++;
            Time.timeScale = 1;
        }



        public void StartGame()
        {
            OnGameStart.Invoke();
            gameOverScreen.SetActive(false);
            gameplayUI.SetActive(true);
            enemiesKilled = 0;
            inMenu = false;
            wave = 1;
            playerLevel = 0;
            samplesToLevelUp = BaseSamplesToLevel;
            enemiesKilledText.text = enemiesKilled.ToString();
            Enemy.EnemiesAlive = 0;
            playerScore = 0;
            playerScoreText.text = playerScore.ToString();
            samplesSinceLastLevelUp = 0;

            UpdateSamples(0, true);
            Time.timeScale = 1;
            gameTimer = gameTimerBase;
        }


        public void EnemyKilled()
        {
            enemiesKilled++;

            //if (enemiesKilledThisRound >= GetEnemyCountToSpawnThisRound())
            //{
            //    RoundEnd();
            //}

            enemiesKilledText.text = enemiesKilled.ToString();
        }

        public void UpdateSamples(int playerTotalSamples, bool initializing = false, int newSamples = 0)
        {
            /// right now there's no content past level 7
            //if (playerLevel < 7)
            //{
                if (!initializing)
                {
                    samplesSinceLastLevelUp += newSamples;
                    // level the player up if necessary
                    if (samplesSinceLastLevelUp >= samplesToLevelUp)
                    {
                        samplesSinceLastLevelUp = 0;
                        playerLevel++;
                        samplesToLevelUp = 10 * playerLevel;
                        Time.timeScale = 0;
                        shopUI.SetActive(true);
                        shopUI.GetComponent<UpgradeScreen>().SetupShopScreen(false);
                        gameplayUI.SetActive(false);
                    }
                }

                PlayerBar.Instance.HandleXP(samplesSinceLastLevelUp, samplesToLevelUp);
            //}

            samplesText.text = playerTotalSamples.ToString();
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