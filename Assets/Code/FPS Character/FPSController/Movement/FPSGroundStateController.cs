using System;
using System.Collections;
using _Systems.Audio;
using FirstPersonMovement;
using InternalRealtimeCSG;
using KinematicCharacterController;
using KinematicCharacterController.Walkthrough.ClimbingLadders;
using unity.Assets._Scripts.FPS.Movement.Types;
using UnityEngine;

[RequireComponent(typeof(KinematicCharacterMotor))]
public class FPSGroundStateController : FPSMovementStateController, ICharacterController
{
    public FPSGroundMovementProfile MovementProfile;
    
	[Header("Jumping")]
	[SerializeField] private AudioEvent _jumpAudioEvent;
	
	[Header("Audio")] 
	[SerializeField] private MaterialToFootstepAudioEventConfiguration _footstepEventConfiguration;
	[SerializeField] private float _velocityMuteThreshold = 0.1f;
	[SerializeField] private float _footstepAudioPlaySpread = 0.5f;
	
	private float _footstepAudioPlaybackTimer = 0;

	private AudioSource _audioSource;
	
	// Objects
	private FrameState       _previousTick;			// State at the end of the Previous Fixed Update
	private FPSStanceHandler _stanceHandler;        // Reference to the handler assigned to this player
	private FPSPlayer        _player;				// Reference to our player
	private FPSMouseLook     _fpsMouseLook;         // Reference to mouse look component

	// State
	private bool    _isJumping;						// Are we in a jumping state
	private bool    _isOnSlope;						// Are we on a slope?
	private bool    _isSprinting;					// Are we sprinting?
	private float   _startingStepOffset;			// The Step Offset that the game begins with, matching Inspector
	private float   _currentSpeedMultiplier = 1;    // Current speed multiplier. Calculated each frame
	private float   _stanceSpeedMultiplier = 1;     // Speed multiplier from current stance.

	private bool _isActiveState = false;

	private void Awake()
	{
		_previousTick  = new FrameState();
		_stanceHandler = GetComponent<FPSStanceHandler>();
		_player        = GetComponent<FPSPlayer>();
		_fpsMouseLook  = GetComponent<FPSMouseLook>();
		_audioSource   = GetComponent<AudioSource>();
	}

	public override void EnterState()
	{
		ResetInputs();
		_isActiveState = true;
	}

	public override void ExitState()
	{
		_isActiveState = false;
	}

	private void Update()
	{
		if (Motor.Velocity.magnitude > _velocityMuteThreshold && Motor.GroundingStatus.IsStableOnGround) 
		{
			if (_footstepAudioPlaybackTimer > 0)
			{
				_footstepAudioPlaybackTimer -= Time.deltaTime * (Motor.Velocity.magnitude / MovementProfile._baseMaxSpeed);
			}
			else
			{
				// Find material under player, look up the audio to play, and play it.
				HandleFootstepAudio();
			}
		}
	}

	/**
	 * This is the main function where we tell the Character motor what the velocity should be.
	 * Updated during FIXED UPDATE
	 */
	public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
	{
		// Apply Speed Multipliers : Handle Sprinting Input
		HandleSprintRequest();

		// Handle Base Movement
		if (Motor.GroundingStatus.IsStableOnGround)
		{
			PerformGroundedMovement(ref currentVelocity,  deltaTime);
		}
		else
		{
			PerformUngroundedMovement(ref currentVelocity, deltaTime);
		}
		
		// Handle Jumping
		if (_jumpRequested && !_isJumping && Motor.GroundingStatus.FoundAnyGround)
		{
			PerformJump(ref currentVelocity);
		}

		if (currentVelocity.magnitude > MovementProfile._maxFallSpeed)
		{
			currentVelocity = currentVelocity.normalized * MovementProfile._maxFallSpeed;
		}
	}
	
	/**
	 * Tell character motor what its new rotation should be
	 */
	public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
	{
		currentRotation = _fpsMouseLook.cachedRotation;
	}

	/**
	 * Movement to execute when player is grounded
	 */
	private void PerformGroundedMovement(ref Vector3 currentVelocity, float deltaTime)
	{
		Vector3 targetMovementVelocity;
		
		// Reorient velocity on slope
		currentVelocity = Motor.GetDirectionTangentToSurface(currentVelocity, Motor.GroundingStatus.GroundNormal) * currentVelocity.magnitude;

		if (_inputVector != Vector3.zero)
		{
			// Apply inputs and accelerate character
			targetMovementVelocity = _inputVector * MovementProfile._baseMaxSpeed * _currentSpeedMultiplier;
			currentVelocity = Vector3.Lerp(currentVelocity, targetMovementVelocity, 1 - Mathf.Exp(-MovementProfile._acceleration * deltaTime));
		}
		else
		{
			// Apply friction since no input
			targetMovementVelocity = new Vector3(0, currentVelocity.y, 0);
			currentVelocity = Vector3.Lerp(currentVelocity, targetMovementVelocity, 1 - Mathf.Exp(-MovementProfile._friction * deltaTime));
		}
	}
	
	
	/**
	 * Movement to execute when the player is not grounded
	 */
	private void PerformUngroundedMovement(ref Vector3 currentVelocity, float deltaTime)
	{
		if (_inputVector.sqrMagnitude > 0f)
		{
			currentVelocity += _inputVector.normalized * MovementProfile._airAcceleration * deltaTime;
		}
		else
		{
			currentVelocity = Vector3.Lerp(currentVelocity, currentVelocity.y * Vector3.up, 1 - Mathf.Exp(-MovementProfile._airFriction * deltaTime));
		}
		
		// Gravity
		currentVelocity += Vector3.down * MovementProfile._gravity * deltaTime;
	}


