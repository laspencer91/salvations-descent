using System;
using _Systems.Audio;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public abstract class WeaponBase : MonoBehaviour
{
    public GunType Type;

    [FoldoutGroup("Shooting")]
    public int Damage = 1;
    [FoldoutGroup("Shooting")]
    public float fireRate = 1;
    [FoldoutGroup("Shooting")]
    public bool clickToShoot = true;
    [FoldoutGroup("Shooting")]
    public AudioEvent shootAudioEvent;
    [FoldoutGroup("Shooting")]
    public AudioEvent dryFireAudioEvent;
    [FoldoutGroup("Shooting")]
    public AudioEvent reloadAudioEvent;
    [FoldoutGroup("Shooting")]
    public UnityEvent shootEvent = new UnityEvent();

    [FoldoutGroup("Ammunition")]
    public int StartingAmmo = 10;

    [FoldoutGroup("Ammunition")]
    public int MaximumAmmo = 30;

    [FoldoutGroup("Ammunition")]
    public int ShotsPerReload = 5;

    [FoldoutGroup("Recoil Animation")]
    public float recoilDistance = 0.01f;

    [FoldoutGroup("Recoil Animation")]
    public float recoilRecoverSpeed = 5f;

    [HideInInspector]
    public int CurrentAmmoOnBelt = 0;

    [HideInInspector]
    public int CurrentAmmoLoaded = 0;

    [HideInInspector]
    public bool IsReloading = false;
    
    private float fireRateTimer = 0;

    [HideInInspector]
    public bool IsShootingEnabled = true;
    
    private AudioSource audioSource;

    private Animator animator;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        CurrentAmmoOnBelt = StartingAmmo - ShotsPerReload;
        CurrentAmmoLoaded = ShotsPerReload;
    }

    private void OnEnable()
    {
        animator.Play("GunIntoFrame");
        IsShootingEnabled = false;
    }

    void Update()
    {
        if (IsShootingEnabled)
        {
            if (!IsReloading) 
            {
                HandleShooting();
            }
            else
            {
                HandleReloading();
            }

            HandleVisualRecoil();
        }
    }

    private void HandleShooting() 
    {
        if (fireRateTimer <= 0)
        {
            var validShootButton = clickToShoot ? Input.GetButtonDown("Fire1") : Input.GetButton("Fire1");
            if (validShootButton)
            {
                if (CurrentAmmoLoaded > 0) 
                {
                    shootEvent.Invoke();
                    shootAudioEvent.Play(audioSource);
                    transform.localPosition = Vector3.back * recoilDistance;
                    fireRateTimer = fireRate;
                    CurrentAmmoLoaded -= 1;
                    Fire();

                    if (CurrentAmmoLoaded <= 0 && CurrentAmmoOnBelt > 0) 
                    {
                        IsReloading = true;
                        StartCoroutine(HandleReloading());
                    }
                }
                else
                {
                    if (CurrentAmmoOnBelt <= 0) 
                    {
                        // Play Click Sound, No Ammo.
                        dryFireAudioEvent.Play(audioSource);
                        fireRateTimer = fireRate;
                    }
                }
            }
        }
        else
        {
            fireRateTimer -= Time.deltaTime;
        }
    }

    protected IEnumerator HandleReloading()
    {
        // Animation
        animator.enabled = true;
        animator.SetBool("IsReloading", true);
        reloadAudioEvent.Play(audioSource);
        // Audio

        yield return new WaitForSeconds(2f);
        animator.SetBool("IsReloading", false);

        IsShootingEnabled = false;

        IsReloading = false;
        int loadedAmmoCount = Math.Min(ShotsPerReload, CurrentAmmoOnBelt);
        CurrentAmmoLoaded = loadedAmmoCount;
        CurrentAmmoOnBelt -= loadedAmmoCount;
    }

    protected abstract void Fire();

    void HandleVisualRecoil()
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, recoilRecoverSpeed * Time.deltaTime);
    }

    public void TransitionOut()
    {
        animator.enabled = true;
        animator.SetTrigger("PutAway");
    }

    // Animator Event
    public void Disable()
    {
        gameObject.SetActive(false);
    }

    // Animator Event
    public void EnableShooting()
    {
        animator.enabled = false;
        IsShootingEnabled = true;
    }
}
