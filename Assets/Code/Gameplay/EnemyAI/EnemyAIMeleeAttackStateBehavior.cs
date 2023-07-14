using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;


public class EnemyAIMeleeAttackStateBehavior : AIStateBehavior<EnemyState>
{
    public float AttackRange = 2f; // Adjust the attack range as needed
    public float AttackDuration = 1.25f;
    public float ChaseSpeed = 7f;
    [Tooltip("Speed at which the character will rotate toward the player before attacking again.")]
    public float RetargetRotationSpeed = 360f;
    public float CalculateNavDestinationFrequency = 0.25f;

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
    private MeleeAttackState currentState;
    private Quaternion desiredRotation; // Store the desired rotation for rotation correction

    public override void AwakeState()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
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
            animator.CrossFadeInFixedTime(ChaseAnimationName, 0.25f);
            navMeshAgent.isStopped = false;
        }

        calculateNavDestinationTime -= Time.deltaTime;
        if (calculateNavDestinationTime <= 0)
        {
            calculateNavDestinationTime = CalculateNavDestinationFrequency;
            navMeshAgent.SetDestination(playerTransform.position);
        }

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