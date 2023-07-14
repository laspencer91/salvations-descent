using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AmmoCounter : MonoBehaviour
{
    public WeaponSystem weaponSystem;

    TextMeshProUGUI ammoCounterText;

    private void Awake() 
    {
        ammoCounterText = GetComponent<TextMeshProUGUI>();
    }

    private void Update() 
    {
        WeaponBase currentWeapon = weaponSystem.GetCurrentWeapon();
        ammoCounterText.text = (currentWeapon.CurrentAmmoOnBelt + currentWeapon.CurrentAmmoLoaded).ToString();
    }
}
