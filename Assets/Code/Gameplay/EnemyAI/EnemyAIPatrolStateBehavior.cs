using UnityEngine;
using UnityEngine.AI;
using Sirenix.OdinInspector;
using System.Collections.Generic;

public class EnemyAIPatrolStateBehavior : AIStateBehavior<EnemyState>, ITriggerListener
{
    [BoxGroup("Patrol Settings")]
    public float IdleTimeBetweenPatrolPoints = 2f;
    [BoxGroup("Patrol Settings")]
    public float PatrolWalkSpeed = 2f;

    [BoxGroup("Alert Settings")]
    [Tooltip("Should the state transition based on an AreaTrigger being crossed by the player?")]
    public bool AlertOnAreaTrigger = true;
    [BoxGroup("Alert Settings")] [ShowIf("AlertOnAreaTrigger")]
    public AreaTrigger AlertOnAreaTriggerObject;
    [BoxGroup("Alert Settings")]
    public bool AlertOnDistanceToPlayer;
    [BoxGroup("Alert Settings")] [ShowIf("AlertOnDistanceToPlayer")]
    [Tooltip("Frequency of Distance and Line Of Sight Checks")]
    public float CheckFrequency = 0.75f;
    [BoxGroup("Alert Settings")] [ShowIf("AlertOnDistanceToPlayer")]
    public float AlertDistance = 25;
    [BoxGroup("Alert Settings")] [ShowIf("AlertOnDistanceToPlayer")]
    public bool CheckLineOfSight = false;
    [BoxGroup("Alert Settings")] [ShowIf("CheckLineOfSight")]
    public LayerMask LineOfSightObstacleMask;


    [BoxGroup("Patrol Points")]
    public bool GenerateRandomPoints = false;
    [BoxGroup("Patrol Points")] [ShowIf("GenerateRandomPoints")]
    public int PatrolPointCount = 3;
    [BoxGroup("Patrol Points")] [ShowIf("GenerateRandomPoints")]
    public float PatrolRadius = 7;
    [BoxGroup("Patrol Points")] [HideIf("GenerateRandomPoints")]
    public Transform[] PatrolPointTransforms;

    [BoxGroup("Animations")]
    public bool UseBlendTreeForAnimations = false;
    [BoxGroup("Animations")] [HideIf("UseBlendTreeForAnimations", false)]
    public string IdleAnimationName;
    [BoxGroup("Animations")] [HideIf("UseBlendTreeForAnimations", false)]
    public string WalkingAnimationName;
    [BoxGroup("Animations")] [ShowIf("UseBlendTreeForAnimations", true)]
    public string BlendTreeName;
    [BoxGroup("Animations")] [ShowIf("UseBlendTreeForAnimations", true)]
    [Tooltip("The Animator Parameter to sync with the NavMeshAgent's velocity.")]
    [Required]
    public string VelocityAnimatorParameter;

    private Animator animator;
    private FPSPlayer player;
    private NavMeshAgent navMeshAgent;
    private List<Vector3> patrolPointPositions;

    public override void AwakeState()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        player = FindObjectOfType<FPSPlayer>();

        InitPatrolPoints();
    }

    private void InitPatrolPoints()
    {
        patrolPointPositions = new List<Vector3>();
        if (GenerateRandomPoints)
        {
            // Find random points to patrol
            int generatedPoints = 0;

            while (generatedPoints < PatrolPointCount)
            {
                Vector2 randomPoint = Random.insideUnitCircle * PatrolRadius;
                Vector3 randomPoint3D = new Vector3(randomPoint.x, 0f, randomPoint.y);
                Vector3 patrolPoint = transform.position + randomPoint3D;

                NavMeshHit navMeshHit;
                if (NavMesh.SamplePosition(patrolPoint, out navMeshHit, PatrolRadius, NavMesh.AllAreas))
                {
                    Debug.DrawRay(navMeshHit.position, Vector3.up, Color.green, 2f);
                    patrolPointPositions.Add(navMeshHit.position);
                    generatedPoints++;
                }
            }
        }
        else
        {
            // Get positions of each of the patrol transforms and store them
            foreach (Transform t in PatrolPointTransforms)
            {
                patrolPointPositions.Add(t.position);
            }
        }
    }

    private bool isWaiting;

    public override void EnterState()
    {
        isWaiting = false;
        navMeshAgent.speed = PatrolWalkSpeed;
        MoveToNextPatrolPoint();

        if (UseBlendTreeForAnimations)
        {
            animator.CrossFadeInFixedTime(BlendTreeName, 0.5f);
        }
    }

    public override void ExitState()
    {
        isWaiting = false;
        navMeshAgent.isStopped = true;
    }

    private void Update()
    {
        if (UseBlendTreeForAnimations)
        {
            animator.SetFloat(VelocityAnimatorParameter, navMeshAgent.velocity.magnitude / PatrolWalkSpeed);
        }

        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            if (!isWaiting)
            {
                isWaiting = true;
                Invoke(nameof(MoveToNextPatrolPoint), IdleTimeBetweenPatrolPoints);

                if (!UseBlendTreeForAnimations)
                {
                    animator.CrossFadeInFixedTime(IdleAnimationName, 0.5f);
                }
            }
        }

        if (AlertOnDistanceToPlayer)
        {
            CheckForPlayerInView();
        }
    }

    /// Called by an AreaTrigger.
    public void OnTrigger(string triggerName)
    {
        if (AlertOnAreaTrigger && triggerName == AlertOnAreaTriggerObject.gameObject.name) 
        {
            Debug.Log("Triggered By: " + triggerName);
            stateMachine.TransitionToState(EnemyState.Attacking);
        }
    }

    private int currentPatrolIndex = 0;
    private int patrolDirection = 1;
    private void MoveToNextPatrolPoint()
    {
        isWaiting = false;

        if (patrolPointPositions.Count == 0)
            return; 

        currentPatrolIndex += patrolDirection;

        // Check if reached the end or start of patrol points
        if (currentPatrolIndex >= patrolPointPositions.Count)
        {
            currentPatrolIndex = patrolPointPositions.Count - 2; // Move to second last point
            patrolDirection = -1; // Reverse direction
        }
        else if (currentPatrolIndex < 0)
        {
            currentPatrolIndex = 1; // Move to second point
            patrolDirection = 1; // Forward direction
        }

        navMeshAgent.SetDestination(patrolPointPositions[currentPatrolIndex]);
        navMeshAgent.isStopped = false;

        if (!UseBlendTreeForAnimations)
        {
            animator.CrossFadeInFixedTime(WalkingAnimationName, 0.5f);
        }
    }

    private float lineOfSightCheckTime = 0;
    private void CheckForPlayerInView()
    {
        lineOfSightCheckTime -= Time.deltaTime;
        if (lineOfSightCheckTime <= 0)
        {
            if (IsPlayerInView())
            {
                Debug.Log("Player is in view, transitioning to Attack State");
                stateMachine.TransitionToState(EnemyState.Attacking);
            }
            lineOfSightCheckTime = CheckFrequency;
        }
    }

    private bool IsPlayerInView()
    {
        Vector3 playerDirection = player.transform.position - transform.position;

        if (playerDirection.sqrMagnitude <= AlertDistance * AlertDistance)
        {
            RaycastHit hit;
            Vector3 rayOriginOffset = Vector3.up * 0.4f;
            if (CheckLineOfSight && Physics.Raycast(transform.position + rayOriginOffset, playerDirection, out hit, AlertDistance, LineOfSightObstacleMask))
            {
                return false;
            }

            return true;
        }

        return false;
    }
}
