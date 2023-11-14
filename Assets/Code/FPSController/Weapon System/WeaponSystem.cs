using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public enum GunType
{
    Gipapang,
    RedemptionRevolver,
    Rifle,
    DamnatorShotgun,
    Wrench,
}

public class WeaponSystem : MonoBehaviour
{
    private List<WeaponBase> allGuns = new List<WeaponBase>();

    public List<GunType> OwnedGuns = new List<GunType>();

    private WeaponBase currentlyActiveGun; 
    
    // Start is called before the first frame update
    void Awake()
    {
        allGuns = GetComponentsInChildren<WeaponBase>().ToList();
        allGuns.ForEach((gun) => gun.gameObject.SetActive(false));
        
        if (currentlyActiveGun == null && OwnedGuns.Count > 0)
        {
            EquipGun(OwnedGuns.First());
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            EquipGun(GunType.Gipapang);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            EquipGun(GunType.DamnatorShotgun);
        }
    }

    void EquipGun(GunType type)
    {
        if (currentlyActiveGun != null && (currentlyActiveGun.Type == type || !currentlyActiveGun.IsShootingEnabled)) return;
        
        if (OwnedGuns.Contains(type))
        {
            if (currentlyActiveGun != null)
                currentlyActiveGun.TransitionOut();
            
            currentlyActiveGun = allGuns.Find((gun) => gun.Type == type);
            
            currentlyActiveGun.gameObject.SetActive(true);
        }
    }

    public void AddToArsenal(GunType type)
    {
        if (!OwnedGuns.Contains(type))
        {
            OwnedGuns.Add(type);
            if (currentlyActiveGun == null)
            {
                EquipGun(type);
            }
        }
    }

    public WeaponBase GetCurrentWeapon()
    {
        return currentlyActiveGun;
    }
}