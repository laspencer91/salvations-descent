using System.Collections;
using UnityEngine;
using Sirenix.OdinInspector;

public class HeadBob : MonoBehaviour 
{
    public GameObject cameraArm;
    public Transform cameraTiltArm;
    public Transform weaponHolder;

    [FoldoutGroup("HeadBob")] public float bobSpeed = 4.8f;
    [FoldoutGroup("HeadBob")] public float bobAmount = 0.05f;
    [FoldoutGroup("HeadBob")] public float swayAmount = 0.15f;
    [FoldoutGroup("HeadBob")] public float idleReturnSpeed = 0.05f; //smooths out the transition from moving to not moving.

    [FoldoutGroup("Landing")] public AnimationCurve landingCurve;
    [FoldoutGroup("Landing")] public float groundLandingVerticalOffsetDistance = 0.05f;
    [FoldoutGroup("Landing")] public float groundLandingOffsetRecoverySpeed = 5f;

    [FoldoutGroup("Weapon Animation")] public float weaponHolderBobDampen = 0.5f;
    [FoldoutGroup("Weapon Animation")] public float weaponHolderSwayDampen = 0.5f;

    [FoldoutGroup("Camera Tilt")] public float cameraTiltAmount = 0.5f;

    private float timer = Mathf.PI;
    private float verticalOffsetAnimTimer = 0;
    private FPSMovementStateController smoothMovement;
    private Vector3 gunRestPosition;
    private Vector3 camArmRestPosition; //local position where your camera would rest when it's not bobbing.



    private void Awake() {
        smoothMovement = GetComponent<FPSMovementStateController>();
        gunRestPosition = weaponHolder.transform.position - transform.position;
        camArmRestPosition = cameraArm.transform.position - transform.position;
        //GetComponent<CharacterControllerEvents>().onLanding.AddListener(OnGroundLanding);
    }

    void Update()
    {
        Vector3 verticalOffset = Vector3.zero; //CalculateVerticalOffset();
        if ((Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0) && smoothMovement.Motor.GroundingStatus.IsStableOnGround) //moving
        {
            timer += bobSpeed * Time.deltaTime;
        }
        else
        {
            float angleA = Mathf.PI * 1.5f;
            float angleB = Mathf.PI / 2;
            float targetAngle;
            if (Mathf.Abs(angleA - timer) < Mathf.Abs(angleB - timer)) {
                targetAngle = angleA;
            } else {
                targetAngle = angleB;
            }

            timer = Mathf.Lerp(timer, targetAngle, idleReturnSpeed * Time.deltaTime);
        }

        Vector3 newPosition = new Vector3(camArmRestPosition.x + Mathf.Cos(timer) * swayAmount, camArmRestPosition.y + (Mathf.Sin(timer * 2) * bobAmount), camArmRestPosition.z); //abs val of y for a parabolic path
        cameraArm.transform.localPosition = newPosition + verticalOffset;

        Vector3 camArmDifference = cameraArm.transform.localPosition - camArmRestPosition;

        weaponHolder.localPosition = new Vector3((gunRestPosition.x + camArmDifference.x * weaponHolderSwayDampen), (gunRestPosition.y - 0.3f + camArmDifference.y * weaponHolderBobDampen), gunRestPosition.z);

        weaponHolder.rotation = Quaternion.LookRotation(Camera.main.transform.forward, transform.up);

        UpdateCameraTilt();

        if (timer > Mathf.PI * 2)
            timer -= Mathf.PI * 2;
    }

    private Vector3 CalculateVerticalOffset() {
        verticalOffsetAnimTimer -= groundLandingOffsetRecoverySpeed * Time.deltaTime;
        return Vector3.down * landingCurve.Evaluate(verticalOffsetAnimTimer) * groundLandingVerticalOffsetDistance;
    }

    private void UpdateCameraTilt() {
        // Get the horizontal velocity of the character
        Vector3 characterVelocity = transform.InverseTransformDirection(smoothMovement.Motor.Velocity);
        float horizontalVelocity = characterVelocity.x;

        // Apply the tilt to the camera rotation
        Quaternion targetRotation = Quaternion.Euler(cameraTiltArm.localEulerAngles.x, cameraTiltArm.localEulerAngles.y, cameraTiltAmount * -horizontalVelocity);
        cameraTiltArm.localRotation = Quaternion.Lerp(cameraTiltArm.localRotation, targetRotation, Time.deltaTime * 10f);
    }

    public void OnGroundLanding() {
        verticalOffsetAnimTimer = 1;
    }
}