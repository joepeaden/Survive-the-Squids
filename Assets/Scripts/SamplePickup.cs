using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame
{
    public class SamplePickup : MonoBehaviour
    {
        private Player player;
        private GameManager gameManager;
        [SerializeField] AudioClip pickupSound;

        private void Start()
        {
            player = Player.instance;
            gameManager = GameManager.instance;
            gameManager.OnGameStart.AddListener(RemoveOldSample);
        }

        
        private void OnTriggerEnter2D(Collider2D other)
        {
            // should only happen with player circle pickup zone thing. Cause they re on the pickup physics layer

            //if (other.GetComponent<CharacterBody>())
            //{

            GameObject audioSource = ObjectPool.instance.GetAudioSource();
            audioSource.SetActive(true);
            //audioSource.GetComponent<AudioSource>().clip = weaponData.weaponFireSound;
            //audioSource.GetComponent<AudioSource>().Play();
            audioSource.GetComponent<PooledAudioSource>().SetData(pickupSound, AudioGroups.pickup);

            player.UpdateSamples(1);

                // maybe no need for object pooling cause it's not like there's gonna be a lot
                Destroy(gameObject);
            //}
        }

        private void RemoveOldSample()
        {
            gameManager.OnGameStart.RemoveListener(RemoveOldSample);
            Destroy(gameObject);
        }
    }
}