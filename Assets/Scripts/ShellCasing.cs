using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame
{
    public class ShellCasing : MonoBehaviour
    {
        SpriteRenderer rend;
        Rigidbody2D rb;
        private float spawnTime;

        void Start()
        {
            spawnTime = Time.time;
            rend = GetComponent<SpriteRenderer>();
            rb = GetComponent<Rigidbody2D>();

            GameplayManager.OnGameStart.AddListener(Reset);
        }

        private void OnDestroy()
        {
            GameplayManager.OnGameStart.RemoveListener(Reset);
        }

        private void Reset()
        {
            gameObject.SetActive(false);
        }

        private void Update()
        {
            if (Time.time - spawnTime > .2f)
            {
                rend.sortingOrder = 13;
            }

            if (Time.time - spawnTime > 2f)
            {
                rb.isKinematic = true;
            }
        }
    }
}