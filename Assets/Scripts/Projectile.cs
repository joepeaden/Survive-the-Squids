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
            if (_charInfo != null)
            {
                float distTravelled = (transform.position - startLocation).magnitude;
                if (distTravelled > (_charInfo.weaponData.range + _charInfo.RangeBuff))
                {
                    gameObject.SetActive(false);
                }
            }

            if (!_data.useProjPhys && (Time.time - spawnTime) > lifeSpan)
            {
                gameObject.SetActive(false);
            }

            rb.velocity = _data.projectileVelocity * transform.up;
        }

        public Vector3 startLocation;
        private void OnTriggerEnter2D(Collider2D collision)
        {

            if (_data.useProjPhys)
            {
                if (collision.tag == "Player" && !firedFromPlayer)
                {
                    Debug.Log("Hit a Player character");
                }
                else if (collision.tag == "Enemy" && firedFromPlayer)
                {
                    Enemy enemy = collision.GetComponent<Enemy>();

                    enemy.GetHit(_charInfo, (enemy.transform.position - transform.position).normalized); ;

                    if (enemy.isDead)
                    {
                        _charInfo.TallyKill(enemy.data);
                    }

                    penetration--;

                    if (penetration < 0)
                    {
                        gameObject.SetActive(false);
                    }
                }
                else if (collision.tag == "Boundary")
                {
                    gameObject.SetActive(false);
                }
            }
        }

        public void SetData(WeaponData data, CharacterInfo charInfo)
        {
            _data = data;
            _charInfo = charInfo;

            if (charInfo != null)
            {
                penetration = _charInfo.penetrationBuff;
            }

            spriteRenderer.sprite = data.projSprite;

            //fireSoundSource.clip = data.weaponFireSound;
            //fireSoundSource.Play();
        }
    }
}