using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Systems.Audio
{
    [CreateAssetMenu(fileName = "New Audio Event", menuName = "Game/Create Audio Event")]
    public class AudioEvent : ScriptableObject
    {
        public List<AudioClip> audioClips = new List<AudioClip>();

        public float volume = 1;

        public Vector2 pitchVariation = new Vector2(1, 1);

        [Tooltip("Set a cooldown for this audio clip for certain scenarios, like a shotgun blast that could trigger the same hit effect multiple times in an instant!")]
        public float PlayTriggerCooldown = 0.0f;

        private float lastTriggerTime = 0;
        
        public void Play(AudioSource audioSource, float volumePercentage = 1)
        {
            // Reset trigger time because the variable persists between plays.
            if (Time.time < lastTriggerTime)
                lastTriggerTime = 0;

            // Check for trigger time cooldown.
            if (Time.time - lastTriggerTime < PlayTriggerCooldown) 
                return;

            var cachePitch = audioSource.pitch;
            audioSource.pitch = Random.Range(pitchVariation.x, pitchVariation.y);
            audioSource.PlayOneShot(audioClips[Random.Range(0, audioClips.Count)], volume * volumePercentage);

            lastTriggerTime = Time.time;
        }


        public void Play2DSound(float volumePercentage = 1)
        {
            Play(AudioManager.Get2DAudioSource(), volumePercentage);
        }
    }
}