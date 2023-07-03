using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    public float speed = 60;

    public float lifetime = 2;
        
    public float hitOffset = 0f;
    
    public bool UseFirePointRotation;
    
    public Vector3 rotationOffset = new Vector3(0, 0, 0);
    
    public GameObject hit;
    
    public GameObject flash;
    
    public GameObject[] Detached;

    private Rigidbody rigidbody;

    private int damage = 0;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }
    
    public void SetDirection(Vector3 rayDirection)
    {
        rigidbody.velocity = rayDirection * speed;
    }
    
    private void Update()
    {
        lifetime -= Time.deltaTime;
        if (lifetime <= 0)
        {
            Destroy(gameObject);
        }
    }

    private RaycastHit raycastHitFromPreviousFrame;

    private bool readyToProcessRaycastHit = false;

    private void FixedUpdate() 
    {
        CheckForCollision();
    }

    private void CheckForCollision()
    {
        if (readyToProcessRaycastHit) 
        {
            // Get rotation from normal
            Quaternion rot = Quaternion.FromToRotation(Vector3.up, raycastHitFromPreviousFrame.normal);
            Vector3 pos = raycastHitFromPreviousFrame.point;
            // Create the hit particle system gameobject
            var createParticleSystemInstance = Instantiate(hit, pos, rot);

            if (rotationOffset != Vector3.zero) 
            { 
                createParticleSystemInstance.transform.rotation = Quaternion.Euler(rotationOffset); 
            }
            else 
            { 
                createParticleSystemInstance.transform.LookAt(raycastHitFromPreviousFrame.point + raycastHitFromPreviousFrame.normal); 
            }

            // Damage the other object
            IDamageable damageableComponent = raycastHitFromPreviousFrame.collider.gameObject.GetComponent<IDamageable>();
            if (damageableComponent != null) 
            {
                damageableComponent.TakeDamage(damage);
            }

            // Destroy self
            Destroy(gameObject);

            // Set Particle System To Die
            var hitPs = createParticleSystemInstance.GetComponent<ParticleSystem>();
            if (hitPs != null)
            {
                Destroy(createParticleSystemInstance, 5);
            }
            else
            {
                var hitPsParts = createParticleSystemInstance.transform.GetChild(0).GetComponent<ParticleSystem>();
                Destroy(createParticleSystemInstance, 5);
            }

            return;
        }

        RaycastHit raycastHit;
        if (Physics.Raycast(transform.position, rigidbody.velocity.normalized, out raycastHit, rigidbody.velocity.magnitude * Time.fixedDeltaTime)) 
        {
            if (LayerMask.LayerToName(raycastHit.collider.gameObject.layer) == "Player") 
            {
                Debug.LogWarning("Hit Player");
                return;
            }

            raycastHitFromPreviousFrame = raycastHit;
            readyToProcessRaycastHit = true;
        }
    }

    public void SetDamage(int damage) 
    {
        this.damage = damage;
    }
}
