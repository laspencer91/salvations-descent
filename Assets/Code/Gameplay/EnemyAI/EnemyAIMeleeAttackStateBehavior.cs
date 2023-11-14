using System.Collections;
using System.Collections.Generic;
using _Systems.Audio;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;


[RequireComponent(typeof(AudioSource))]
public class EnemyAIMeleeAttackStateBehavior : AIStateBehavior<EnemyState>
{
    public float AttackRange = 2f; // Adjust the attack range as needed
    public float AttackDuration = 1.25f;
    public float ChaseSpeed = 7f;
    [Tooltip("Speed at which the character will rotate toward the player before attacking again.")]
    public float RetargetRotationSpeed = 360f;
    public float CalculateNavDestinationFrequency = 0.25f;

    [BoxGroup("Audio")]
    public AudioEvent ChaseAudioEvent;
    [BoxGroup("Audio")]
    public Vector2 ChaseAudioPlayInterval = new Vector2(1f, 2f);

    [BoxGroup("Animations")]
    [RequiredListLength(1, 99)]
    public List<string> AttackAnimationNames;
    [BoxGroup("Animations")]
    public string ChaseAnimationName;
    [BoxGroup("Animations")] [Tooltip("Sync a parameter to the navMeshAgent velocity. Can be used for blend trees.")]
    public string VelocityAnimatorParameter;

    private NavMeshAgent navMeshAgent;
    private Transform playerTransform;
    private Animator animator;
    private AudioSource audioSource;
    private MeleeAttackState currentState;
    private Quaternion desiredRotation; // Store the desired rotation for rotation correction
    private float chasePlayAudioTimer = 0;

    public override void AwakeState()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (VelocityAnimatorParameter != null && VelocityAnimatorParameter != "")
        {
            animator.SetFloat(VelocityAnimatorParameter, navMeshAgent.velocity.magnitude);
        }

        switch(currentState)
        {
            case MeleeAttackState.Chasing: ExecuteChaseState(); break;
            case MeleeAttackState.Attacking: ExecuteAttackState(); break;
            case MeleeAttackState.Retargeting: ExecuteRetargetState(); break;
        }

        HandleAudio();
    }

    private bool isAttacking = false;
    public void ExecuteAttackState()
    {
        if (!isAttacking)
        {
            navMeshAgent.isStopped = true;
            navMeshAgent.velocity /= 2; // Cut the speed down to attack, auto braking just wasn't working.
            isAttacking = true;
            string animationClipName = AttackAnimationNames[Random.Range(0, AttackAnimationNames.Count)];
            animator.CrossFadeInFixedTime(animationClipName, 0.25f);
            Invoke("StopAttack", AttackDuration);  
        }
    }

    private void StopAttack()
    {
        isAttacking = false;
        currentState = MeleeAttackState.Retargeting;
        animator.CrossFadeInFixedTime(ChaseAnimationName, 0.25f);
    }

    private float calculateNavDestinationTime = 0;
    
    public void ExecuteChaseState()
    {
        if (navMeshAgent.isStopped)
        {
            // First Frame In The Chase State, this check detects the transition.
            animator.CrossFadeInFixedTime(ChaseAnimationName, 0.25f);
            navMeshAgent.isStopped = false;
        }
        // Recalculate Destination
        calculateNavDestinationTime -= Time.deltaTime;
        if (calculateNavDestinationTime <= 0)
        {
            calculateNavDestinationTime = CalculateNavDestinationFrequency;
            navMeshAgent.SetDestination(playerTransform.position);
        }

        // Attack Transition Detection
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        if (distanceToPlayer <= AttackRange)
        {
            currentState = MeleeAttackState.Attacking;
        }
    }

    public void ExecuteRetargetState()
    {
        Vector3 directionToPlayer = playerTransform.position - transform.position;
        directionToPlayer.y = 0f; // Ignore vertical difference
        Quaternion desiredRotation = Quaternion.LookRotation(directionToPlayer);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, desiredRotation, RetargetRotationSpeed * Time.deltaTime);

        // Resume chasing if the player is far enough away
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        if (distanceToPlayer > AttackRange)
        {
            currentState = MeleeAttackState.Chasing;
        }
        else if (IsFacingRotation(desiredRotation))
        {
            currentState = MeleeAttackState.Attacking;
        }
    }

    private bool IsFacingRotation(Quaternion targetRotation)
    {
        float angleDifference = Quaternion.Angle(transform.rotation, targetRotation);
        return angleDifference <= 5f;
    }

    private void HandleAudio() 
    {
        chasePlayAudioTimer -= Time.deltaTime;
        if (chasePlayAudioTimer <= 0)
        {
            ChaseAudioEvent.Play(audioSource);
            chasePlayAudioTimer = Random.Range(ChaseAudioPlayInterval.x, ChaseAudioPlayInterval.y);
        }
    }

    public override void EnterState()
    {
        Debug.Log("ENTERING ATTACK STATE");
        navMeshAgent.speed = ChaseSpeed;
    }

    public override void ExitState()
    {
        StopAttack();
        navMeshAgent.isStopped = true;
    }
}

enum MeleeAttackState
{
    Chasing,
    Attacking,
    Retargeting
}