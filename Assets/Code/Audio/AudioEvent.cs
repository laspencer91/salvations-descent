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
        
        public void Play(AudioSource audioSource, float volumePercentage = 1)
        {
            var cachePitch = audioSource.pitch;
            audioSource.pitch = Random.Range(pitchVariation.x, pitchVariation.y);
            audioSource.PlayOneShot(audioClips[Random.Range(0, audioClips.Count)], volume * volumePercentage);
        }
    }
}