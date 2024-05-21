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
        private CharacterBody targetCharacter;
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

        WeaponData lastWeaponThatHit;
        float bleedTimeRemaining;

        private void Awake()
        {
            GameplayManager.OnGameStart.AddListener(Reset);
        }

        private void Start()
        {
            pathfinder = GetComponent<AIPath>();
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
            GameplayManager.Instance.enemies.Add(this);
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
                    pathfinder.destination = targetCharacter.transform.position;

                    float distFromTarget = (transform.position - targetCharacter.transform.position).magnitude;
                    float attackDist = .5f;
                    //float attackDist = targetCharacter.hitCircle.localScale.x/2;

                    if (distFromTarget < attackDist && attackTimer <= 0)
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

        IEnumerator HandleBleed()
        {
            do
            {
                remainingHitPoints -= lastWeaponThatHit.bleedDamage;
                HandleDamge(lastWeaponThatHit.bleedDamage, isBleed: true);
                yield return new WaitForSeconds(1f);
                bleedTimeRemaining -= 1f;
            } while (bleedTimeRemaining > 0f);
        }

        private void OnDestroy()
        {
            GameplayManager.OnGameStart.RemoveListener(Reset);
        }

        private void Reset()
        {
            transform.parent.gameObject.SetActive(false);
            remainingHitPoints = data.hitPoints;
        }

        public void SetData(EnemyData newData)
        {
            data = newData;
            spriteController.SetData(newData);
            Initialize();
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

        public void GetHit(WeaponData hitWeaponData, Vector2 forceDirection, bool isSlam, bool isStun, bool isCrit)
        {
            lastWeaponThatHit = hitWeaponData;

            int damage = isCrit ? hitWeaponData.damage * 3 : hitWeaponData.damage;
            remainingHitPoints -= damage;

            float newStunTime = hitWeaponData.stunTime + (isStun ? 2 : 0);
            // set stunTime, but only change it if the new value is greater than the old.
            if (stunTime < newStunTime)
            {
                stunTime = newStunTime;
            }

            if (hitWeaponData.causesBleed)
            {
                bleedTimeRemaining += hitWeaponData.bleedTime;
                StartCoroutine(HandleBleed());
            }

            // apply knockback
            rb.AddForce(forceDirection.normalized * (hitWeaponData.knockBack + (isSlam ? 100f : 0)));

            HandleDamge(damage, isCrit);
        }

        void HandleDamge(int damage, bool isCrit = false, bool isBleed = false)
        {
            OnGetHit.Invoke();

            spriteController.HandleHit(isCrit);

            // should probably move all this into the TextFloatUp script or whatever. just pass in enum for damage type.
            string floatText = "0";
            Color textColor = Color.white;
            if (isCrit)
            {
                ColorUtility.TryParseHtmlString("#ffffff", out textColor);
                floatText = "CRIT!";
            }
            else if (isBleed)
            {
                ColorUtility.TryParseHtmlString("#ff0100", out textColor);
                floatText = "BLEED!";
            }
            else
            {
                ColorUtility.TryParseHtmlString("#b4d8f7", out textColor);
                floatText = damage.ToString();
            }

            GameplayUI.Instance.AddTextFloatup(transform.position, floatText, textColor);

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
            GameplayManager.Instance.EnemyKilled();

            EnemiesAlive--;
            GameplayManager.Instance.enemies.Remove(this);

            transform.parent.gameObject.SetActive(false);
        }
    }
}