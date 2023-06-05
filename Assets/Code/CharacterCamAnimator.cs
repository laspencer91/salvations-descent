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

    private float timer = Mathf.PI / 2;
    private float verticalOffsetAnimTimer = 0;
    private SmoothMovement smoothMovement;

    private void Awake() {
        smoothMovement = GetComponent<SmoothMovement>();
        GetComponent<CharacterControllerEvents>().onLanding.AddListener(OnGroundLanding);
    }

    void Update()
    {
        Vector3 verticalOffset = CalculateVerticalOffset();
        Debug.Log(verticalOffset);

        if ((Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0) && smoothMovement.isGrounded) //moving
        {
            timer += bobSpeed * Time.deltaTime;

            //use the timer value to set the position
            Vector3 newPosition = new Vector3(Mathf.Cos(timer) * swayAmount, restPosition.y + Mathf.Abs((Mathf.Sin(timer) * bobAmount)), restPosition.z); //abs val of y for a parabolic path
            cameraArm.transform.localPosition = newPosition + verticalOffset;
        }
        else
        {
            float angleA = Mathf.PI / 2;
            float angleB = (3 * Mathf.PI) / 2;

            // Calculate the absolute difference between the angles
            float diffA = Mathf.Abs(Mathf.DeltaAngle(timer, angleA));
            float diffB = Mathf.Abs(Mathf.DeltaAngle(timer, angleB));

            // Interpolate towards the nearest angle
            float targetAngle = (diffA < diffB) ? angleA : angleB;
            timer = Mathf.MoveTowardsAngle(timer, targetAngle, idleReturnSpeed * Time.deltaTime);

            //use the timer value to set the position
            Vector3 newPosition = new Vector3(Mathf.Cos(timer) * swayAmount, restPosition.y + Mathf.Abs((Mathf.Sin(timer) * bobAmount)), restPosition.z); //abs val of y for a parabolic path
            cameraArm.transform.localPosition = newPosition + verticalOffset;
        }

        
        if (timer > Mathf.PI * 2)
            timer = 0;

    }

    private Vector3 CalculateVerticalOffset() {
        verticalOffsetAnimTimer -= groundLandingOffsetRecoverySpeed * Time.deltaTime;
        return Vector3.down * landingCurve.Evaluate(verticalOffsetAnimTimer) * groundLandingVerticalOffsetDistance;
    }

    public void OnGroundLanding() {
        verticalOffsetAnimTimer = 1;
    }
}