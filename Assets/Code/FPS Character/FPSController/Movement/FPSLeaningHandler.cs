using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSLeaningHandler : MonoBehaviour
{
	[SerializeField] private float _maxLeanDistance;
	[SerializeField] private float _maxLeanAngle;
	
	private Vector3 _maxLeanVector;

	private bool _isSprinting;
	
	private FPSPlayer    _player;
	private CameraSocket _cameraSocket;
	private Direction    _leanDirection;


	public void Start()
	{
		_player 	   = GetComponent<FPSPlayer>();
		_cameraSocket  = GetComponentInChildren<CameraSocket>();
		_maxLeanVector = Vector3.right * _maxLeanDistance;
		
		// Register event handlers
		_player.Events.OnStartSprint += OnPlayerStartSprint;
		_player.Events.OnEndSprint   += OnPlayerStopSprint;
	}

	
	void OnEnable()
	{
		if (_player == null) return;
		
		_player.Events.OnStartSprint += OnPlayerStartSprint;
		_player.Events.OnEndSprint   += OnPlayerStopSprint;
	}
	
	
	void OnDisable()
	{
		_player.Events.OnStartSprint -= OnPlayerStartSprint;
		_player.Events.OnEndSprint   -= OnPlayerStopSprint;
	}
	
	
	public void Update()
	{
		if (Input.GetKey(KeyCode.Q))
			LeanLeft();
		if (Input.GetKey(KeyCode.E))
			LeanRight();

		if (!Input.GetKey(KeyCode.Q) && !Input.GetKey(KeyCode.E))
			StopLeaning();
		
		if (_isSprinting) StopLeaning();
	}
	
	
	public void LeanLeft()
	{
		if (_leanDirection == Direction.Left || _isSprinting) return;
		
		_cameraSocket.LeaningOffset -= _maxLeanVector;
		_cameraSocket.TargetLeanAngle  += _maxLeanAngle;
		_leanDirection = Direction.Left;
		
		_player.Events.FireLeanEvent(_leanDirection);
	}
	
	
	public void LeanRight()
	{
		if (_leanDirection == Direction.Right || _isSprinting) return;
		
		_cameraSocket.LeaningOffset += _maxLeanVector;
		_cameraSocket.TargetLeanAngle  -= _maxLeanAngle;
		_leanDirection = Direction.Right;
		
		_player.Events.FireLeanEvent(_leanDirection);
	}

	
	public void StopLeaning()
	{
		if (_leanDirection == Direction.None) return;

		if (_leanDirection == Direction.Left)
		{
			_cameraSocket.LeaningOffset   = Vector3.zero;
			_cameraSocket.TargetLeanAngle = 0;
		}
		else if (_leanDirection == Direction.Right)
		{
			_cameraSocket.LeaningOffset   = Vector3.zero;
			_cameraSocket.TargetLeanAngle = 0;
		}

		_leanDirection = Direction.None;
		_player.Events.FireLeanEvent(_leanDirection);
	}
	

	private void OnPlayerStartSprint()
	{
		_isSprinting = true;
	}

	private void OnPlayerStopSprint()
	{
		_isSprinting = false;
	}
}

public enum Direction { None, Left, Right }