using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame
{
    public class SurvivorPickup : MonoBehaviour
    {
        private Player player;
        private GameplayManager gameplayManager;
        [SerializeField] AudioClip sound;

        public float initialPickupTimer;
        float pickupTimer;

        private void Start()
        {
            player = Player.instance;
            //gameplayManager = GameplayManager.Instance;
            
        }

        private void OnEnable()
        {
            pickupTimer = initialPickupTimer;

            GameplayManager.Instance.AddObjMarker(transform);

            GameplayManager.OnGameStart.AddListener(Reset);
        }

        private void OnDisable()
        {

            GameplayManager.OnGameStart.RemoveListener(Reset);
        }

        private void Reset()
        {
            Destroy(transform.parent.gameObject);
        }

        // should only be colliding with the pickup trigger on the player
        private void OnTriggerEnter2D(Collider2D other)
        {

            //if (other.GetComponent<CharacterBody>())
            //{
            if (other.tag == "PickupCollider")
            {
                //pickupTimer -= Time.deltaTime;
                //if (pickupTimer <= 0)
                //{
                    RescueSurvivor();
                //    }
            }
        }

        void RescueSurvivor()
        {
            player.AddCharacter();//PickupWeapon(weapon);

            GameObject audioSource = ObjectPool.instance.GetAudioSource();
            audioSource.SetActive(true);
            audioSource.GetComponent<PooledAudioSource>().SetData(sound, AudioGroups.pickup);

            // maybe no need for object pooling cause it's not like there's gonna be a lot
            Destroy(transform.parent.gameObject);
            
        }
    }
}