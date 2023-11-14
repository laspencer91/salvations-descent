using System;
using System.Collections;
using System.Collections.Generic;
using _Systems.Audio;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class WeaponPickup : MonoBehaviour
{
    [SerializeField]
    private GunType type;
    
    [SerializeField]
    private float rotationAnglesPerSecond = 180;

    [SerializeField]
    private AudioEvent pickupAudioEvent;
        
    void Update()
    {
        transform.RotateAround(transform.position, Vector3.up, rotationAnglesPerSecond * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponentInChildren<WeaponSystem>().AddToArsenal(type);
            ScreenFlash.FlashScreen(FlashType.ItemPickup);
            pickupAudioEvent.Play(other.gameObject.GetComponent<AudioSource>());
            Destroy(gameObject);
        }
    }
}
