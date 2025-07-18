using UnityEngine;

namespace MyGame
{
    public class Pickup : MonoBehaviour
    {
        public static int existingPickups;

        [SerializeField]
        protected PickupData data;

        protected Player player;

        [SerializeField] 
        private SpriteRenderer _rend;

        protected virtual void Awake()
        {
            GameplayManager.OnGameStart.AddListener(RemovePickup);


        }

        private void Start()
        {
            player = Player.instance;
        }

        protected virtual void OnEnable()
        {
            existingPickups++;
            _rend.sprite = data.sprite;
        }

        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            GameObject audioSource = ObjectPool.instance.GetAudioSource();
            audioSource.SetActive(true);
            audioSource.GetComponent<PooledAudioSource>().SetDataAndPlay(data.pickupSound, AudioGroups.pickup);

            gameObject.SetActive(false);
        }

        private void RemovePickup()
        {
            gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            existingPickups--;
        }

        protected virtual void OnDestroy()
        {
            GameplayManager.OnGameStart.RemoveListener(RemovePickup);
        }
    }
}