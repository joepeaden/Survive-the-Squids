using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame
{

    public class Projectile : MonoBehaviour
    {
        
        public SpriteRenderer spriteRenderer;

        [HideInInspector]
        public int projectileVelocity;
        [HideInInspector]
        public Vector2 projectileDirection;
        [HideInInspector]
        public bool useProjectilePhysics;
        [HideInInspector]
        public bool firedFromPlayer;
        [HideInInspector]
        public float lifeSpan = 10f;
        [HideInInspector]
        public int damage;

        [SerializeField] AudioSource fireSoundSource;

        private Rigidbody2D rb;
        private float spawnTime;

        private WeaponData _data;

        public int penetration = 0;

        private CharacterInfo _charInfo;

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        private void OnEnable()
        {
            spawnTime = Time.time;
        }

        void FixedUpdate()
        {
            if (!_data.useProjPhys && (Time.time - spawnTime) > lifeSpan)
            {
                gameObject.SetActive(false);
            }

            rb.velocity = _data.projectileVelocity * transform.up;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            // doesn't account for penetrator rounds - just keep it in mind.

            if (_data.useProjPhys)
            {
                if (collision.tag == "Player" && !firedFromPlayer)
                {
                    Debug.Log("Hit a Player character");
                }
                else if (collision.tag == "Enemy" && firedFromPlayer)
                {
                    bool isCrit = false;
                    // roll for crit, if so then 3x damage
                    float critRoll = Random.Range(0f, 1f);
                    if (critRoll < (_charInfo.CritChance + _data.critChance))
                    {
                        //damage *= 3;
                        isCrit = true;
                    }

                    Enemy enemy = collision.GetComponent<Enemy>();

                    enemy.GetHit(_data, (enemy.transform.position - transform.position).normalized, _charInfo.hasSlamRounds, _charInfo.hasStunRounds, isCrit, _charInfo.DamageBuff); ;

                    if (enemy.isDead)
                    {
                        _charInfo.TallyKill(enemy.data);
                    }
                }
            }
        }

        public void SetData(WeaponData data, CharacterInfo charInfo)
        {
            _data = data;
            _charInfo = charInfo;

            spriteRenderer.sprite = data.projSprite;

            //fireSoundSource.clip = data.weaponFireSound;
            //fireSoundSource.Play();
        }
    }
}