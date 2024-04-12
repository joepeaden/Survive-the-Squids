using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class GameManager : MonoBehaviour
{
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
    public bool inMenu = true;
    public int level = 1;
    public int baseEnemyAmountToSpawn;
    public List<WeaponData> weapons = new List<WeaponData>();

    [Header("UI")]
    public GameObject startUI;
    public Button startButton;
    public GameObject betweenRoundsUI;
    public Button startRoundButton;
    public TMP_Text enemiesKilledText;
    public TMP_Text characterName;
    public TMP_Text currentWeapon;

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
        betweenRoundsUI.SetActive(true);
        inMenu = true;
    }

    private void StartNewRound()
    {
        OnNewRound.Invoke();
        betweenRoundsUI.SetActive(false);
        enemiesKilledThisRound = 0;
        inMenu = false;
        level++;
    }

    private void StartGame()
    {
        OnGameStart.Invoke();
        startUI.SetActive(false);
        enemiesKilled = 0;
        enemiesKilledThisRound = 0;
        inMenu = false;
        level = 1;
        enemiesKilledText.text = enemiesKilled.ToString();
    }

    public void EnemyKilled()
    {
        enemiesKilled++;
        enemiesKilledThisRound++;

        if (enemiesKilledThisRound >= GetEnemyCountToSpawnThisRound())
        {
            RoundEnd();
        }

        enemiesKilledText.text = enemiesKilled.ToString();
    }

    public int GetEnemyCountToSpawnThisRound()
    {
        return baseEnemyAmountToSpawn * level;
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

    public void UpdateCharacterUI(CharacterBody controlledCharacter)
    {
        characterName.text = controlledCharacter.charName;
        currentWeapon.text = controlledCharacter.CurrentWeapon.weaponName;
    }
}
