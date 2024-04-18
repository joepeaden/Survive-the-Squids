using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame
{
    public class SamplePickup : MonoBehaviour
    {
        private Player player;
        private GameManager gameManager;

        private void Start()
        {
            player = Player.instance;
            gameManager = GameManager.instance;
            gameManager.OnGameStart.AddListener(RemoveOldSample);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.GetComponent<CharacterBody>())
            {
                player.UpdateSamples(1);

                // maybe no need for object pooling cause it's not like there's gonna be a lot
                Destroy(gameObject);
            }
        }

        private void RemoveOldSample()
        {
            gameManager.OnGameStart.RemoveListener(RemoveOldSample);
            Destroy(gameObject);
        }
    }
}