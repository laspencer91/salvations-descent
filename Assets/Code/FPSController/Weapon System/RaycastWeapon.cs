using System;
using _Systems.Audio;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class RaycastWeapon : WeaponBase
{
    [BoxGroup("Shotgun Raycast Weapon Properties")]
    public LayerMask raycastDetectionLayers;

    [BoxGroup("Raycast Weapon Properties")]
    [Tooltip("This is the particle to be spawned on hit for this weapon.")]
    [Required]
    public GameObject raycastHitEffectPrefab;

    protected override void Fire()
    {
        // Handle Raycast Weapon Firing
        Vector3 rayOrigin = Camera.main.ViewportToWorldPoint (new Vector3(0.5f, 0.5f, 0.0f));

        // Declare a raycast hit to store information about what our raycast has hit
        RaycastHit raycastHit;

        if (Physics.Raycast(rayOrigin, Camera.main.transform.forward, out raycastHit, Mathf.Infinity, raycastDetectionLayers))
        {
            // Get rotation from normal
            Quaternion rot = Quaternion.FromToRotation(Vector3.up, raycastHit.normal);
            Vector3 pos = raycastHit.point;
            // Create the hit particle system gameobject
            var createdParticleSystem = Instantiate(raycastHitEffectPrefab, pos, rot);
            createdParticleSystem.transform.LookAt(raycastHit.point + raycastHit.normal); 

            IDamageable damageableComponent = raycastHit.collider.gameObject.GetComponent<IDamageable>();
            if (damageableComponent != null) 
            {
                damageableComponent.TakeDamage(Damage);
            }

            Destroy(createdParticleSystem, 8); // TODO: change how this operates.
        }
    }
}
