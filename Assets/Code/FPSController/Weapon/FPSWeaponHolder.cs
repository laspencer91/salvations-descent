using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSWeaponHolder : MonoBehaviour
{
	private GameObject _aimDownSightPointObject;

	public FPSPlayer _player;
	
	private Camera _camera;
	
	private Vector3 _sightPointOffset;
	
	public bool _aimDownSight;
	
	
	
	// Use this for initialization
	void Start ()
	{
		_camera = GetComponentInParent<Camera>();
		_player = GetComponentInParent<FPSPlayer>();
			
		_aimDownSightPointObject = GameObject.FindGameObjectWithTag("aimdown-sight-point");
		
		if (_aimDownSightPointObject == null) 
			Debug.Log("No Aimdown Sight Point Found For Weapon Holder");
		else
		{
			_sightPointOffset = _camera.transform.position - _aimDownSightPointObject.transform.position;
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetMouseButtonDown(1))
		{
			_aimDownSight = !_aimDownSight;
			_player.Events.FireADSToggleEvent(_aimDownSight);
		}
		
		if (_aimDownSight)
		{
			transform.localPosition = Vector3.MoveTowards(transform.localPosition, _sightPointOffset, 5f * Time.deltaTime);
		}
		else
		{
			transform.localPosition = Vector3.MoveTowards(transform.localPosition, Vector3.zero, 1f * Time.deltaTime);
		}
	}
}
