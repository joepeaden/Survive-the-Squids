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
            gameObject.SetActive(false);
            remainingHitPoints = data.hitPoints;
        }

        private void PickTarget()
        {
            float smallestDist = float.MaxValue;
            for (int i = 0; i < player.CharacterBodies.Count; i++)
            {
                if (player.CharacterBodies[i].isDead)
                {
                    continue;
                }

                float dist = (transform.position - player.CharacterBodies[i].transform.position).magnitude;
                if (dist < smallestDist)
                {
                    smallestDist = dist;
                    targetCharacter = player.CharacterBodies[i];
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

            isDead = true;
            GameManager.instance.EnemyKilled();
            gameObject.SetActive(false);
        }
    }
}