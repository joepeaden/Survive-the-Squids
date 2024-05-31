using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame
{

    public class CharacterBody : MonoBehaviour
    {
        /// <summary>
        /// Whether or not player is manually aiming
        /// </summary>
        public static bool ManualAimEnabled;

        public bool isDead;
        //[SerializeField]
        //private SpriteRenderer weaponSprite;

        [SerializeField]
        private WeaponSprite weaponSpriteScript;
        [SerializeField]
        CharacterSprite charSpriteScript;

        public WeaponData CurrentWeapon => weaponData;
        [HideInInspector]
        public string charName;

        private Player player;
        private GameplayManager gameplayManager;
        private Enemy currentTarget;
        private float attackTimer;
        private WeaponData weaponData;

        public CharacterInfo CharInfo => charInfo;
        private CharacterInfo charInfo;
        public List<Enemy> enemiesInRange = new List<Enemy>();
        [SerializeField] bool isInvincible;
        private int hitPoints;

        int ammoInWeapon;

        [SerializeField] AudioClip levelUpSound;

        Camera mainCamera;

        private void Start()
        {
            player = Player.instance;
            gameplayManager = GameplayManager.Instance;
            mainCamera = Camera.main;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.tag == "Enemy")
            {
                enemiesInRange.Add(collision.GetComponent<Enemy>());
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.tag == "Enemy")
            {
                enemiesInRange.Remove(collision.GetComponent<Enemy>());
            }
        }

        private void Update()
        {
            //Debug.Log(charName + " HP: " + hitPoints);

            if (weaponData == null)
            {
                return;
            }

            attackTimer -= Time.deltaTime;

            if (!gameplayManager.inMenu)
            {
                if (ManualAimEnabled)
                {
                    UpdateRotation();

                    if (Input.GetMouseButton(0))
                    {
                        if (attackTimer <= 0)
                        {
                            // if we just waited for reload because no bullets left, then reload the weapon
                            if (ammoInWeapon <= 0 && attackTimer <= 0)
                            {
                                ammoInWeapon = weaponData.magSize;
                            }

                            // just in case - in Attack we're adding to the attack timer not setting it
                            attackTimer = 0;
                            Attack();
                        }
                    }
                }
                else
                {

                    if (currentTarget != null && currentTarget.isDead)
                    {
                        enemiesInRange.Remove(currentTarget);
                        currentTarget = null;
                    }


                    //if (currentTarget == null || (currentTarget != null && currentTarget.isDead))
                    //{
                    //    if (currentTarget != null)
                    //    {
                    //        enemiesInRange.Remove(currentTarget);
                    //    }

                    Enemy oldTarget = currentTarget;
                    currentTarget = GetEnemy();
                        //}

                    if (oldTarget == null && currentTarget != null)
                    {
                        // adding random time before firing at first target so that all characters don't fire at once (sound concerns)
                        attackTimer += Random.Range(0, .2f);
                    }

                    UpdateRotation();


                    // if we just waited for reload because no bullets left, then reload the weapon
                    if (ammoInWeapon <= 0 && attackTimer <= 0)
                    {
                        ammoInWeapon = weaponData.magSize;
                    }

                    if (currentTarget != null)// || weaponData.controlStyle == ControlStyle.moveDirection)
                    {
                        if (attackTimer <= 0)
                        {
                            // just in case - in Attack we're adding to the attack timer not setting it
                            attackTimer = 0;


                            Attack();
                        }
                    }
                }
            }
        }


        public Enemy GetEnemy()
        {
            if (enemiesInRange.Count == 0)
            {
                return null;
            }

            // to shoot at random:
            //int randomIndex = Random.Range(0, enemiesInRange.Count);
            //return enemiesInRange[randomIndex];

            float closestDist = float.MaxValue;
            Enemy closestEnemy = null;
            foreach (Enemy e in enemiesInRange)
            {
                Vector2 vectorToEnemy = e.transform.position - transform.position;
                //Debug.DrawRay(transform.position, vectorToEnemy.normalized, Color.green, .5f);
                RaycastHit2D hit = Physics2D.Raycast(transform.position, vectorToEnemy.normalized, 1000f, LayerMask.GetMask("Obstacles", "Enemies"));
                if (hit && hit.transform.tag == "Enemy")
                {
                    float distToEnemy = vectorToEnemy.magnitude;
                    if (distToEnemy < closestDist)
                    {
                        closestDist = distToEnemy;
                        closestEnemy = e;
                    }
                }
            }

            return closestEnemy;
        }

        public void SetBodyActive(bool shouldEnable)
        {
            transform.parent.gameObject.SetActive(shouldEnable);
        }

        private void UpdateRotation()
        {
            Vector3 targetDir = GetTargetDirection();
            Quaternion newRotation = Quaternion.LookRotation(Vector3.forward, upwards: targetDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime * charInfo.ReflexSpeed);
        }

        public void PlayLevelUpSound()
        {
            GameObject audioSource = ObjectPool.instance.GetAudioSource();
            audioSource.SetActive(true);
            audioSource.GetComponent<PooledAudioSource>().SetData(levelUpSound, AudioGroups.pickup);
        }

        private void Attack()
        {
            float angle = -(weaponData.projSpreadAngle+charInfo.ProjSpreadMod) / 2;
            
            for (int i = 0; i < (weaponData.projPerShot + CharInfo.ProjNumBuff); i++)
            {
                Quaternion projectileRotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z + angle);

                GameObject projectileGO = ObjectPool.instance.GetProjectile();
                projectileGO.SetActive(true);
                projectileGO.transform.position = transform.position;
                projectileGO.transform.rotation = projectileRotation;

                Projectile projectile = projectileGO.GetComponent<Projectile>();
                projectile.firedFromPlayer = true;
                projectile.lifeSpan = 10f;
                projectile.SetData(weaponData, charInfo);

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

                // projectile doesn't account for it, it's only implemented for Raycast for now!
                //projectile.penetration = charInfo.hasPenetratorRounds ? 1 : 0;

                if (charInfo.hasPenetratorRounds)
                {
                    projectile.spriteRenderer.color = Color.red;
                }
                else if (charInfo.hasStunRounds)
                {
                    projectile.spriteRenderer.color = Color.blue;
                }
                else if (charInfo.hasSlamRounds)
                {
                    projectile.spriteRenderer.color = Color.green;
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

                            bool isCrit = false;
                            // roll for crit, if so then 3x damage
                            float critRoll = Random.Range(0f, 1f);
                            if (critRoll < (charInfo.CritChance + weaponData.critChance))
                            {
                                //damage *= 3;
                                isCrit = true;
                            }

                            enemy.GetHit(weaponData, (enemy.transform.position - transform.position).normalized, charInfo.hasSlamRounds, charInfo.hasStunRounds, isCrit, charInfo.DamageBuff);
                            enemiesHit++;

                            if (enemy.isDead)
                            {
                                charInfo.TallyKill(enemy.data);
                            }

                            // If penetrator rounds then can hit multiple targets. Otherwise exit here.
                            if (!charInfo.hasPenetratorRounds || enemiesHit > 1)
                            {
                                projectile.lifeSpan = Mathf.Abs((transform.position - hit.transform.position).magnitude) / projectile.projectileVelocity;
                                break;
                            }
                        }
                    }
                }

                angle += (weaponData.projSpreadAngle + charInfo.ProjSpreadMod) / (weaponData.projPerShot + CharInfo.ProjNumBuff);
            }

            weaponSpriteScript.PlayFireAnim();

            ammoInWeapon--;
            // if still ammo, wait standard time between shots; if no ammo, then wait for reload
            if (ammoInWeapon > 0)
            {
                attackTimer += weaponData.attackInterval - (weaponData.attackInterval * charInfo.FireRateBuff);
            }
            else
            {
                attackTimer += weaponData.reloadTime - (weaponData.reloadTime * charInfo.ReloadTimeReduction);
            }
            
        }

        private Vector3 GetTargetDirection()
        {
            if (ManualAimEnabled)
            {
                Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                return (mousePos - transform.position).normalized;
            }
            else

            //if (weaponData.controlStyle == ControlStyle.moveDirection)
            //{
            if (currentTarget != null)
            {
                return (currentTarget.transform.position - transform.position).normalized;
            }
            else
            {
                return player.MoveDirection.normalized;
            }
        }

        public void GetHit(int damage)
        {
            if (isInvincible)
            {
                return;
            }

            hitPoints -= damage;

            if (hitPoints <= 0)
            {
                Die();
            }
            else
            {
                charSpriteScript.HandleHit();
            }
        }

        public void Disable()
        {
            isDead = false;
            currentTarget = null;
            SetBodyActive(false);
            //player.ActiveCharacters.Remove(this);
        }

        public void Die()
        {
            isDead = true;
            currentTarget = null;
            SetBodyActive(false);
            player.RemoveCharacter(charInfo);
            player.CheckGameOver();
        }

        public void Reset()
        {
            isDead = false;
            SetBodyActive(false);
            charSpriteScript.Reset();

            if (gameplayManager == null)
            {
                gameplayManager = GameplayManager.Instance;
            }
        }

        public void RefreshCharacter()
        {
            SetWeapon(charInfo.weaponData);
            charName = charInfo.charName;
            hitPoints = charInfo.TotalHitPoints;
        }

        public void SetCharacter(CharacterInfo info)
        {
            charInfo = info;
            info.currentBody = this;
            RefreshCharacter();
        }

        [SerializeField] CircleCollider2D rangeTrigger;
        public void SetWeapon(WeaponData newWeapon)
        {
            weaponData = newWeapon;
            weaponSpriteScript.SetWeapon(newWeapon);
            rangeTrigger.radius = newWeapon.range + (newWeapon.range * CharInfo.RangeBuff);
            ammoInWeapon = newWeapon.magSize;
        }
    }
}