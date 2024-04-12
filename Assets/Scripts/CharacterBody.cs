using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBody : MonoBehaviour
{
    public bool isPlayerControlled;
    public bool canAttack = true;
    public bool isDead;
    [SerializeField]
    private SpriteRenderer weaponSprite;

    [SerializeField]
    private WeaponSprite weaponSpriteScript;

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

    private int hitPoints;

    private void Start()
    {
        player = Player.instance;
        gameManager = GameManager.instance;
    }

    private void Update()
    {
        attackTimer -= Time.deltaTime;

        if (!gameManager.inMenu)
        {
            if (isPlayerControlled)
            {
                UpdateRotation();

                if (Input.GetMouseButtonDown(0))
                {
                    Attack();
                }
            }
            else
            {
                if (currentTarget == null || currentTarget.isDead)
                {
                    currentTarget = player.GetEnemy();
                }

                if (currentTarget != null)
                {
                    UpdateRotation();

                    if (canAttack && attackTimer <= 0)
                    {
                        Attack();
                    }
                }
            }
        }
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

            if (!weaponData.useProjPhys)
            {
                RaycastHit2D hit = Physics2D.Raycast(transform.position, projectile.transform.up, 100f, ~LayerMask.GetMask("Projectiles", "Boundary", "Characters"));
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

                    enemy.GetHit(weaponData, (enemy.transform.position - transform.position).normalized);
                    projectile.lifeSpan = Mathf.Abs((transform.position - hit.transform.position).magnitude) / projectile.projectileVelocity;

                    if (enemy.isDead)
                    {
                        charInfo.AddXP(enemy.data.xpReward);
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
        if (isPlayerControlled)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            return (mousePos - transform.position).normalized;
        }
        else
        {
            return (currentTarget.transform.position - transform.position).normalized;
        }
    }

    public void GetHit(int damage)
    {
        hitPoints -= damage;

        if (hitPoints <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        isDead = true;
        currentTarget = null;
        gameObject.SetActive(false);
    }

    public void Reset()
    {
        isDead = false;
        gameObject.SetActive(true);

        if (gameManager == null)
        {
            gameManager = GameManager.instance;
        }
    }

    public void SetCharacter(CharacterInfo info)
    {
        charInfo = info;

        weaponData = charInfo.weaponData;
        charName = charInfo.charName;
        reflexSpeed = charInfo.ReflexSpeed;
        hitPoints = charInfo.TotalHitPoints;
    }

    public void SetWeapon(WeaponData newWeapon)
    {
        weaponData = newWeapon;
    }
}
