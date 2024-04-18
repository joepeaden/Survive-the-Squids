using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame
{

    public class WeaponPickup : MonoBehaviour
    {
        //private List<WeaponData> possibleWeapons;
        //private WeaponData weapon;

        //private Player player;
        //private GameManager gameManager;

        //private void Start()
        //{
        //    player = Player.instance;
        //    gameManager = GameManager.instance;

        //    // 1 so we don't just give them another pistol
        //    weapon = gameManager.weapons[Random.Range(1, gameManager.weapons.Count)];
        //}

        //private void OnTriggerEnter2D(Collider2D other)
        //{
        //    if (other.GetComponent<CharacterBody>())
        //    {
        //        player.PickupWeapon(weapon);

        //        // maybe no need for object pooling cause it's not like there's gonna be a lot
        //        Destroy(gameObject);
        //    }
        //}
    }
}