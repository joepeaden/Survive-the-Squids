using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

namespace MyGame
{

    public class Enemy : MonoBehaviour
    {
        public bool isDead;
        public EnemyData data;

        private int remainingHitPoints;
        private float attackTimer;
        private Player player;
        private CharacterBody targetCharacter;
        private AIPath pathfinder;
        private Rigidbody2D rb;
        private float stunTime;

        [SerializeField]
        GameObject sampleDrop;
        [SerializeField]
        EnemySprite spriteController;

        private void Start()
        {
            pathfinder = GetComponent<AIPath>();
            GameManager.instance.OnGameStart.AddListener(Reset);
            rb = GetComponent<Rigidbody2D>();
        }

        private void OnEnable()
        {
            player = Player.instance;
            isDead = false;
            PickTarget();
            remainingHitPoints = data.hitPoints;
            if (pathfinder == null)
            {
                pathfinder = GetComponent<AIPath>();
            }
            pathfinder.maxSpeed = data.moveSpeed;
        }

        private void Update()
        {
            attackTimer -= Time.deltaTime;
            stunTime -= Time.deltaTime;

            if (stunTime > 0)
            {
                pathfinder.canMove = false;
            }
            else
            {
                pathfinder.canMove = true;

                if (targetCharacter != null)
                {
                    pathfinder.destination = targetCharacter.transform.position;

                    if (pathfinder.reachedDestination && attackTimer <= 0)
                    {
                        targetCharacter.GetHit(data.damage);
                        attackTimer = data.attackInterval;

                        if (targetCharacter.isDead)
                        {
                            PickTarget();
                        }
                    }
                }
            }
        }

        private void OnDestroy()
        {
            GameManager.instance.OnGameStart.RemoveListener(Reset);
        }

        private void Reset()
        {
            transform.parent.gameObject.SetActive(false);
            remainingHitPoints = data.hitPoints;
        }

        public void SetData(EnemyData newData)
        {
            data = newData;
            spriteController.bodySprite.sprite = newData.bodySprite;
        }

        private void PickTarget()
        {
            float smallestDist = float.MaxValue;
            for (int i = 0; i < player.ActiveCharacters.Count; i++)
            {
                if (player.ActiveCharacters[i].isDead || !player.ActiveCharacters[i].isActiveAndEnabled)
                {
                    continue;
                }

                float dist = (transform.position - player.ActiveCharacters[i].transform.position).magnitude;
                if (dist < smallestDist)
                {
                    smallestDist = dist;
                    targetCharacter = player.ActiveCharacters[i];
                }
            }
        }

        public void GetHit(WeaponData hitWeaponData, Vector2 forceDirection)
        {
            remainingHitPoints -= hitWeaponData.damage;

            // set stunTime, but only change it if the new value is greater than the old.
            if (stunTime < hitWeaponData.stunTime)
            {
                stunTime = hitWeaponData.stunTime;
            }

            // apply knockback
            rb.AddForce(forceDirection.normalized * hitWeaponData.knockBack);

            if (remainingHitPoints <= 0)
            {
                Die();
            }
        }

        public void Die()
        {
            if (player.enemiesInRange.Contains(this))
            {
                player.enemiesInRange.Remove(this);
            }

            Instantiate(sampleDrop, transform.position, Quaternion.identity);
            spriteController.HandleDeath();

            isDead = true;
            GameManager.instance.EnemyKilled();

            transform.parent.gameObject.SetActive(false);
        }
    }
}