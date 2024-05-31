using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MyGame
{
    /*
     * Plan:
     *  - One wave per minute
     *  - Spawn until target number of enemies on screen for that wave
     */


    public class EnemySpawner : MonoBehaviour
    {
        static int MAX_ENEMY_COUNT = 200;

        public float SpawnInterval;
        //public float spawnRateIncreaseInterval;
        //public float currentSpawnInterval;
        //public float spawnRateIncrement;
        //public int minToSpawnAtOnce;
        //public int maxToSpawnAtOnce;

        private List<Transform> spawnPoints = new List<Transform>();
        private float spawnTimer;
        private bool shouldSpawn = false;
        private int spawnedAmmountThisWave;
        private GameplayManager gameplayManager;
        private Player player;

        public List<EnemyData> enemyTypes = new List<EnemyData>();

        int wave = 0;
        public int BaseTargetEnemyCount;
        public int TargetEnemyCount;
        float waveStartTime;
        public float waveInterval;

        public float enemyCountMultiplier;
        Camera mainCamera;

        private void Awake()
        {
            GameplayManager.OnGameStart.AddListener(HandleGameStart);
            mainCamera = Camera.main;
        }

        private void Start()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                spawnPoints.Add(transform.GetChild(i));
            }

            gameplayManager = GameplayManager.Instance;
            //gameManager.OnNewRound.AddListener(HandleNewRound);

            //StartCoroutine(IncrementSpawnInterval());

            player = Player.instance;
        }

        private void LateUpdate()
        {
            //follow player to stay out of camera so nothing spawns in there
            //transform.position = player.transform.position;


            if (shouldSpawn)
            {

                // increment wave# and enemies to spawn every minue
                float timeSinceLastWave = Time.time - waveStartTime;
                if (timeSinceLastWave > waveInterval)
                {
                    wave++;
                    TargetEnemyCount = (int)Mathf.Ceil(wave * enemyCountMultiplier) * BaseTargetEnemyCount;
                    //Debug.Log("New Target Enemy Count: " + TargetEnemyCount);
                    waveStartTime = Time.time;
                }


                spawnTimer -= Time.deltaTime;
                if (spawnTimer <= 0 && Enemy.EnemiesAlive < MAX_ENEMY_COUNT)
                {
                    spawnTimer = SpawnInterval;

                    int spawnPosIndex;

                    int amountToSpawn = (TargetEnemyCount - Enemy.EnemiesAlive);
                    List<Transform> possibleSpawnPoints = GetSpawnPoints();

                    for (int i = 0; i < amountToSpawn; i++)
                    {
                        spawnPosIndex = Random.Range(0, possibleSpawnPoints.Count);
                        Vector3 spawnPosition = possibleSpawnPoints[spawnPosIndex].position;

                        EnemyData enemyType = PickEnemyType();
                        // don't go over the amount we are set to spawn
                        int groupSize = enemyType.spawnGroupSize > amountToSpawn ? amountToSpawn : enemyType.spawnGroupSize;
                        // make sure not to exceed the max pop of this enemy type

                        int enemyPopLimit = enemyType.maxPop;
                        if (wave > 0)
                        {
                            enemyPopLimit *= (int)Mathf.Ceil(wave * enemyCountMultiplier);
                        }

                        if (Enemy.GetPopOfEnemyType(enemyType) + groupSize > enemyPopLimit)
                        {
                            groupSize = enemyPopLimit - Enemy.GetPopOfEnemyType(enemyType);
                        }

                        for (int j = 0; j < groupSize; j++)
                        {
                            SpawnEnemyAtPosition(new Vector3(spawnPosition.x + Random.Range(-2f, 2f), spawnPosition.y + Random.Range(-2f, 2f), spawnPosition.z), enemyType);
                        }

                        //  we spawned "groupSize" enemies so advance the counter
                        i += groupSize;

                        //enemyGO.transform.rotation = Quaternion.identity;

                        //GameplayUI.Instance.AddHealthBar(enemyGO.GetComponentInChildren<Enemy>());

                        //spawnedAmmountThisWave++;
                        //if (spawnedAmmountThisWave >= gameManager.GetEnemyCountToSpawnThisRound())
                        //{
                        //    shouldSpawn = false;
                        //    break;
                        //}
                    }
                }
            }
        }

        public static void SpawnEnemyAtPosition(Vector3 position, EnemyData enemyType)
        {
            GameObject enemyGO = ObjectPool.instance.GetEnemy();
            enemyGO.transform.GetChild(0).position = position;
            enemyGO.GetComponentInChildren<Enemy>().SetData(enemyType);
            enemyGO.SetActive(true);
        }

        List<Transform> GetSpawnPoints()
        {
            List<Transform> possibleSpawnPoints = new List<Transform>();

            // only select ones not in the camera view
            foreach (Transform spawnPoint in spawnPoints)
            {
                if (!IsObjectInView(spawnPoint))
                {
                    possibleSpawnPoints.Add(spawnPoint);
                }
            }
            
            // pick the top ten that are closest to the player
            possibleSpawnPoints = possibleSpawnPoints.OrderBy(t => Vector3.Distance(t.position, player.transform.position)).Take(10).ToList();

            return possibleSpawnPoints;
        }


        private void OnDestroy()
        {
            GameplayManager.OnGameStart.RemoveListener(HandleGameStart);
            //gameManager.OnNewRound.RemoveListener(HandleNewRound);
        }

        EnemyData PickEnemyType()
        {
            // Calculate the total weight
            float totalWeight = 0f;
            foreach (EnemyData enemy in enemyTypes)
            {
                if (enemy.introTime <= gameplayManager.CurrentGameTime && Enemy.GetPopOfEnemyType(enemy) < enemy.maxPop)
                {
                    totalWeight += enemy.spawnChance;
                }
            }

            // Generate a random number between 0 and totalWeight
            float randomValue = Random.Range(0, totalWeight);

            // Determine which enemy to spawn
            float cumulativeWeight = 0f;
            foreach (EnemyData enemy in enemyTypes)
            {
                if (enemy.introTime <= gameplayManager.CurrentGameTime && Enemy.GetPopOfEnemyType(enemy) < enemy.maxPop)
                {
                    cumulativeWeight += enemy.spawnChance;
                    if (randomValue < cumulativeWeight)
                    {
                        return enemy;
                    }
                }
            }

            // if something doesn't work return first enemy type
            return enemyTypes[0];
        }
    

        //private void HandleNewRound()
        //{
        //    shouldSpawn = true;
        //spawnedAmmountThisWave = 0;
        //currentSpawnInterval = baseSpawnInterval;
        //}

        bool IsObjectInView(Transform objTransform)
        {
            // Convert world position to viewport position
            Vector3 viewportPoint = mainCamera.WorldToViewportPoint(objTransform.position);

            // Check if within the viewport range
            bool isInCameraView = (viewportPoint.x >= 0 && viewportPoint.x <= 1) &&
                             (viewportPoint.y >= 0 && viewportPoint.y <= 1) &&
                             (viewportPoint.z > 0); // Ensure the object is in front of the camera

            return isInCameraView;
        }

        private void HandleGameStart()
        {
            shouldSpawn = true;
            waveStartTime = Time.time;
            TargetEnemyCount = BaseTargetEnemyCount;
            wave = 0;
        //currentSpawnInterval = baseSpawnInterval;
        }

        //private IEnumerator IncrementSpawnInterval()
        //{
        //    while (true)
        //    {
        //        yield return new WaitForSeconds(spawnRateIncreaseInterval);
        //        currentSpawnInterval -= spawnRateIncrement;
        //    }
        //}
    }
}