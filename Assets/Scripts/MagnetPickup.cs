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

        // don't forget to set data objects.

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

        protected override void OnTriggerEnter2D(Collider2D other)
        {
            SamplePickup.ResetStartDelay();
            foreach (SamplePickup sampleObject in SamplePickup.AllSamples)
            {
                sampleObject.IsMagnetized = true;
            }

            base.OnTriggerEnter2D(other);
        }
    }
}