	void HandleSprintRequest()
	{
		_currentSpeedMultiplier = _stanceSpeedMultiplier;
		if (_sprintRequested && CanSprint())
		{
			_isSprinting = true;
			_currentSpeedMultiplier *= MovementProfile._sprintSpeedMultiplier;
			if (!_previousTick.Sprinting) _player.Events.FireStartSprintEvent();
		}
		else
		{
			_isSprinting = false;
			if (_previousTick.Sprinting)  _player.Events.FireEndSprintEvent();
		}
	}
	

	private bool CanSprint()
	{
		return _player.CanSprint && _inputVector != Vector3.zero;
	}
	

	/**
	 * Perform the steps to Jump
	 */
	private void PerformJump(ref Vector3 currentVelocity)
	{
		// Makes the character skip ground probing/snapping on its next update. 
		// If this line weren't here, the character would remain snapped to the ground when trying to jump. Try commenting this line out and see.
		Motor.ForceUnground();

		// Add to the return velocity and reset jump state
		currentVelocity += (Vector3.up * MovementProfile._jumpPower) - Vector3.Project(currentVelocity, Motor.CharacterUp);
		
		_jumpAudioEvent.Play(_audioSource);
		_isJumping = true;
		_player.Events.FireJumpEvent();
	}


	/**
	 * Used to make sure we dont start the Jump Cooldown Co-Routine Twice
	 */
	private bool _jumpCR_isRunning;
	
	private IEnumerator JumpCoolDown(float timer)
	{
		_jumpCR_isRunning = true;
		while (_isJumping)
		{
			yield return new WaitForSeconds(timer);
			_isJumping = false;
			_jumpCR_isRunning = false;
		}
	}
	
	#region Stance Changed Delegate / Enable : Disable

	private void OnEnable()
	{
		if (_stanceHandler != null)
			_stanceHandler.StanceChangedDelegate += StanceChanged;
	}
    
	private void OnDisable()
	{
		if (_stanceHandler.StanceChangedDelegate != null) 
			_stanceHandler.StanceChangedDelegate -= StanceChanged;
	}
        
	private void StanceChanged(Stance stance)
	{
		_stanceSpeedMultiplier = stance.speedMultiplier;
	}

	#endregion
	

	#region BaseMovementController Overrides

	public void AfterCharacterUpdate(float deltaTime)
	{
		// If grounded and jumping.. Start jump reset countdown
		if (_isJumping && Motor.GroundingStatus.FoundAnyGround)
		{
			if (!_jumpCR_isRunning)
				StartCoroutine(JumpCoolDown(MovementProfile._jumpCooldown));
		}

		_previousTick.Position    = transform.position;
		_previousTick.Grounded    = Motor.GroundingStatus.IsStableOnGround;
		_previousTick.Jumping     = _isJumping;
		_previousTick.InputVector = _inputVector;
		_previousTick.Sprinting   = _isSprinting;
		
		ResetInputs();
	}

	private void OnCollisionEnter(Collision other)
	{
		if (!_isActiveState) return;

		if (other.gameObject.CompareTag("Ladder"))
		{
			_player.SetCharacterControllerState(CharacterControllerState.LadderMovement);
		}
	}

	public void BeforeCharacterUpdate(float deltaTime)
	{

	}

	
	public void PostGroundingUpdate(float deltaTime)
	{
		
	}

	
	public bool IsColliderValidForCollisions(Collider coll)
	{
		return true;
	}

	
	public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
	{
		if (!_previousTick.Grounded)
		{
			//HandleFootstepAudio();
		}
	}

	
	public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint,
		ref HitStabilityReport hitStabilityReport)
	{
	}

	
	public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition,
		Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
	{
		
	}

	public void OnDiscreteCollisionDetected(Collider hitCollider)
	{
		Debug.Log("Discrete Collision With", hitCollider);
	}

	#endregion

	void HandleFootstepAudio()
	{
		Ray footstepRay = new Ray(transform.position, Vector3.down);
		RaycastHit hit;

		if (Physics.Raycast(footstepRay, out hit)) 
		{
			MeshRenderer hitRenderer = hit.transform.GetComponent<MeshRenderer>();
			if (hitRenderer != null) 
			{
				// Find the correct audio event from the assigned AudioEventDictionary.
				AudioEvent footstepAudioEvent = _footstepEventConfiguration.GetFootstepAudioEventForMaterial(hitRenderer.sharedMaterial.mainTexture);
				
				if (footstepAudioEvent)
				{
					_footstepAudioPlaybackTimer = _footstepAudioPlaySpread;
					footstepAudioEvent.Play(_audioSource,
						(Motor.Velocity.magnitude / MovementProfile._baseMaxSpeed * _stanceSpeedMultiplier));
				}
			} 
			else
			{
				TerrainCollider terrainCollider = hit.collider.GetComponent<TerrainCollider>();
				if (terrainCollider) 
				{
					TextureToAlphaMapValue[] textureValues = GetTerrainTexture.GetTextureAtPosition(hit.point);
					foreach (TextureToAlphaMapValue textureValue in textureValues) 
					{
						if (textureValue.alpha > 0.1f)
						{
							// Find the correct audio event from the assigned AudioEventDictionary.
							AudioEvent footstepAudioEvent = _footstepEventConfiguration.GetFootstepAudioEventForMaterial(textureValue.texture);
							Debug.Log("Footstep Audio Event: " + footstepAudioEvent.name);
							if (footstepAudioEvent)
							{
								_footstepAudioPlaybackTimer = _footstepAudioPlaySpread;
								footstepAudioEvent.Play(_audioSource,
									((Motor.Velocity.magnitude / MovementProfile._baseMaxSpeed * _stanceSpeedMultiplier) * textureValue.alpha));
							}
						}
					}
				}
			}
		}
	}
}
