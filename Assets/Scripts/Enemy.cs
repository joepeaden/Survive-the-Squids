using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using UnityEngine.Events;

namespace MyGame
{

    public class Enemy : MonoBehaviour
    {
        public static int EnemiesAlive;

        public bool isDead;
        public EnemyData data;

        public int HitPoints => remainingHitPoints;
        private int remainingHitPoints;
        private float attackTimer;
        private Player player;
        //private CharacterBody targetCharacter;
        private AIPath pathfinder;
        private Rigidbody2D rb;
        private float stunTime;

        [SerializeField]
        GameObject sampleDrop;
        [SerializeField]
        EnemySprite spriteController;

        [HideInInspector]
        public UnityEvent OnGetHit = new UnityEvent();

        [SerializeField] AudioSource impactAudio;

        private void Start()
        {
            pathfinder = GetComponent<AIPath>();
            GameManager.instance.OnGameStart.AddListener(Reset);
            rb = GetComponent<Rigidbody2D>();
        }

        private void Initialize()
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

            EnemiesAlive++;
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

                if (player != null)
                {
                    pathfinder.destination = player.transform.position;

                    float distFromTarget = (transform.position - player.transform.position).magnitude;
                    float attackDist = player.hitCircle.localScale.x/2;

                    if (distFromTarget < attackDist && attackTimer <= 0)
                    {
                        player.GetHit(data.damage);
                        attackTimer = data.attackInterval;

                        //if (targetCharacter.isDead)
                        //{
                        //    PickTarget();
                        //}
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
            Initialize();
        }

        private void PickTarget()
        {
            //float smallestDist = float.MaxValue;
            //for (int i = 0; i < player.ActiveCharacters.Count; i++)
            //{
            //    if (player.ActiveCharacters[i].isDead || !player.ActiveCharacters[i].isActiveAndEnabled)
            //    {
            //        continue;
            //    }

            //    float dist = (transform.position - player.ActiveCharacters[i].transform.position).magnitude;
            //    if (dist < smallestDist)
            //    {
            //        smallestDist = dist;
            //        targetCharacter = player.ActiveCharacters[i];
            //    }
            //}
        }

        public void GetHit(WeaponData hitWeaponData, Vector2 forceDirection, bool isSlam, bool isStun)
        {

            remainingHitPoints -= hitWeaponData.damage;

            float newStunTime = hitWeaponData.stunTime + (isStun ? 2 : 0);
            // set stunTime, but only change it if the new value is greater than the old.
            if (stunTime < newStunTime)
            {
                stunTime = newStunTime;
            }

            // apply knockback
            rb.AddForce(forceDirection.normalized * (hitWeaponData.knockBack + (isSlam ? 100f : 0)));

            OnGetHit.Invoke();

            if (remainingHitPoints <= 0)
            {
                Die();
            }
        }

        public void Die()
        {

            //impactAudio.Play();
            GameObject audioSource = ObjectPool.instance.GetAudioSource();
            audioSource.SetActive(true);
            //audioSource.GetComponent<AudioSource>().clip = weaponData.weaponFireSound;
            //audioSource.GetComponent<AudioSource>().Play();
            audioSource.GetComponent<PooledAudioSource>().SetData(impactAudio.clip, AudioGroups.death);
            //if (player.enemiesInRange.Contains(this))
            //{
            //    player.enemiesInRange.Remove(this);
            //}

            Instantiate(sampleDrop, transform.position, Quaternion.identity);
            spriteController.HandleDeath();

            isDead = true;
            GameManager.instance.EnemyKilled();

            EnemiesAlive--;

            transform.parent.gameObject.SetActive(false);
        }
    }
}