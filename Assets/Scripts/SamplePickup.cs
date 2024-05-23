using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame
{
    public class SamplePickup : MonoBehaviour
    {
        private Player player;
        private GameplayManager gameManager;
        [SerializeField] AudioClip pickupSound;
        float disappearTime;
        public float baseDisappearTime;

        private void Awake()
        {
            GameplayManager.OnGameStart.AddListener(RemoveOldSample);
        }

        private void Start()
        {
            player = Player.instance;
            gameManager = GameplayManager.Instance;
        }

        private void OnEnable()
        {
            disappearTime = baseDisappearTime;
        }

        private void Update()
        {
            disappearTime -= Time.deltaTime;
            if (disappearTime < 0)
            {
                Destroy(gameObject);
            }
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
            GameplayManager.OnGameStart.RemoveListener(RemoveOldSample);
            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            GameplayManager.OnGameStart.RemoveListener(RemoveOldSample);
        }
    }
}