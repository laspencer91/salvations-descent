using UnityEngine;

public class SmoothMovement : MonoBehaviour
{
    public float movementSpeed = 5f;
    public float acceleration = 10f;
    public float groundFriction = 5f;
    public float airFriction = 0.2f;
    public float inAirInputModifier = 0.5f;
    public float changeOfDirectionModifier = 0.5f;  // New variable for change of direction modifier
    public float mouseSensitivity = 2f;
    public float jumpForce = 5f;
    public float slopeThreshold = 45f;
    public float slopeSpeed = 0.8f;

    public Vector3 gravity = new Vector3(0f, -9.81f, 0f);

    [HideInInspector] public Camera playerCamera;
    [HideInInspector] public bool isGrounded;
    [HideInInspector] public Vector3 velocity;

    private CharacterController controller;
    private float verticalVelocity = 0f;
    private float yaw = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        playerCamera = Camera.main;
    }

    void Update()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivity;
        float mouseY = -Input.GetAxisRaw("Mouse Y") * mouseSensitivity;

        yaw += mouseX;
        transform.eulerAngles = new Vector3(0f, yaw, 0f);
        playerCamera.transform.eulerAngles += new Vector3(mouseY, 0f, 0f);

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        Vector3 moveDirection = (transform.right * moveX + transform.forward * moveZ).normalized;

        // Apply acceleration and deceleration
        if (isGrounded)
        {
            if (moveDirection.magnitude > 0f)
            {
                float currentSpeed = velocity.magnitude;

                if (currentSpeed < movementSpeed)
                {
                    velocity += moveDirection * acceleration * Time.deltaTime;
                }
            }
            else
            {
                // Apply instant deceleration when no input is provided
                velocity -= velocity.normalized * groundFriction * Time.deltaTime;
            }

            if (Input.GetButtonDown("Jump"))
            {
                verticalVelocity = jumpForce;
            }
        }
        else
        {
            // Apply air friction
            velocity -= velocity * airFriction * Time.deltaTime;
            velocity += moveDirection * (acceleration * inAirInputModifier) * Time.deltaTime;

            verticalVelocity += gravity.y * Time.deltaTime;
        }

        velocity.y = verticalVelocity;

        // Apply ground friction when grounded
        if (isGrounded)
        {
            velocity -= velocity * groundFriction * Time.deltaTime;
        }

        // Apply change of direction modifier
        if (isGrounded && moveDirection.magnitude > 0f && Vector3.Dot(velocity, moveDirection) < 0f)
        {
            float directionModifier = Mathf.Clamp01(-Vector3.Dot(velocity.normalized, moveDirection));
            velocity *= Mathf.Lerp(1f, changeOfDirectionModifier, directionModifier);
        }

        // Clamp the velocity to the maximum speed
        velocity = Vector3.ClampMagnitude(velocity, movementSpeed);

        // Check if the character is grounded
        isGrounded = controller.isGrounded;

        // Move the character and detect collisions
        CollisionFlags collisionFlags = controller.Move((velocity) * Time.deltaTime);
        
        // Slide against walls
        if ((collisionFlags & CollisionFlags.CollidedSides) != 0)
        {
            if (!isGrounded)
            {
                Vector3 wallNormal = GetWallCollisionNormal();

                // Calculate the sliding direction
                Vector3 slideDirection = Vector3.Reflect(velocity.normalized, wallNormal).normalized;

                // Adjust the velocity to slide along the wall
                velocity = slideDirection * velocity.magnitude;
            }
        }
    }

    private Vector3 GetWallCollisionNormal()
    {
        Vector3 wallNormal = Vector3.zero;
        RaycastHit hit;

        if (Physics.Raycast(transform.position, velocity.normalized, out hit, 0.5f))
        {
            wallNormal = hit.normal;
        }

        return wallNormal;
    }

    private Vector3 hitNormal;
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (!isGrounded && velocity.y > 0f ) {
            if (Vector3.Dot(hit.normal, Vector3.down) > 0.8f)
            {
                verticalVelocity = 0f;
            }
        }

        // Check if we are sliding.
         var hitNormal = hit.normal;
         var angle = Vector3.Angle(Vector3.up, hitNormal);
         var isSliding = (angle > hit.controller.slopeLimit && angle <= 90f);
         if (isSliding)
         {
             // Slide along the slopes surface. This ensures the character stays grounded.
             var slopeRotation = Quaternion.FromToRotation(Vector3.up, hitNormal);
             var slopeVelocity = slopeRotation * new Vector3(hitNormal.x, 0f, hitNormal.z) * slopeSpeed;
             Vector3 slideDirection = Vector3.ProjectOnPlane(velocity, hitNormal).normalized;
             velocity = slideDirection * -gravity.y;

             // Prevent jumping if we are sliding, but not if we are walking against a slide.
             var collisionFlags = hit.controller.collisionFlags;
             if (!(collisionFlags.HasFlag(CollisionFlags.Below) && collisionFlags.HasFlag(CollisionFlags.Sides)))
             {
                 //yourScript.PreventJump();
             }
         }
    }
}
