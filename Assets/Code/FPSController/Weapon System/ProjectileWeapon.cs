using System;
using _Systems.Audio;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class ProjectileWeapon : WeaponBase
{
    public Projectile projectilePrefab;
    public GameObject projectileSpawnTransform;

    protected override void Fire()
    {
        // Handle Projectile Based Weapon Firing
        var projectile = Instantiate(projectilePrefab, projectileSpawnTransform.transform.position,
            Camera.main.transform.rotation);
        projectile.gameObject.layer = LayerMask.NameToLayer("Player");
        projectile.SetDamage(Damage);

        float x = Screen.width / 2;
        float y = Screen.height / 2;
        var ray = Camera.main.ScreenPointToRay(new Vector3(x, y, 0));
        projectile.SetDirection(ray.direction);
    }
}
