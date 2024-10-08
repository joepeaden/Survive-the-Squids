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

            for (int i = 0; i < transform.childCount; i++)
            {
                spawnPoints.Add(transform.GetChild(i));
            }

            //gameStarted = true;
        }

        private void Reset()
        {
            spawnTimer = GetSpawnTime();
            //gameStarted = true;
        }

        private void OnDestroy()
        {
            GameplayManager.OnGameStart.RemoveListener(Reset);
            //gameManager.OnNewRound.RemoveListener(SpawnPickup);
        }

        float GetSpawnTime()
        {
            switch (Player.instance.ActiveCharacters.Count)
            {
                case 1:
                    return survivorSpawnTime1;
                case 2:
                    return survivorSpawnTime2;
                case 3:
                    return survivorSpawnTime3;
                //case 4:
                //    return survivorSpawnTime4;
                //case 5:
                //    return survivorSpawnTime5;
                //case 6:
                //    return survivorSpawnTime6;
                //case 7:
                //    return survivorSpawnTime7;
                default:
                    return float.MaxValue;
            }
        }

        private void Update()
        {
            if (Player.instance.ActiveCharacters.Count < Player.MAX_CHARACTERS)
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