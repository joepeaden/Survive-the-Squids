using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame
{
    public class SurvivorPickup : MonoBehaviour
    {
        private Player player;
        private GameManager gameManager;
        [SerializeField] AudioClip sound;

        private void Start()
        {
            player = Player.instance;
            gameManager = GameManager.instance;
        }

        // should only be colliding with the pickup trigger on the player
        private void OnTriggerEnter2D(Collider2D other)
        {

            //if (other.GetComponent<CharacterBody>())
            //{
                player.AddCharacter();//PickupWeapon(weapon);

            GameObject audioSource = ObjectPool.instance.GetAudioSource();
            audioSource.SetActive(true);
            audioSource.GetComponent<PooledAudioSource>().SetData(sound, AudioGroups.pickup);

            // maybe no need for object pooling cause it's not like there's gonna be a lot
            Destroy(gameObject);
            //}
        }
    }
}