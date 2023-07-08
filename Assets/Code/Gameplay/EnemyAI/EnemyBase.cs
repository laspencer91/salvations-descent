using UnityEngine;

[RequireComponent(typeof(UnityEngine.AI.NavMeshAgent))]
class EnemyBase : MonoBehaviour
{
    [Tooltip("When an enemy stops to attack, we don't want him reset if the player takes one step back. The distance buffer solves this.")]
    public float AttackDistanceBuffer = 10f;

    private EnemyState state = EnemyState.Passive;

    private Transform playerTransform;

    private UnityEngine.AI.NavMeshAgent navAgent;

    private float navAgentDestinationRecalculationDelta = 1f;

    private float currentNavAgentDestinationRecalculationTime = 0;

    private void Awake() 
    {
        playerTransform = GameObject.FindWithTag("Player").transform;
    }

    private void Update() 
    {
        switch (state)
        {
            case EnemyState.Passive: ExecutePassiveState(); break;
            case EnemyState.Pursuing: ExecutePursuingState(); break;
            case EnemyState.Attacking: ExecuteAttackingState(); break;
        }
    }

    private void ExecutePassiveState()
    {
        if (IsPlayerInView(30, 60))
        {
            state = EnemyState.Pursuing;
        }
    }

    protected void IsPlayerInView(float maxAngle, float maxDistance) 
    {
        // Calculate the direction from the enemy to the player
        Vector3 directionToPlayer = playerTransform.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;
        directionToPlayer.Normalize();

        // Calculate the angle between the enemy's forward vector and the direction to the player
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        // Check if the player is within the detection angle
        return distanceToPlayer <= maxDistance && angleToPlayer <= maxAngle;
    }

    private void ExecutePursuingState()
    {
        // Recalculate destination to the player every once in a while.
        if (currentNavAgentDestinationRecalculationTime <= 0) 
        {
            navAgent.SetDestination(playerTransform.position);
            currentNavAgentDestinationRecalculationTime = navAgentDestinationRecalculationDelta;
        }
        else
        {
            currentNavAgentDestinationRecalculationTime -= 1;
        }

        float distanceToPlayer = (playerTransform.position - transform.position).magnitude;
        if (distanceToPlayer <= maximumAttackDistance)
        {
            state = EnemyState.Attacking;
        }
    }

    private void ExecuteAttackingState()
    {
        float distanceToPlayer = (playerTransform.position - transform.position).magnitude;
        if (distanceToPlayer > maximumAttackDistance + AttackDistanceBuffer)
        {
            state = EnemyState.Pursuing;
        }
    }

    protected void Attack()
    {
        Debug.Log("Attacking!");
    }
}