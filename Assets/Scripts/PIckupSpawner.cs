using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.UI;
using TMPro;

namespace MyGame
{
    public class PIckupSpawner : MonoBehaviour
    {
        //[SerializeField]
        //private GameObject weaponPickupPrefab;
        [SerializeField]
        private GameObject survivorPrefab;
        private GameplayManager gameplayManager;
        Player player;

        // survivor spawn time if player has just one character, and so on
        public float survivorSpawnTime1;
        public float survivorSpawnTime2;
        public float survivorSpawnTime3;
        public float survivorSpawnTime4;
        public float survivorSpawnTime5;
        public float survivorSpawnTime6;
        public float survivorSpawnTime7;

        int currentCharacterSpawn;

        int lastCharacterCount;
        float spawnTimer;

        [SerializeField] TMP_Text survivorTimer;
        bool gameStarted;

        List<Transform> spawnPoints = new List<Transform>();

        private void Awake()
        {
            GameplayManager.OnGameStart.AddListener(Reset);
        }

        public void Start()
        {
            gameplayManager = GameplayManager.Instance;
            //gameManager.OnNewRound.AddListener(SpawnPickup);
            player = Player.instance;

            for (int i = 0; i < transform.childCount; i++)
            {
                spawnPoints.Add(transform.GetChild(i));
            }
        }

        private void Reset()
        {
            currentCharacterSpawn = 1;
            spawnTimer = GetSpawnTime();
            gameStarted = true;
        }

        private void OnDestroy()
        {
            GameplayManager.OnGameStart.RemoveListener(SpawnPickup);
            //gameManager.OnNewRound.RemoveListener(SpawnPickup);
        }

        float GetSpawnTime()
        {
            switch (currentCharacterSpawn)//player.ActiveCharacters.Count)
            {
                case 1:
                    return survivorSpawnTime1;
                case 2:
                    return survivorSpawnTime2;
                case 3:
                    return survivorSpawnTime3;
                case 4:
                    return survivorSpawnTime4;
                case 5:
                    return survivorSpawnTime5;
                case 6:
                    return survivorSpawnTime6;
                case 7:
                    return survivorSpawnTime7;
                default:
                    return float.MaxValue;
            }
        }

        private void Update()
        {
            if (gameStarted && player.ActiveCharacters.Count < Player.MAX_CHARACTERS)
            {
                // maybe if we add the possibility of losing characters 
                //if (lastCharacterCount != player.ActiveCharacters.Count)
                //{
                //    spawnTimer = GetSpawnTime();
                //}

                spawnTimer -= Time.deltaTime;
                survivorTimer.text = spawnTimer.ToString();
                if (spawnTimer <= 0)
                {
                    SpawnPickup();
                    currentCharacterSpawn++;
                    spawnTimer = GetSpawnTime();
                }

                //lastCharacterCount = player.ActiveCharacters.Count;
            }
        }

        public void SpawnPickup()
        {
            Vector2 spawnPos = spawnPoints[Random.Range(0, spawnPoints.Count)].position;//new Vector2(Random.Range(gameManager.leftBoundary, gameManager.rightBoundary), Random.Range(gameManager.lowerBoundary, gameManager.upperBoundary));

            Instantiate(survivorPrefab, spawnPos, Quaternion.identity);
        }
    }
}