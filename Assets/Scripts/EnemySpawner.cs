using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public float baseSpawnInterval;
    public float spawnRateIncreaseInterval;
    public float currentSpawnInterval;
    public float spawnRateIncrement;

    private List<Transform> spawnPoints = new List<Transform>();
    private float spawnTimer;
    private bool shouldSpawn = false;
    private int spawnedAmmountThisWave;
    private GameManager gameManager;
    private Player player;

    private void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            spawnPoints.Add(transform.GetChild(i));
        }

        gameManager = GameManager.instance;
        gameManager.OnGameStart.AddListener(HandleGameStart);
        gameManager.OnNewRound.AddListener(HandleNewRound);

        StartCoroutine(IncrementSpawnInterval());

        player = Player.instance;
    }

    private void Update()
    {
        // follow player to stay out of camera so nothing spawns in there
        transform.position = player.transform.position;

        if (shouldSpawn)
        {
            spawnTimer -= Time.deltaTime;
            if (spawnTimer <= 0)
            {
                spawnTimer = currentSpawnInterval;

                int spawnPosIndex;

                int iterations = 0;
                do
                {
                    // this isn't performant but whatever hopefully it doesn't freeze
                    spawnPosIndex = Random.Range(0, spawnPoints.Count);

                    // just to stop it from potentially freezing
                    if (iterations > spawnPoints.Count)
                    {
                        break;
                    }

                } while (!gameManager.WithinBounds(spawnPoints[spawnPosIndex].position));


                GameObject enemyGO = ObjectPool.instance.GetEnemy();
                enemyGO.SetActive(true);
                enemyGO.transform.position = spawnPoints[spawnPosIndex].position;
                enemyGO.transform.rotation = Quaternion.identity;

                spawnedAmmountThisWave++;
                if (spawnedAmmountThisWave >= gameManager.GetEnemyCountToSpawnThisRound())
                {
                    shouldSpawn = false;
                }
            }
        }
    }


    private void OnDestroy()
    {
        gameManager.OnGameStart.RemoveListener(HandleGameStart);
        gameManager.OnNewRound.RemoveListener(HandleNewRound);
    }

    private void HandleNewRound()
    {
        shouldSpawn = true;
        spawnedAmmountThisWave = 0;
        currentSpawnInterval = baseSpawnInterval; 
    }

    private void HandleGameStart()
    {
        shouldSpawn = true;
        spawnedAmmountThisWave = 0;
        currentSpawnInterval = baseSpawnInterval;
    }

    private IEnumerator IncrementSpawnInterval()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnRateIncreaseInterval);
            currentSpawnInterval -= spawnRateIncrement;
        }
    }
}
