using System;
using _Systems.Audio;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

[RequireComponent(typeof(AudioSource))]
public abstract class WeaponBase : MonoBehaviour
{
    public GunType Type;

    public int Damage = 1;

    public float fireRate = 1;

    public bool clickToShoot = true;

    public float recoilDistance = 0.01f;

    public float recoilRecoverSpeed = 5f;
    
    public AudioEvent shootAudioEvent;

    public UnityEvent shootEvent = new UnityEvent();
    
    private float fireRateTimer = 0;

    private bool shootingEnabled = true;
    
    private AudioSource audioSource;

    private Animator animator;
    
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        animator.Play("GunIntoFrame");
        shootingEnabled = false;
    }

    void Update()
    {
        if (shootingEnabled)
        {
            HandleShooting();
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
                shootEvent.Invoke();
                shootAudioEvent.Play(audioSource);
                transform.localPosition = Vector3.back * recoilDistance;
                fireRateTimer = fireRate;
                Fire();
            }
        }
        else
        {
            fireRateTimer -= Time.deltaTime;
        }
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
        shootingEnabled = true;
    }
}
