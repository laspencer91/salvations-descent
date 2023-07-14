using UnityEngine;
using Sirenix.OdinInspector;
using _Systems.Audio;

[RequireComponent(typeof(UnityEngine.AI.NavMeshAgent))]
class MeleeEnemyController : BehaviorStateManager<EnemyState>
{
    [BoxGroup("Basic Attack Attributes")]
    public int AttackDamage = 10;

    [BoxGroup("Audio")]
    public AudioEvent FootstepAudioEvent;
    [BoxGroup("Audio")]
    public AudioEvent AttackHitAudioEvent;

    [BoxGroup("State Behaviors")]
    public EnemyState startState = EnemyState.Idle;

    [BoxGroup("State Behaviors")]
    [Required("Add an AIStateBehavior component, then select it from this list.")]
    [ValueDropdown("GetStateBehaviorDropdownList")]
    public AIStateBehavior<EnemyState> IdleStateBehavior;

    [BoxGroup("State Behaviors")]
    [Required("Add an AIStateBehavior component, then select it from this list.")]
    [ValueDropdown("GetStateBehaviorDropdownList")]
    public AIStateBehavior<EnemyState> AttackStateBehavior;

    private AIStateBehavior<EnemyState> currentState;

    private EnemyDamageTrigger damageTrigger;

    private AudioSource audioSource;

    private Animator animator;

    // <summary> This takes place of MonoBehavior Awake, but is still called in the Awake Cycle.</summary>
    protected override void AwakeStateController() 
    {
        damageTrigger = GetComponentInChildren<EnemyDamageTrigger>();
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();

        TransitionToState(startState);
    }

    private void Update() 
    {
        // States are monobehaviors, so they are always playing.
    }

    public override void TransitionToState(EnemyState state)
    {
        if (currentState != null)
        {
            currentState.ExitState();
            currentState.enabled = false;
        }

        switch(state)
        {
            case EnemyState.Idle:
                currentState = IdleStateBehavior;
                break;
            case EnemyState.Attacking:
                currentState = AttackStateBehavior;
                break;
        }

        currentState.EnterState();
        currentState.enabled = true;
    }

    public void OnFootHitGroundKeyFrame()
    {
        FootstepAudioEvent.Play(audioSource);
    }

    public void OnAttackKeyFrameReached()
    {
        FPSPlayer player = damageTrigger.GetPlayerCollision();
        if (player)
        {
            player.TakeDamage(AttackDamage);
            AttackHitAudioEvent.Play(audioSource);
        }
    }

}