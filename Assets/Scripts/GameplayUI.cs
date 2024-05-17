using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame
{

    public class GameplayUI : MonoBehaviour
    {
        public static GameplayUI Instance;
        public Transform healthBarsParent;

        public GameObject healthBarPrefab;
        public GameObject floatUpPrefab;

        public Transform worldSpaceCanvas;

        private void Awake()
        {
            Instance = this;
        }

        public void AddHealthBar(Enemy e)
        {
            GameObject hb = Instantiate(healthBarPrefab, healthBarsParent);
            hb.GetComponent<HealthBar>().SetupHealthBar(e);
        }

        public void AddTextFloatup(Vector3 position, string text, Color color)
        {
            GameObject floater = Instantiate(floatUpPrefab, worldSpaceCanvas);
            floater.GetComponent<TextFloatUp>().SetData(position, text, color);
        }
    }
}