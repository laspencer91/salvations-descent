using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SmoothMovement))]
public class HeadBob : MonoBehaviour 
{
    public Vector3 restPosition; //local position where your camera would rest when it's not bobbing.
    public float bobSpeed = 4.8f; //how quickly the player's head bobs.
    public float bobAmount = 0.05f; //how dramatic the bob is. Increasing this in conjunction with bobSpeed gives a nice effect for sprinting.
    public float swayAmount = 0.15f;
    public float idleReturnSpeed = 0.05f; //smooths out the transition from moving to not moving.
    public AnimationCurve landingCurve;
    public float groundLandingVerticalOffsetDistance = 0.05f;
    public float groundLandingOffsetRecoverySpeed = 5f;
    public GameObject cameraArm;
    public Transform weaponHolder;
    public float weaponHolderBobDampen = 0.5f;
    public float weaponHolderSwayDampen = 0.5f;

    private float timer = Mathf.PI;
    private float verticalOffsetAnimTimer = 0;
    private SmoothMovement smoothMovement;


    private void Awake() {
        smoothMovement = GetComponent<SmoothMovement>();
        GetComponent<CharacterControllerEvents>().onLanding.AddListener(OnGroundLanding);
    }

    void Update()
    {
        Vector3 verticalOffset = CalculateVerticalOffset();
        if ((Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0) && smoothMovement.isGrounded) //moving
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

        Vector3 newPosition = new Vector3(restPosition.x + Mathf.Cos(timer) * swayAmount, restPosition.y + (Mathf.Sin(timer * 2) * bobAmount), restPosition.z); //abs val of y for a parabolic path
        cameraArm.transform.localPosition = newPosition + verticalOffset;

        Vector3 camArmDifference = cameraArm.transform.localPosition - restPosition;

        weaponHolder.localPosition = new Vector3((restPosition.x + camArmDifference.x * weaponHolderSwayDampen), (restPosition.y + camArmDifference.y * weaponHolderBobDampen), restPosition.z);
        
        if (timer > Mathf.PI * 2)
            timer -= Mathf.PI * 2;
    }

    private Vector3 CalculateVerticalOffset() {
        verticalOffsetAnimTimer -= groundLandingOffsetRecoverySpeed * Time.deltaTime;
        return Vector3.down * landingCurve.Evaluate(verticalOffsetAnimTimer) * groundLandingVerticalOffsetDistance;
    }

    public void OnGroundLanding() {
        verticalOffsetAnimTimer = 1;
    }
}