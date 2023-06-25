using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSWeaponSway : MonoBehaviour
{
	[SerializeField] private float _smoothing;
	[SerializeField] private float _maxSwayDistance;
	
	private Vector3 _startingPosition;

	private Vector2 _mouseDelta;
	
	void Start()
	{
		_startingPosition = transform.localPosition;
	}
	
	void Update()
	{
		_mouseDelta.x = Input.GetAxis( "Mouse X" );
		_mouseDelta.y = Input.GetAxis( "Mouse Y" );

		
		if (_mouseDelta.magnitude > _maxSwayDistance)
			_mouseDelta = _mouseDelta.normalized * _maxSwayDistance;
		
		Vector3 targetPosition = _startingPosition + new Vector3(-_mouseDelta.x, -_mouseDelta.y, 0);
		
		transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, _smoothing * Time.deltaTime);
	}
}
