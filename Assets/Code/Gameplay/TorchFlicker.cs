using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorchFlicker : MonoBehaviour
{
    public bool effectRange = false;
    public float minIntensity = 0.5f;
    public float maxIntensity = 1f;
    public float flickerSpeed = 1f;
    private float randomOffset;

    private Light torchLight;

    private void Start()
    {
        torchLight = GetComponent<Light>();
        randomOffset = Random.Range(0f, 100f);
    }

    private void Update()
    {
        // Calculate the flickering intensity using a sine wave
        float flickerIntensity = Mathf.Lerp(minIntensity, maxIntensity, Mathf.PerlinNoise(Time.time * flickerSpeed + randomOffset, 0f));

        // Apply the flickering intensity to the torch light
        if (effectRange) 
        {
            torchLight.intensity = flickerIntensity;
        }
        else
        {
            torchLight.range = flickerIntensity;
        }
    }
}