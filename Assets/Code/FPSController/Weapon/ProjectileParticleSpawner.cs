using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ProjectileParticleSpawner : MonoBehaviour
{
    public float speed = 60;

    public float lifetime = 2;
    
    private Rigidbody _rigidbody;
    
    public float hitOffset = 0f;
    
    public bool UseFirePointRotation;
    
    public Vector3 rotationOffset = new Vector3(0, 0, 0);
    
    public GameObject hit;
    
    public GameObject flash;
    
    public GameObject[] Detached;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }
    
    public void SetDirection(Vector3 rayDirection)
    {
        _rigidbody.velocity = rayDirection * speed;
    }

    private void OnCollisionEnter(Collision other)
    {
        HandleCollision(other);
        Destroy(gameObject);
    }
    
    private void Update()
    {
        lifetime -= Time.deltaTime;
        if (lifetime <= 0)
        {
            Destroy(gameObject);
        }
    }
    
    public void HandleProjectileSpawn()
    {
        if (flash != null)
        {
            var flashInstance = Instantiate(flash, transform.position, Quaternion.identity);
            flashInstance.transform.forward = gameObject.transform.forward;
            var flashPs = flashInstance.GetComponent<ParticleSystem>();
            if (flashPs != null)
            {
                Destroy(flashInstance, flashPs.main.duration);
            }
            else
            {
                var flashPsParts = flashInstance.transform.GetChild(0).GetComponent<ParticleSystem>();
                Destroy(flashInstance, flashPsParts.main.duration);
            }
        }
        Destroy(gameObject,5);
	}

    public void HandleCollision(Collision collision)
    {
        ContactPoint contact = collision.contacts[0];
        Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
        Vector3 pos = contact.point + contact.normal * hitOffset;

        if (hit != null)
        {
            var hitInstance = Instantiate(hit, pos, rot);
            if (UseFirePointRotation) { hitInstance.transform.rotation = gameObject.transform.rotation * Quaternion.Euler(0, 180f, 0); }
            else if (rotationOffset != Vector3.zero) { hitInstance.transform.rotation = Quaternion.Euler(rotationOffset); }
            else { hitInstance.transform.LookAt(contact.point + contact.normal); }

            var hitPs = hitInstance.GetComponent<ParticleSystem>();
            if (hitPs != null)
            {
                Destroy(hitInstance, hitPs.main.duration);
            }
            else
            {
                var hitPsParts = hitInstance.transform.GetChild(0).GetComponent<ParticleSystem>();
                Destroy(hitInstance, hitPsParts.main.duration);
            }
        }
        
        foreach (var detachedPrefab in Detached)
        {
            if (detachedPrefab != null)
            {
                detachedPrefab.transform.parent = null;
            }
        }
    }
}
