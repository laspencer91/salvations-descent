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
    private float _rotationAnglesPerSecond = 180;

    [SerializeField]
    private AudioEvent _pickupAudioEvent;
    
    private SphereCollider _pickupTriggerArea;
    
    void Update()
    {
        transform.Rotate(transform.up, _rotationAnglesPerSecond * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponentInChildren<FPSGunHandler>().AddToArsenal(type);
            _pickupAudioEvent.Play(other.gameObject.GetComponent<AudioSource>());
            Destroy(gameObject);
        }
    }
}
