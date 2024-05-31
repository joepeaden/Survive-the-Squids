using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using UnityEngine.Events;

namespace MyGame
{

    public class Enemy : MonoBehaviour
    {
        public int maxPickups;

        public static int EnemiesAlive;
        private static Dictionary<EnemyData, int> enemyTypePop = new Dictionary<EnemyData, int>();

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

        float randomPathfindingUpdateOffset;

        private void Awake()
        {
            GameplayManager.OnGameStart.AddListener(HandleGameStart);
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

            // add to enemy type population tracking
            if (!enemyTypePop.ContainsKey(data))
            {
                enemyTypePop[data] = 1;
            }
            else
            {
                enemyTypePop[data]++;
            }

            GameplayManager.Instance.enemies.Add(this);
        }

        private void OnEnable()
        {
            if (!isDead)
            {
                randomPathfindingUpdateOffset = Random.Range(0f, 1f);
                StartCoroutine(PathfindingCoroutine());
            }
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

            // if ever too far away, just respawn a new enemy
            if ((transform.position - player.transform.position).magnitude > 15f)
            {
                EnemiesAlive--;
                enemyTypePop[data]--;
                GameplayManager.Instance.enemies.Remove(this);
                Reset();
            }
        }

        IEnumerator PathfindingCoroutine()
        {
            if (targetCharacter == null)
            {
                yield return null;
            }

            pathfinder.destination = targetCharacter.transform.position;

            // just so they don't all obviously update at the same time
            yield return new WaitForSeconds(randomPathfindingUpdateOffset);

            float closeToPlayerDist = 3f;

            while (!isDead)
            {
                if (targetCharacter != null)
                {
                    float distToPlayer = (targetCharacter.transform.position - transform.position).magnitude;
                    if (distToPlayer > closeToPlayerDist)
                    {
                        float radius = 2f;
                        pathfinder.destination = targetCharacter.transform.position + new Vector3(Random.Range(-radius, radius), 0, Random.Range(-radius, radius));
                    }
                    else
                    {
                        pathfinder.destination = targetCharacter.transform.position;
                    }

                    // if close, should update path more often
                    if (distToPlayer > closeToPlayerDist * 2)
                    {
                        yield return new WaitForSeconds(1f);
                    }
                    else
                    {
                        yield return new WaitForSeconds(.25f);
                    }
                }

                yield return null;
            }
             //= GetDestination();

        }

        //Vector3 GetDestination()
        //{
        //    if ((targetCharacter.transform.position - transform.position).magnitude > 3f)
        //    {
        //        float radius = 1.5f;
        //        return targetCharacter.transform.position + new Vector3(Random.Range(-radius, radius), 0, Random.Range(-radius, radius));
        //    }
        //    else
        //    {
        //        return targetCharacter.transform.position;
        //    }

            //    Vector3 playerPosition = player.transform.position;
            //    Vector3 targetPosition = playerPosition + GetRandomOffset(1.5f); // 1.5f is the spread radius
            //    seeker.StartPath(transform.position, targetPosition, OnPathComplete);
        //}


        /// <summary>
        /// Get population of this enemy type
        /// </summary>
        /// <param name="theType"></param>
        /// <returns></returns>
        public static int GetPopOfEnemyType(EnemyData theType)
        {
            if (enemyTypePop.ContainsKey(theType))
            {
                return enemyTypePop[theType];
            }

            return 0;
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
            GameplayManager.OnGameStart.RemoveListener(HandleGameStart);
        }

        void HandleGameStart()
        {
            EnemiesAlive = 0;
            enemyTypePop.Clear();
            Reset();
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

        public void GetHit(WeaponData hitWeaponData, Vector2 forceDirection, bool isSlam, bool isStun, bool isCrit, float damageBuff)
        {
            lastWeaponThatHit = hitWeaponData;

            int damageWithBuff = hitWeaponData.damage + Mathf.CeilToInt(hitWeaponData.damage * damageBuff);

            // apply critical hit damage if it's a crit
            int damage = isCrit ? damageWithBuff * 3 : damageWithBuff;

            // if we have a crit weakness and it happens to be one, full damage goes through
            if (isCrit && data.vulnToCrits)
            {
                // lol, I just thought it was easier to read this way
                ;
            }
            // otherwise, reduce by the damage resistance
            else
            {
                damage = Mathf.FloorToInt(damage * (1-data.dmgResist));
            }

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

            if (!isDead)
            {
                spriteController.HandleHit(isCrit);
            }

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
            //GameObject audioSource = ObjectPool.instance.GetAudioSource();
            //audioSource.SetActive(true);
            //audioSource.GetComponent<PooledAudioSource>().SetData(impactAudio.clip, AudioGroups.death);

            if (Random.Range(0f, 1f) < data.chanceSpawnEnemyOnDeath)
            {
                EnemySpawner.SpawnEnemyAtPosition(transform.position, data.enemyToSpawnOnDeath);
            }

            //Instantiate(sampleDrop, transform.position, Quaternion.identity);

            if (SamplePickup.existingPickups < maxPickups)
            {
                GameObject sample = ObjectPool.instance.GetSample();
                sample.transform.position = transform.position;
                sample.SetActive(true);
                sample.GetComponent<SamplePickup>().Setup(false);
            }
            else
            {
                if (SamplePickup.megaSample == null)
                {
                    GameObject sample = ObjectPool.instance.GetSample();
                    sample.transform.position = transform.position;
                    sample.SetActive(true);
                    sample.GetComponent<SamplePickup>().Setup(true);
                }
                else
                {
                    SamplePickup.megaSample.XPValue++;
                }
            }
            //sample.GetComponent<PooledAudioSource>().SetData(levelUpSound, AudioGroups.pickup);

            spriteController.HandleDeath();

            isDead = true;
            GameplayManager.Instance.EnemyKilled();

            EnemiesAlive--;
            enemyTypePop[data]--;
            GameplayManager.Instance.enemies.Remove(this);

            StopCoroutine(PathfindingCoroutine());
            transform.parent.gameObject.SetActive(false);
        }
    }
}