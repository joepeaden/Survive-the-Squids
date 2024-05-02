using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace MyGame
{
    public class PooledAudioSource : MonoBehaviour
    {
        public int sameProjLimit;
        static Dictionary<AudioClip, int> projSources = new Dictionary<AudioClip, int>();

        [SerializeField] AudioSource source;
        [SerializeField] AudioMixerGroup projectilesGroup;
        [SerializeField] AudioMixerGroup deathGroup;
        [SerializeField] AudioMixerGroup pickupGroup;


        // Start is called before the first frame update
        public void SetData(AudioClip clip, AudioGroups group)
        {
            source.clip = clip;

            switch(group)
            {
                case AudioGroups.projectiles:
                    source.outputAudioMixerGroup = projectilesGroup;

                    if (!projSources.ContainsKey(clip))
                    {
                        projSources[clip] = 0;
                    }
                    projSources[clip]++;

                    // limit the amount of the same sounds playing at once
                    if (projSources[clip] > sameProjLimit)
                    {
                        gameObject.SetActive(false);
                        return;
                    }

                    break;
                case AudioGroups.death:
                    source.outputAudioMixerGroup = deathGroup;
                    break;
                case AudioGroups.pickup:
                    source.outputAudioMixerGroup = pickupGroup;
                    break;
            }

            source.Play();
        }

        private void Update()
        {
            if (!source.isPlaying)
            {
                gameObject.SetActive(false);
            }
        }

        private void OnDisable()
        {
            if (source.outputAudioMixerGroup == projectilesGroup)
            {
                projSources[source.clip]--;
            }
        }
    }

    public enum AudioGroups
    {
        projectiles,
        death,
        pickup
    }
}
