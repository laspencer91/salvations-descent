using System;
using UnityEngine;
using FirstPersonMovement;

public class FPSStanceHandler : MonoBehaviour 
{
    public delegate void StanceChange(Stance stance);
    public StanceChange StanceChangedDelegate;

    [SerializeField] private float _standHeight;
	[SerializeField] private float _crouchHeight;
    [SerializeField] private float _standSpeedMultiplier;
    [SerializeField] private float _crouchSpeedMultiplier;

    [Header("Target Transforms")]
    [SerializeField] private Transform _standingTransform;
    [SerializeField] private Transform _crouchingTransform;
    
    [HideInInspector] public Stance StandStance;
	[HideInInspector] public Stance CrouchStance;
	
	private FPSPlayer             _player;
	private FPSGroundStateController _collider;
	private CameraSocket		  _cameraSocket;
	private Stance 				  _currentStance;
	private bool  				  _hasCamera;
	private Vector3 _normalizedDistanceToBottom;
	
	private void Start()
	{
		_cameraSocket = GetComponentInChildren<CameraSocket>();
		_hasCamera    = (_cameraSocket != null);
		_collider     = GetComponent<FPSGroundStateController>();
		_player       = GetComponent<FPSPlayer>();

		float ccHeight = _collider.Motor.Capsule.height;
		StandStance  = new StandStance(this,  _standHeight  * ccHeight, _standSpeedMultiplier);
		CrouchStance = new CrouchStance(this, _crouchHeight * ccHeight, _crouchSpeedMultiplier);

		_currentStance = StandStance;
		
		_cameraSocket.SetTargetTransform(_standingTransform);

		_normalizedDistanceToBottom = _collider.Motor.CharacterTransformToCapsuleBottom;
	}

	// Stance change functions
	public void Crouch() { _currentStance.Crouch(); }
	public void Stand()  { _currentStance.Stand();  }
	
	private void StanceUpdate(Stance newStance)
	{
		UpdateSocketTarget(newStance);		// Must be before ColliderHeight Update
		UpdateColliderHeight(newStance);
	}

	
	private void UpdateSocketTarget(Stance newStance)
	{
		if (!_hasCamera) return;

		if (newStance == StandStance)
		{
			_cameraSocket.SetTargetTransform(_standingTransform);
		} 
		else if (newStance == CrouchStance)
		{
			_cameraSocket.SetTargetTransform(_crouchingTransform);
		}
	}

	
	private void UpdateColliderHeight(Stance newStance)
	{
		_collider.Motor.SetCapsuleDimensions(_collider.Motor.Capsule.radius, newStance.height, 0);
		var newDistToBottom = _collider.Motor.CharacterTransformToCapsuleBottom;
		var offset = _normalizedDistanceToBottom - newDistToBottom;
		_collider.Motor.SetCapsuleDimensions(_collider.Motor.Capsule.radius, _collider.Motor.Capsule.height, offset.y);
	}


	public bool WouldCollide(Vector3 direction, float distance)
	{
		var distToEdge = _collider.Motor.Capsule.center.y + (_collider.Motor.Capsule.height / 2);
		return Physics.Raycast(transform.position, direction, distToEdge + distance);
	}

	
	public Stance GetStance()
	{
		return _currentStance;
	}

	
	public void SetStance(Stance newStance)
	{
		_currentStance = newStance;

		StanceChangedDelegate?.Invoke(newStance);

		StanceUpdate(newStance);

		_player.CanSprint = (newStance == StandStance);
	}
}

