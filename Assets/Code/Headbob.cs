using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(SmoothMovement))]
public class CharacterCamAnimator : MonoBehaviour 
{
    public GameObject cameraArm;

    [FoldoutGroup("Bob And Sway")] public Vector3 restPosition; //local position where your camera would rest when it's not bobbing.
    [FoldoutGroup("Bob And Sway")] public float bobSpeed = 4.8f; //how quickly the player's head bobs.
    [FoldoutGroup("Bob And Sway")] public float bobAmount = 0.05f; //how dramatic the bob is. Increasing this in conjunction with bobSpeed gives a nice effect for sprinting.
    [FoldoutGroup("Bob And Sway")] public float swayAmount = 0.15f;
    [FoldoutGroup("Bob And Sway")] public float idleReturnSpeed = 0.05f; //smooths out the transition from moving to not moving.
    [FoldoutGroup("Ground Landing Animation")] public AnimationCurve landingCurve;
    [FoldoutGroup("Ground Landing Animation")] public float groundLandingVerticalOffsetDistance = 0.05f;
    [FoldoutGroup("Ground Landing Animation")] public float groundLandingOffsetRecoverySpeed = 5f;
    [FoldoutGroup("Tilt")] public float tiltAmount = 10.0f; // The maximum tilt angle in degrees
    [FoldoutGroup("Tilt")] public float tiltSpeed = 5.0f; // The speed at which the camera tilts
    [FoldoutGroup("Tilt")] private float currentTilt = 0.0f; // The current tilt angle

    private float timer = Mathf.PI / 2;
    private float verticalOffsetAnimTimer = 0;
    private SmoothMovement smoothMovement;
    private CharacterController characterController;

    private void Awake() {
        smoothMovement = GetComponent<SmoothMovement>();
        characterController = GetComponent<CharacterController>();
        GetComponent<CharacterControllerEvents>().onLanding.AddListener(OnGroundLanding);
    }

    void Update()
    {
        ApplyMovementTilt();
        
        Vector3 verticalOffset = CalculateVerticalOffset();

        if ((Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0) && smoothMovement.isGrounded) //moving
        {
            timer += bobSpeed * Time.deltaTime;

            //use the timer value to set the position
            Vector3 newPosition = new Vector3(Mathf.Cos(timer) * swayAmount, restPosition.y + Mathf.Abs((Mathf.Sin(timer) * bobAmount)), restPosition.z); //abs val of y for a parabolic path
            cameraArm.transform.localPosition = newPosition + verticalOffset;
        }
        else
        {
            // float angleA = Mathf.PI / 2;
            // float angleB = (3 * Mathf.PI) / 2;

            // // Calculate the absolute difference between the angles
            // float diffA = Mathf.Abs(Mathf.DeltaAngle(timer, angleA));
            // float diffB = Mathf.Abs(Mathf.DeltaAngle(timer, angleB));

            // // Interpolate towards the nearest angle
            // float targetAngle = (diffA < diffB) ? angleA : angleB;
            // timer = Mathf.MoveTowardsAngle(timer, targetAngle, idleReturnSpeed * Time.deltaTime);

            // //use the timer value to set the position
            Vector3 newPosition = new Vector3(Mathf.Cos(timer) * swayAmount, restPosition.y + Mathf.Abs((Mathf.Sin(timer) * bobAmount)), restPosition.z); //abs val of y for a parabolic path
            cameraArm.transform.localPosition = newPosition + verticalOffset;
        }

        
        if (timer >= Mathf.PI * 2)
            timer = 0;

    }

    private Vector3 CalculateVerticalOffset() {
        verticalOffsetAnimTimer -= groundLandingOffsetRecoverySpeed * Time.deltaTime;
        return Vector3.down * landingCurve.Evaluate(verticalOffsetAnimTimer) * groundLandingVerticalOffsetDistance;
    }

    void ApplyMovementTilt()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float characterControllerHorizontalSpeed = cameraArm.transform.InverseTransformDirection(characterController.velocity).x;
        float characterMaxSpeed = smoothMovement.movementSpeed;

        float currentHorizontalSpeedPercent = Mathf.Abs(characterControllerHorizontalSpeed) / characterMaxSpeed;

        if (horizontalInput != 0.0f)
        {
            // Calculate the target tilt based on the horizontal input
            float targetTilt = Mathf.Sign(horizontalInput) * -tiltAmount * currentHorizontalSpeedPercent;
            
            // Smoothly interpolate the current tilt towards the target tilt
            currentTilt = Mathf.Lerp(currentTilt, targetTilt, tiltSpeed * Time.deltaTime);
        }
        else
        {
            // Smoothly return the tilt to 0 when there is no horizontal input
            currentTilt = Mathf.Lerp(currentTilt, 0.0f, tiltSpeed * Time.deltaTime);
        }

        // Apply the tilt to the camera's local rotation
        cameraArm.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, currentTilt);
    }

    public void OnGroundLanding() {
        verticalOffsetAnimTimer = 1;
    }
}