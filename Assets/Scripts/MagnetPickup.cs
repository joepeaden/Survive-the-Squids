using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace MyGame
{
    public class MagnetPickup : Pickup
    {
        /// <summary>
        /// Ya know, the little thang that sucks up all the XP and the player is like "ADSKFJAE;IFHAER;ITJHREW;A I AM SO POWERFUL"?
        /// </summary>
        public static GameObject Instance => _instance;
        private static GameObject _instance;

        private void Awake()
        {
            if (_instance == null) 
            {
                _instance = gameObject;
            }
            else
            {
                Debug.Log("Too many magnet pickups, deleting one! (singleton)");
                Destroy(gameObject);
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            StartCoroutine(Timeout());
        }

        private IEnumerator Timeout()
        {
            float timeToDespawn = 30;

            while (timeToDespawn > 0)
            {
                timeToDespawn -= Time.deltaTime;
                yield return null;
            }

            gameObject.SetActive(false);
        }

        protected override void OnTriggerEnter2D(Collider2D other)
        {
            SamplePickup.ResetStartDelay();
            foreach (SamplePickup sampleObject in SamplePickup.AllActiveSamples)
            {
                if (sampleObject.gameObject.activeInHierarchy)
                {
                    sampleObject.IsMagnetized = true;
                }
            }

            base.OnTriggerEnter2D(other);
        }
    }
}