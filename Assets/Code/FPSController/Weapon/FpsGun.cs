using System;
using _Systems.Audio;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

[RequireComponent(typeof(AudioSource))]
public class FpsGun : MonoBehaviour
{
    public GunType Type;

    public bool isRaycastWeapon = false;

    [HideIf("isRaycastWeapon")]
    public ProjectileParticleSpawner projectilePrefab;

    [HideIf("isRaycastWeapon")]
    public GameObject projectileSpawnTransform;
    
    [ShowIf("isRaycastWeapon")]
    [Tooltip("This is the particle to be spawned on hit for this weapon.")]
    public GameObject raycastHitEffectPrefab;

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

    void HandleShooting()
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

                if (!isRaycastWeapon) 
                {
                    // Handle Projectile Based Weapon Firing
                    var projectile = Instantiate(projectilePrefab, projectileSpawnTransform.transform.position,
                        Camera.main.transform.rotation);
                    projectile.gameObject.layer = LayerMask.NameToLayer("Player");

                    float x = Screen.width / 2;
                    float y = Screen.height / 2;
                    var ray = Camera.main.ScreenPointToRay(new Vector3(x, y, 0));
                    projectile.SetDirection(ray.direction);
                }
                else
                {
                    // Handle Raycast Weapon Firing
                    Vector3 rayOrigin = Camera.main.ViewportToWorldPoint (new Vector3(0.5f, 0.5f, 0.0f));

                    // Declare a raycast hit to store information about what our raycast has hit
                    RaycastHit raycastHit;

                    int layerMask = ~(1 << LayerMask.NameToLayer("Player"));

                    if (Physics.Raycast(rayOrigin, Camera.main.transform.forward, out raycastHit, Mathf.Infinity, layerMask))
                    {
                        // Get rotation from normal
                        Quaternion rot = Quaternion.FromToRotation(Vector3.up, raycastHit.normal);
                        Vector3 pos = raycastHit.point;
                        // Create the hit particle system gameobject
                        var createdParticleSystem = Instantiate(raycastHitEffectPrefab, pos, rot);
                        createdParticleSystem.transform.LookAt(raycastHit.point + raycastHit.normal); 
                        Destroy(createdParticleSystem, 8); // TODO: change how this operates.
                    }
                }
            }
        }
        else
        {
            fireRateTimer -= Time.deltaTime;
        }
    }

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
