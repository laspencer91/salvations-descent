using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

public class MeleeWeapon : MonoBehaviour
{
    public GunType Type;

    public int Damage = 1;

    public float TimeToMaxAttackCharge = 1;

    public float AttackDuration = 1;

    [BoxGroup("Animation")]
    public string ChargeAnimationName = "Charge Up";

    [BoxGroup("Animation")]
    public string AttackAnimationName = "Attack";

    private AudioSource audioSource;

    private Animator animator;

    private float chargeTime = 0;

    private bool swinging = false;

    private float swingAttackTimeElapsed = 0;

    private void Awake() 
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
    }
    
    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            chargeTime += Time.deltaTime;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            animator.CrossFadeInFixedTime(AttackAnimationName, .1f);
            float attackSpeed = animator.GetCurrentAnimatorStateInfo(0).length / AttackDuration;

            // Set the AttackSpeed parameter in the Animator to control the animation speed
            animator.SetFloat("Attack Speed", attackSpeed);
            animator.SetFloat("Attack Power", Mathf.Clamp01(chargeTime / TimeToMaxAttackCharge));
            Debug.Log(Mathf.Clamp01(chargeTime / TimeToMaxAttackCharge));

            chargeTime = 0;
            swinging = true;
        }

        if (!swinging)
        {
            SetAnimationTime();
        }
        else
        {
            swingAttackTimeElapsed += Time.deltaTime;
            if (swingAttackTimeElapsed >= AttackDuration)
            {
                swinging = false;
                swingAttackTimeElapsed = 0;
            }
        }
    }

    // What is the intended behavior?
    // Hold shoot button to charge up the melee swing.
    // During charge - The charge animation will charge over time.
    // Letting go will play the swinging animation.
    // Clicking quickly will barely perform the charge animation, followed by the swing.
    // A raycast will be fired on attack, with a max distance (which is melee range)
    // On hit will be performed much the same way as the gun shots.
    // Damage will be calculated as ChargeTime / MaxMeleeAttackChargeTime * Damage.
    // A recovery time will need to pass before beginning the charge again.

    private void SetAnimationTime()
    {
        float normalizedTime = Mathf.Clamp01(chargeTime / TimeToMaxAttackCharge); // Normalize the holdTime to [0, 1] range
        float timeInAnimation = normalizedTime * animator.GetCurrentAnimatorStateInfo(0).length; // Calculate the time within the animation
        animator.Play(ChargeAnimationName, 0, timeInAnimation);
    }
}
