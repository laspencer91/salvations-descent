using System;
using _Systems.Audio;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class ShotgunRaycastWeapon : WeaponBase
{
    [BoxGroup("Shotgun Raycast Weapon Properties")]
    [Tooltip("This is the particle to be spawned on hit for this weapon.")]
    [Required]
    public GameObject RaycastHitEffectPrefab;

    [BoxGroup("Shotgun Raycast Weapon Properties")]
    public Vector2[] BulletAngles = Array.Empty<Vector2>();

    protected override void Fire()
    {
        // Calculate the angle between each raycast
        Vector3 rayOrigin = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));

        int layerMask = ~(1 << LayerMask.NameToLayer("Player"));

        for (int i = 0; i < BulletAngles.Length; i++)
        {
            // Calculate the direction of the current raycast with spread
            Quaternion spreadRotation = Quaternion.Euler(0f, BulletAngles[i].x, BulletAngles[i].y);
            Vector3 rayDirection = spreadRotation * Camera.main.transform.forward;

            // Declare a raycast hit to store information about what our raycast has hit
            RaycastHit raycastHit;

            if (Physics.Raycast(rayOrigin, rayDirection, out raycastHit, Mathf.Infinity, layerMask))
            {
                // Get rotation from normal
                Quaternion rot = Quaternion.FromToRotation(Vector3.up, raycastHit.normal);
                Vector3 pos = raycastHit.point;
                // Create the hit particle system gameobject
                var createdParticleSystem = Instantiate(RaycastHitEffectPrefab, pos, rot);
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
}
