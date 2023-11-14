using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioSource PlayerAudioSource;

    private void Awake() 
    {
        if (Instance != null)
        {
            DestroyImmediate(gameObject);
            return;
        }

        Instance = this;

        if (!PlayerAudioSource)
        {
            PlayerAudioSource = Camera.main.GetComponent<AudioSource>();
            if (PlayerAudioSource == null)
            {
                Debug.LogWarning("Camera Audio Source is not initialized in AudioManager. 2D Audio will not work properly!");
            }
        }
    }

    public static AudioSource Get2DAudioSource()
    {
        if (AudioManager.Instance == null)
        {
            Debug.LogWarning("Audio Manager is not initialized. Probably does not exist in the scene. Add an AudioManager component to a GameObject!");
            return null;
        }
        return AudioManager.Instance.PlayerAudioSource;
    }
}
