using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class FPSGunHandler : MonoBehaviour
{
    private List<FpsGun> allGuns = new List<FpsGun>();

    public List<GunType> OwnedGuns = new List<GunType>();

    private FpsGun currentlyActiveGun; 
    
    // Start is called before the first frame update
    void Awake()
    {
        allGuns = GetComponentsInChildren<FpsGun>().ToList();
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
            EquipGun(GunType.Rifle);
        }
    }

    void EquipGun(GunType type)
    {
        if (currentlyActiveGun != null && currentlyActiveGun.Type == type) return;
        
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
}

[Flags]
public enum GunType
{
    Gipapang,
    Pistol,
    Rifle
}