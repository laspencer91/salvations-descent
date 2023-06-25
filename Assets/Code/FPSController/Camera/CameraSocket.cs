using UnityEngine;

/**
 * Camera socket is used to have a base point for the camera, and smoothly update the cameras
 * position to its' target.
 */
public class CameraSocket : MonoBehaviour
{
	[SerializeField][Range(0, 10)] private float _positionSmoothAmount = 4f;

	private Transform _currentTargetTransform;
	
	private Vector3 _targetPositionOffset = Vector3.zero;

	private Camera _cam;
	private float  _baseFov;
	private float  _currentFov;
	private bool   _isSprinting;
	
	public Vector3 LeaningOffset    { get; set; }
	public float   TargetFov        { get; set; }
	public float   TargetLeanAngle  { get; set; }
	
	private void Start()
	{
		_cam = GetComponentInChildren<Camera>();
		TargetFov = _cam.fieldOfView;
	}
	
	private void Update()
	{
		if (_currentTargetTransform == null) return;
		
		transform.position = Vector3.Lerp(transform.position - _targetPositionOffset, _currentTargetTransform.position, _positionSmoothAmount * Time.deltaTime);

		transform.position += _targetPositionOffset;
	}

	public void SetTargetTransform(Transform targetTransform)
	{
		_currentTargetTransform = targetTransform;
	}

	public void SetTargetPositionOffset(Vector3 offset)
    {
        _targetPositionOffset = offset;
    }
}