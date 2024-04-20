using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame
{

    public class GameplayUI : MonoBehaviour
    {
        public static GameplayUI Instance;

        public GameObject healthBarPrefab;

        private void Awake()
        {
            Instance = this;
        }

        public void AddHealthBar(Enemy e)
        {
            GameObject hb = Instantiate(healthBarPrefab, transform);
            hb.GetComponent<HealthBar>().SetupHealthBar(e);
        }
    }
}