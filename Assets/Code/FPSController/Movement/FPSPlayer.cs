using _Systems.Audio;
using _Systems.FPS_Character.FPSController.Scripts.Movement;
using Cinemachine;
using EventManagement;
using KinematicCharacterController;
using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(FPSStanceHandler), typeof(FPSGroundStateController), typeof(AudioSource))]
public class FPSPlayer : MonoBehaviour
{
    public KinematicCharacterMotor Motor;

    public int CurrentHealth = 100;

    [BoxGroup("Take Damage")] public AudioEvent TakeDamageAudioEvent;
    [BoxGroup("Take Damage")] public CinemachineImpulseSource ShakeImpulseSource;
    [BoxGroup("Take Damage")] public float TakeDamageImpulseMultiplier = 1.0f;
    [BoxGroup("Take Damage")] public Trigger OnPlayerDeathTrigger;

    // Components
    private FPSStanceHandler            _stanceHandler;
    private FPSInput                    _input;
    private FPSMovementStateController  _activeMovementController;
    private FPSMovementStateController  _groundMovementController;
    private FPSMovementStateController  _ladderMovementController;
    private AudioSource                 _audioSource;
    
    // Event management system that is accessable from all child components. This is a good
    // way for components to listen for messages between each other without being tightly coupled.
    public readonly PlayerEvents Events = new PlayerEvents();
    
    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        _stanceHandler            = GetComponent<FPSStanceHandler>();
        _groundMovementController = GetComponent<FPSGroundStateController>();
        _ladderMovementController = GetComponent<FPSLadderStateController>();

        _groundMovementController.Motor = Motor;
        _ladderMovementController.Motor = Motor;

        _audioSource = GetComponent<AudioSource>();
        
        SetCharacterControllerState(CharacterControllerState.GroundMovement);
    }

    public bool IsDead = false;

    public void TakeDamage(int damage)
    {
        CurrentHealth -= damage;
        ScreenFlash.FlashScreen(FlashType.Damage);
        TakeDamageAudioEvent.Play(_audioSource);
        ShakeImpulseSource.GenerateImpulse(damage * TakeDamageImpulseMultiplier);

        if (CurrentHealth <= 0)
        {
            CurrentHealth = 0;

            if (!IsDead)
            {
                OnPlayerDeathTrigger.Emit();
                IsDead = true;
            }
        }
    }

    [Button]
    public void SetCharacterControllerState(CharacterControllerState state)
    {
        // Wont be set on first call. Check it
        if (_activeMovementController != null)
        {
            _activeMovementController.ExitState();
        }

        switch (state)
        {
            case CharacterControllerState.GroundMovement:
                Motor.CharacterController = (ICharacterController) _groundMovementController;
                _activeMovementController = _groundMovementController;
                break;
            case CharacterControllerState.LadderMovement:
                Motor.CharacterController = (ICharacterController) _ladderMovementController;
                _activeMovementController = _ladderMovementController;
                break;
        }
        
        _activeMovementController.EnterState();
    }
    
    
    void Update()
    {
        if (GameManager.GetGameState() == GameState.LevelComplete || GameManager.GetGameState() == GameState.GameOver) 
            return;

        ClearKeys();
        
        // Stance Handler
        if (_stanceHandler != null)
        {
            if (Input.GetKeyDown(KeyCode.C))
                _stanceHandler.Crouch();
        }

        // Sprinting
        _input.SprintKey = Input.GetButton("Sprint");
        
        // Jumping
        if (Input.GetButtonDown("Jump"))
        {
            if (_stanceHandler == null || _stanceHandler.GetStance() == _stanceHandler.StandStance)
                    _input.JumpKey = true;
            else
                _stanceHandler.Stand();
        }

        // Movement
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput   = Input.GetAxisRaw("Vertical");
        
        ApplyMovement(horizontalInput, verticalInput);

        _activeMovementController.SetInputs(_input);
    }

    void ApplyMovement(float hInput, float vInput)
    {
        Vector3 hTarget = transform.right * hInput;
        Vector3 vTarget = transform.forward * vInput;

        Vector3 targetDirection = (hTarget + vTarget).normalized;

        _input.InputMotion = targetDirection;
    }

    private void ClearKeys()
    {
        _input.InputMotion = Vector3.zero;
        _input.JumpKey     = false;
        _input.SprintKey   = false;
    }
}

public struct FPSInput
{
    public Vector3 InputMotion { get; set; }
    public bool JumpKey { get; set; }
    public bool SprintKey { get; set; }
}

public enum CharacterControllerState
{
    GroundMovement,
    LadderMovement
}