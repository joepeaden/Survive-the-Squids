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

        public bool canAttack = true;
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
        private GameManager gameManager;
        private Enemy currentTarget;
        private float attackTimer;
        private WeaponData weaponData;
        private float reflexSpeed;

        public CharacterInfo CharInfo => charInfo;
        private CharacterInfo charInfo;
        public List<Enemy> enemiesInRange = new List<Enemy>();

        private int hitPoints;

        private void Start()
        {
            player = Player.instance;
            gameManager = GameManager.instance;
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
            if (weaponData == null)
            {
                return;
            }

            attackTimer -= Time.deltaTime;

            if (!gameManager.inMenu)
            {
                //if (ManualAimEnabled)
                //{
                //    UpdateRotation();

                //    if (Input.GetMouseButtonDown(0))
                //    {
                //        Attack();
                //    }
                //}
                //else
                //{
                    
                    if (currentTarget == null || (currentTarget != null && currentTarget.isDead))
                    {
                        if (currentTarget != null)
                        {
                            enemiesInRange.Remove(currentTarget);
                        }

                        currentTarget = GetEnemy();
                    }

                UpdateRotation();

                if (currentTarget != null)// || weaponData.controlStyle == ControlStyle.moveDirection)
                {

                        if (canAttack && attackTimer <= 0)
                        {
                            Attack();
                        }
                    }
                //}
            }
        }


        public Enemy GetEnemy()
        {
            if (enemiesInRange.Count == 0)
            {
                return null;
            }

            int randomIndex = Random.Range(0, enemiesInRange.Count);
            return enemiesInRange[randomIndex];
        }

        public void SetBodyActive(bool shouldEnable)
        {
            transform.parent.gameObject.SetActive(shouldEnable);
        }

        private void UpdateRotation()
        {
            Vector3 targetDir = GetTargetDirection();
            Quaternion newRotation = Quaternion.LookRotation(Vector3.forward, upwards: targetDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime * reflexSpeed);
        }

        private void Attack()
        {
            float angle = -weaponData.projSpreadAngle / 2;
            Quaternion projectileRotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z + angle);

            for (int i = 0; i < weaponData.projPerShot; i++)
            {
                GameObject projectileGO = ObjectPool.instance.GetProjectile();
                projectileGO.SetActive(true);
                projectileGO.transform.position = transform.position;
                projectileGO.transform.rotation = projectileRotation;

                Projectile projectile = projectileGO.GetComponent<Projectile>();
                projectile.firedFromPlayer = true;
                projectile.lifeSpan = 10f;
                projectile.SetData(weaponData);

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

                            // add proj damage and character damage buff (from trait)
                            int damage = projectile.damage + charInfo.DamageBuff;

                            // roll for crit, if so then 3x damage
                            float critRoll = Random.Range(0f, 1f);
                            if (critRoll < charInfo.CritChance)
                            {
                                damage *= 3;
                            }

                            enemy.GetHit(weaponData, (enemy.transform.position - transform.position).normalized, charInfo.hasSlamRounds, charInfo.hasStunRounds);
                            enemiesHit++;

                            if (enemy.isDead)
                            {
                                charInfo.AddXP(enemy.data.xpReward);
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

                angle += weaponData.projSpreadAngle / weaponData.projPerShot;
            }

            attackTimer = weaponData.attackInterval;

            weaponSpriteScript.PlayFireAnim();
        }

        private Vector3 GetTargetDirection()
        {
            //if (ManualAimEnabled)
            //{
            //    Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //    return (mousePos - transform.position).normalized;
            //}
            //else

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
            hitPoints -= damage;

            //if (hitPoints <= 0)
            //{
            //    Die();
            //}
            //else
            //{
                charSpriteScript.HandleHit();
            //}
        }

        public void Die()
        {
            isDead = true;
            currentTarget = null;
            SetBodyActive(false);
            player.ActiveCharacters.Remove(this);

            player.CheckGameOver();
        }

        public void Reset()
        {
            isDead = false;
            SetBodyActive(false);
            charSpriteScript.Reset();

            if (gameManager == null)
            {
                gameManager = GameManager.instance;
            }
        }

        public void SetCharacter(CharacterInfo info)
        {
            charInfo = info;

            SetWeapon(charInfo.weaponData);
            charName = charInfo.charName;
            reflexSpeed = charInfo.ReflexSpeed;
            hitPoints = charInfo.TotalHitPoints;
        }

        [SerializeField] CircleCollider2D rangeTrigger;
        public void SetWeapon(WeaponData newWeapon)
        {
            weaponData = newWeapon;
            weaponSpriteScript.SetWeapon(newWeapon);
            rangeTrigger.radius = newWeapon.range;
        }
    }
}