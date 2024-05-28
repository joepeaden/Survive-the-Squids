using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame
{
    public class SamplePickup : MonoBehaviour
    {
        public static int existingPickups;

        public static SamplePickup megaSample;

        bool isMegaSample;

        private Player player;
        private GameplayManager gameManager;
        [SerializeField] AudioClip pickupSound;

        public Sprite megaSprite;
        public Sprite normalSprite;

        public int XPValue;

        [SerializeField] SpriteRenderer rend;

        //float disappearTime;
        //public float baseDisappearTime;

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
            //disappearTime = baseDisappearTime;
            existingPickups++;
        }

        private void Update()
        {
            //disappearTime -= Time.deltaTime;
            //if (disappearTime < 0)
            //{
            //    gameObject.SetActive(false);
            //}
        }

        public void Setup(bool isMegaSample)
        {
            rend.sprite = isMegaSample ? megaSprite : normalSprite;
            XPValue = 1;
            this.isMegaSample = isMegaSample;

            if (isMegaSample)
            {
                megaSample = this;
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

            player.UpdateSamples(XPValue);

            gameObject.SetActive(false);
            //}
        }

        private void RemoveOldSample()
        {
            gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            existingPickups--;
            if (isMegaSample)
            {
                megaSample = null;
            }
        }

        private void OnDestroy()
        {
            GameplayManager.OnGameStart.RemoveListener(RemoveOldSample);
        }
    }
}