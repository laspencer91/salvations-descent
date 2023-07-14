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
        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
        RaycastHit hit = PerformRaycastImpactCalcuation(Camera.main.ScreenPointToRay(screenCenter), Mathf.Infinity);
    }

    protected RaycastHit PerformRaycastImpactCalcuation(Ray ray, float distance)
    {
        // Handle Raycast Weapon Firing
        // Declare a raycast hit to store information about what our raycast has hit
        RaycastHit raycastHit;

        if (Physics.Raycast(ray, out raycastHit, distance, raycastDetectionLayers, QueryTriggerInteraction.Ignore))
        {
            // Get rotation from normal
            Quaternion rot = Quaternion.FromToRotation(Vector3.up, raycastHit.normal);
            Vector3 pos = raycastHit.point;

            // Can impact materials and footstep materials be syncronized in some way?
            // Do they need to be?
            ImpactMaterial impactMaterialComponent = raycastHit.collider.gameObject.GetComponent<ImpactMaterial>();
            if (impactMaterialComponent != null)
            {
                Debug.Log("Hit, " + impactMaterialComponent.Type);
            }

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

        return raycastHit;
    }
}
