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

        public int enemiesAlive;

        private void Awake()
        {
            GameplayManager.OnGameStart.AddListener(HandleGameStart);
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
            enemiesAlive = Enemy.EnemiesAlive;
            //follow player to stay out of camera so nothing spawns in there
            //transform.position = player.transform.position;


            if (shouldSpawn)
            {

                // increment wave# and enemies to spawn every minue
                float timeSinceLastWave = Time.time - waveStartTime;
                if (timeSinceLastWave > waveInterval)
                {
                    wave++;
                    TargetEnemyCount = (int)Mathf.Ceil(wave * 1.5f) * BaseTargetEnemyCount;
                    waveStartTime = Time.time;
                }


                spawnTimer -= Time.deltaTime;
                if (spawnTimer <= 0)
                {
                    spawnTimer = SpawnInterval;

                    int spawnPosIndex;

                    int amountToSpawn = (TargetEnemyCount - Enemy.EnemiesAlive);//spawnedAmmountThisWave;//Random.Range(minToSpawnAtOnce, maxToSpawnAtOnce);

                    List<Transform> possibleSpawnPoints = new List<Transform>();
                    foreach (Transform spawnPoint in spawnPoints)
                    {
                        if (!IsObjectInView(spawnPoint))
                        {
                            possibleSpawnPoints.Add(spawnPoint);
                        }
                    }

                    possibleSpawnPoints = possibleSpawnPoints.OrderBy(t => Vector3.Distance(t.position, player.transform.position)).Take(10).ToList();

                    for (int i = 0; i < amountToSpawn; i++)// && i < spawnPoints.Count; i++)
                    {

                        //int iterations = 0;

                        //do
                        //{
                        //    iterations++;
                        //    //    // this isn't performant but whatever hopefully it doesn't freeze
                        spawnPosIndex = Random.Range(0, possibleSpawnPoints.Count);
                        Vector3 spawnPosition = possibleSpawnPoints[spawnPosIndex].position;

                        //    //    //just to stop it from potentially freezing
                        //    if (iterations > spawnPoints.Count)
                        //    {
                        //        break;
                        //    }

                        //} while (IsObjectInView(spawnPoints[spawnPosIndex]));//!gameplayManager.WithinBounds(spawnPoints[spawnPosIndex].position));


                        int enemyTypeIndex = Random.Range(0, enemyTypes.Count);
                        EnemyData enemyType = enemyTypes[enemyTypeIndex];
                        int groupSize = enemyType.spawnGroupSize > amountToSpawn ? amountToSpawn : enemyType.spawnGroupSize;

                        for (int j = 0; j < groupSize; j++)
                        {
                            GameObject enemyGO = ObjectPool.instance.GetEnemy();
                            enemyGO.GetComponentInChildren<Enemy>().SetData(enemyType);
                            enemyGO.transform.GetChild(0).position = new Vector3(spawnPosition.x + Random.Range(-2f, 2f), spawnPosition.y + Random.Range(-2f, 2f), spawnPosition.z);
                            enemyGO.SetActive(true);
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


        private void OnDestroy()
        {
            GameplayManager.OnGameStart.RemoveListener(HandleGameStart);
            //gameManager.OnNewRound.RemoveListener(HandleNewRound);
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
            Vector3 viewportPoint = Camera.main.WorldToViewportPoint(objTransform.position);

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