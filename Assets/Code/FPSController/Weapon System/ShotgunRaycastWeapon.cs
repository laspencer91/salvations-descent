using System;
using _Systems.Audio;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class ShotgunRaycastWeapon : WeaponBase
{
    [BoxGroup("Shotgun Raycast Weapon Properties")]
    public LayerMask raycastDetectionLayers;
    
    [BoxGroup("Shotgun Raycast Weapon Properties")]
    [Tooltip("This is the particle to be spawned on hit for this weapon.")]
    [Required]
    public GameObject RaycastHitEffectPrefab;

    [BoxGroup("Shotgun Raycast Weapon Properties")]
    public float BulletSpreadModifier = 10;

    [BoxGroup("Shotgun Raycast Weapon Properties")]
    public Vector2[] BulletAngles = Array.Empty<Vector2>();

    protected override void Fire()
    {
        // Calculate the angle between each raycast
        Vector3 rayOrigin = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));
        Camera mainCamera = Camera.main;


        for (int i = 0; i < BulletAngles.Length; i++)
        {
            // Declare a raycast hit to store information about what our raycast has hit
            RaycastHit raycastHit;

            Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
            Vector3 screenOffset = new Vector3(BulletAngles[i].x, BulletAngles[i].y, 0f) * BulletSpreadModifier;
            Ray ray =  mainCamera.ScreenPointToRay(screenCenter + screenOffset);

            if (Physics.Raycast(ray, out raycastHit, Mathf.Infinity, raycastDetectionLayers))
            {
                Debug.Log(raycastHit.collider.gameObject.name);
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
