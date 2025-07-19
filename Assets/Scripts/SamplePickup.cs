using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame
{
    public class SamplePickup : Pickup
    {
        public static List<SamplePickup> AllActiveSamples = new List<SamplePickup>();

        private static float startDelay = 0;

        /// <summary>
        /// If true, will move towards the player.
        /// </summary>
        public bool IsMagnetized
        {
            get
            {
                return _isMagnetized;
            }
            set
            {
                _isMagnetized = value;
                if (_isMagnetized)
                {
                    if (!_magnetCoroutineActive)
                    {
                        _magnetCoroutineActive = true;
                        StartCoroutine(MagnetCoroutine());
                    }
                }
                else if (_magnetCoroutineActive)
                {
                    StopCoroutine(MagnetCoroutine());
                    _magnetCoroutineActive = false;
                }
            }
        }

        private bool _isMagnetized;
        private bool _magnetCoroutineActive;

        /// <summary>
        /// Reset the delay that makes samples come in over time rather than all at once. Used when starting a new magnet pull
        /// </summary>
        public static void ResetStartDelay()
        {
            startDelay = 0;
        }

        private IEnumerator MagnetCoroutine()
        {
            startDelay += .1f;
            yield return new WaitForSeconds(startDelay);

            while (_isMagnetized)
            {
                gameObject.transform.Translate((player.transform.position - transform.position) * data.magnetSpeed);
                yield return null;
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            AllActiveSamples.Add(this);
            IsMagnetized = false;
        }
        
        protected override void OnDisable()
        {
            AllActiveSamples.Remove(this);
        }

        protected override void OnTriggerEnter2D(Collider2D other)
        {
            AllActiveSamples.Remove(this);
            player.UpdateSamples(data.XPValue);
            StopCoroutine(MagnetCoroutine());
            _magnetCoroutineActive = false;
            base.OnTriggerEnter2D(other);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            AllActiveSamples.Remove(this);
        }
    }
}