using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame
{

    public class Projectile : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer spriteRenderer;

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

        private Rigidbody2D rb;
        private float spawnTime;

        private WeaponData _data;

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
            if (_data.useProjPhys)
            {
                if (collision.tag == "Player" && !firedFromPlayer)
                {
                    Debug.Log("Hit a Player character");
                }
                else if (collision.tag == "Enemy" && firedFromPlayer)
                {
                    collision.GetComponent<Enemy>().GetHit(_data, (collision.transform.position - transform.position).normalized);
                }
            }
        }

        public void SetData(WeaponData data)
        {
            _data = data;

            spriteRenderer.sprite = data.projSprite;
        }
    }
}