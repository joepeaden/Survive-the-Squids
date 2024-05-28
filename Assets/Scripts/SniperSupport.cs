using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame
{
    public class SniperSupport : MonoBehaviour
    {
        public WeaponData weaponData;
        //public List<Enemy> enemiesInRange = new List<Enemy>();
        private Enemy currentTarget;
        private float attackTimer;
        public float baseLaserDesignateTime;
        public LineRenderer line;
        int ammoInWeapon;

        enum PreferTarget { highestHP, closest }
        [SerializeField] PreferTarget targetPref;
        [SerializeField] bool useLaserSight;

        [SerializeField] WeaponSprite weaponSprite;

        Camera mainCamera;

        private void Awake()
        {
            GameplayManager.OnGameStart.AddListener(HandleGameStart);
            mainCamera = Camera.main;
        }

        private void OnDestroy()
        {
            GameplayManager.OnGameStart.RemoveListener(HandleGameStart);
        }

        private void OnEnable()
        {
            ammoInWeapon = weaponData.magSize;
            if (line != null)
            {
                line.enabled = false;
            }
        }

        void HandleGameStart()
        {
            Destroy(gameObject);
        }
    

        private void Update()
        {
            if (weaponData == null)
            {
                return;
            }

            attackTimer -= Time.deltaTime;
            if (currentTarget != null && currentTarget.isDead)
            {
                //enemiesInRange.Remove(currentTarget);
                currentTarget = null;
            }

            //if (currentTarget == null)
            //{
                currentTarget = GetEnemy();
            //}

            if (currentTarget != null)
            {
                UpdateRotation();

                // if we just waited for reload because no bullets left, then reload the weapon
                if (ammoInWeapon <= 0 && attackTimer <= 0)
                {
                    ammoInWeapon = weaponData.magSize;
                }

                if (useLaserSight && attackTimer <= baseLaserDesignateTime)
                {
                    UpdateDesignationLine();
                }

                if (attackTimer <= 0)
                {
                    // just in case - in Attack we're adding to the attack timer not setting it
                    attackTimer = 0;
                    Attack();
                    if (line != null)
                    {
                        line.enabled = false;
                    }
                }
            }
        }

        private void UpdateDesignationLine()
        {
            line.enabled = true;
            Ray2D ray = new Ray2D(transform.position, transform.up);
            int layerMask = ~LayerMask.GetMask("Projectiles", "Boundary", "Characters", "Pickups", "Obstacles", "CharacterTargetDetector", "ObstaclesNoBlockLOS");
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, int.MaxValue, layerMask);

            if (hit)
            {
                line.enabled = true;
                line.SetPosition(0, transform.position);
                line.SetPosition(1, hit.point);
            }
        }

        /// <summary>
        /// Coroutine that projects a raycast from the weapon's position in the direction it is facing, and moves the aim light to the first collision point.
        /// </summary>
        private IEnumerator ProjectRayCastAndMoveAimGlowToFirstCollision()
        {
            //while (aiming)
            //{
            //Ray2D ray = new Ray2D(transform.position, transform.up);
            //int layerMask = ~LayerMask.GetMask("Projectiles", "Boundary", "Characters");
            //RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, int.MaxValue, layerMask);

            //if (hit)
            //{
            //    //aimGlow.transform.position = hit.point;

            //    line.enabled = true;
            //    line.SetPosition(0, transform.position);
            //    line.SetPosition(1, hit.point);
            //}

            yield return null;
            //}
        }

        private void UpdateRotation()
        {
            Vector3 targetDir = GetTargetDirection();
            Quaternion newRotation = Quaternion.LookRotation(Vector3.forward, upwards: targetDir);
            transform.rotation = newRotation;
        }


        public Enemy GetEnemy()
        {
            switch (targetPref)
            {
                case PreferTarget.highestHP:
                    int highestHP = int.MinValue;
                    Enemy strongestEnemy = null;
                    foreach (Enemy e in GameplayManager.Instance.enemies)
                    {
                        // make sure to shoot an enemy in player view
                        bool inCamView = mainCamera.WorldToViewportPoint(e.transform.position).x > 0 && mainCamera.WorldToViewportPoint(e.transform.position).x < 1 && mainCamera.WorldToViewportPoint(e.transform.position).y < 1 && mainCamera.WorldToViewportPoint(e.transform.position).y > 0;
                        // make sure it's in range
                        bool inRange = (e.transform.position - transform.position).magnitude <= weaponData.range;

                        if (inCamView)
                        {
                            if (e.HitPoints > highestHP)
                            {
                                highestHP = e.HitPoints;
                                strongestEnemy = e;
                            }
                        }
                    }
                    return strongestEnemy;

                case PreferTarget.closest:
                    float closestDist = float.MaxValue;
                    Enemy closestEnemy = null;
                    foreach (Enemy e in GameplayManager.Instance.enemies)
                    {
                        // make sure to shoot an enemy in player view
                        bool inCamView = mainCamera.WorldToViewportPoint(e.transform.position).x > 0 && mainCamera.WorldToViewportPoint(e.transform.position).x < 1 && mainCamera.WorldToViewportPoint(e.transform.position).y < 1 && mainCamera.WorldToViewportPoint(e.transform.position).y > 0;

                        // make sure it's in range
                        float distToEnemy = (transform.position - e.transform.position).magnitude;
                        bool inRange = distToEnemy <= weaponData.range;

                        if (inCamView && inRange && distToEnemy < closestDist)
                        {
                            closestDist = distToEnemy;
                            closestEnemy = e;
                        }
                    }
                    return closestEnemy;

                default:
                    // pick random target by default
                    int randomIndex = Random.Range(0, GameplayManager.Instance.enemies.Count);
                    return GameplayManager.Instance.enemies[randomIndex];
            }
        }

        private void Attack()
        {
            float angle = -weaponData.projSpreadAngle / 2;

            for (int i = 0; i < weaponData.projPerShot; i++)
            {
                Quaternion projectileRotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z + angle);

                GameObject projectileGO = ObjectPool.instance.GetProjectile();
                projectileGO.SetActive(true);
                projectileGO.transform.position = transform.position;
                projectileGO.transform.rotation = projectileRotation;

                Projectile projectile = projectileGO.GetComponent<Projectile>();
                projectile.firedFromPlayer = true;
                projectile.lifeSpan = 10f;
                projectile.SetData(weaponData);

                if (weaponSprite != null)
                {
                    weaponSprite.PlayFireAnim();
                }

                // only use one audio source for something like a shotgun w/ multiple proj per shot
                if (i == 0)
                {
                    GameObject audioSource = ObjectPool.instance.GetAudioSource();
                    audioSource.SetActive(true);
                    //audioSource.GetComponent<AudioSource>().clip = weaponData.weaponFireSound;
                    //audioSource.GetComponent<AudioSource>().Play();
                    AudioClip fireSound = weaponData.weaponFireSounds[Random.Range(0, weaponData.weaponFireSounds.Count)];
                    audioSource.GetComponent<PooledAudioSource>().SetData(fireSound, AudioGroups.projectiles);
                }

                if (!weaponData.useProjPhys)
                {
                    int enemiesHit = 0;
                    RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, projectile.transform.up, 100f, ~LayerMask.GetMask("Projectiles", "Boundary", "Characters"));
                    foreach (RaycastHit2D hit in hits)
                    {
                        if (hit && hit.transform.GetComponent<Enemy>() != null)
                        {
                            Enemy enemy = hit.transform.GetComponent<Enemy>();

                            // roll for crit, if so then 3x damage
                            //bool isCrit = false;
                            //float critRoll = Random.Range(0f, 1f);
                            //if (critRoll < charInfo.CritChance)
                            //{
                            //    damage *= 3;
                            //}

                            enemy.GetHit(weaponData, (enemy.transform.position - transform.position).normalized, false, false, false);
                            enemiesHit++;

                            //if (enemy.isDead)
                            //{
                            //    charInfo.AddXP(enemy.data.xpReward);
                            //}

                            // If penetrator rounds then can hit multiple targets. Otherwise exit here.
                            //if (!charInfo.hasPenetratorRounds || enemiesHit > 1)
                            //{
                            //    projectile.lifeSpan = Mathf.Abs((transform.position - hit.transform.position).magnitude) / projectile.projectileVelocity;
                            //    break;
                            //}
                        }
                    }
                }

                angle += weaponData.projSpreadAngle / weaponData.projPerShot;
            }

            ammoInWeapon--;
            // if still ammo, wait standard time between shots; if no ammo, then wait for reload
            if (ammoInWeapon > 0)
            {
                attackTimer += weaponData.attackInterval;
            }
            else
            {
                attackTimer += weaponData.reloadTime;
            }
        }

        private Vector3 GetTargetDirection()
        {
            //if (ManualAimEnabled)
            //{
            //    Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            //    return (mousePos - transform.position).normalized;
            //}
            //else

            //if (weaponData.controlStyle == ControlStyle.moveDirection)
            //{
                return (currentTarget.transform.position - transform.position).normalized;
            
        }

    }
}


        //private Player player;
        //private GameManager gameManager;
        //private WeaponData weaponData;
        //private float reflexSpeed;

        //public CharacterInfo CharInfo => charInfo;
        //private CharacterInfo charInfo;
        //public List<Enemy> enemiesInRange = new List<Enemy>();

        //private int hitPoints;

        //int ammoInWeapon;

        //private void Start()
        //{
        //    player = Player.instance;
        //    gameManager = GameManager.instance;
        //}

      
    