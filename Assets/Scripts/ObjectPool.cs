using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame
{

    public class ObjectPool : MonoBehaviour
    {
        public static ObjectPool instance;

        [Header("Projectiles")]
        public GameObject projectilePrefab;
        public int projectilePoolSize;

        [Header("Enemies")]
        public GameObject enemyPrefab;
        public int enemyPoolSize;

        [Header("AudioSources")]
        public GameObject audioSourcePrefab;
        public int audioSourcePoolSize;

        [Header("corpses")]
        public GameObject corpsePrefab;
        public int corpsePoolSize;

        [Header("TextFloatUp")]
        public GameObject textFloatUpPrefab;
        public int textFloatUpPoolSize;

        [Header("ShellCasings")]
        public GameObject shellPrefab;
        public int shellPoolSize;

        [Header("Smaples")]
        public GameObject samplePrefab;
        public int samplePoolSize;

        private Transform objectPoolParent;
        private List<GameObject> projectiles;
        private List<GameObject> enemies;
        private List<GameObject> audioSources;
        private List<GameObject> corpses;
        private List<GameObject> textFloatUps;
        private List<GameObject> samples;
        private List<GameObject> shells;

        void Awake()
        {
            instance = this;
        }

        void Start()
        {
            // just for organization
            objectPoolParent = Instantiate(new GameObject()).GetComponent<Transform>();
            objectPoolParent.name = "ObjectPool";

            projectiles = CreatePool(projectilePrefab, projectiles, projectilePoolSize);
            enemies = CreatePool(enemyPrefab, enemies, enemyPoolSize);
            audioSources = CreatePool(audioSourcePrefab, audioSources, audioSourcePoolSize);
            corpses = CreatePool(corpsePrefab, corpses, corpsePoolSize);
            textFloatUps = CreatePool(textFloatUpPrefab, textFloatUps, textFloatUpPoolSize);
            samples = CreatePool(samplePrefab, samples, samplePoolSize);
            shells = CreatePool(shellPrefab, shells, shellPoolSize);
        }

        private List<GameObject> CreatePool(GameObject prefab, List<GameObject> listToAssign, int count)
        {
            listToAssign = new List<GameObject>();
            GameObject tmp;
            for (int i = 0; i < count; i++)
            {
                tmp = Instantiate(prefab, objectPoolParent);
                tmp.SetActive(false);
                listToAssign.Add(tmp);
            }

            return listToAssign;
        }

        private GameObject GetPooledObject(List<GameObject> theList, GameObject prefab)
        {
            for (int i = 0; i < theList.Count; i++)
            {
                if (!theList[i].activeInHierarchy)
                {
                    return theList[i];
                }
            }

            GameObject newObject = Instantiate(prefab);
            theList.Add(newObject);
            return newObject;
        }

        public GameObject GetProjectile()
        {
            return GetPooledObject(projectiles, projectilePrefab);
        }

        public GameObject GetEnemy()
        {
            return GetPooledObject(enemies, enemyPrefab);
        }

        public GameObject GetAudioSource()
        {
            return GetPooledObject(audioSources, audioSourcePrefab);
        }

        public GameObject GetCorpse()
        {
            return GetPooledObject(corpses, corpsePrefab);
        }

        public GameObject GetTextFloatUp()
        {
            return GetPooledObject(textFloatUps, textFloatUpPrefab);
        }

        public GameObject GetShell()
        {
            return GetPooledObject(shells, shellPrefab);
        }

        public GameObject GetSample()
        {
            return GetPooledObject(samples, samplePrefab);
        }
    }
}