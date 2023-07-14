using System;
using _Systems.Audio;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class ShotgunRaycastWeapon : RaycastWeapon
{
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
        Camera mainCamera = Camera.main;
        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);

        for (int i = 0; i < BulletAngles.Length; i++)
        {
            Vector3 screenOffset = new Vector3(BulletAngles[i].x, BulletAngles[i].y, 0f) * BulletSpreadModifier;
            Ray ray =  mainCamera.ScreenPointToRay(screenCenter + screenOffset);

            RaycastHit hit = PerformRaycastImpactCalcuation(ray, Mathf.Infinity);
        }
    }
}
