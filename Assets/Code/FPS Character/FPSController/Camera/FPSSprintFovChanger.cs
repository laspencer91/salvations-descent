using System.Collections;
using System.Collections.Generic;
using EventManagement;
using UnityEngine;

[RequireComponent(typeof(CameraSocket))]
public class FPSSprintFovChanger : MonoBehaviour 
{
    [SerializeField] private float _sprintFovIncrease = 10;

    private CameraSocket _cameraSocket;

    private FPSPlayer _player;

    private bool _isAds;

    private float _initialFov;
    
    public void Awake()
    {
        _cameraSocket = GetComponent<CameraSocket>();
        _player       = GetComponentInParent<FPSPlayer>();
    }

    public void Start()
    {
        _initialFov = GetComponentInChildren<Camera>().fieldOfView;
    }

    public void OnEnable()
    {
        _player.Events.OnStartSprint += OnPlayerStartSprint;
        _player.Events.OnEndSprint   += OnPlayerEndSprint;
        _player.Events.OnADSToggle   += OnADSToggle;
    }

    
    public void OnDisable()
    {
        if (_player.Events.OnStartSprint != null)
            _player.Events.OnStartSprint -= OnPlayerStartSprint;
        
        if (_player.Events.OnEndSprint != null)
            _player.Events.OnEndSprint   -= OnPlayerEndSprint;
        
        if (_player.Events.OnADSToggle != null)
            _player.Events.OnADSToggle   -= OnADSToggle;
    }

    
    private void OnPlayerStartSprint()
    {
        _cameraSocket.TargetFov += _sprintFovIncrease;
    }

    
    private void OnPlayerEndSprint()
    {
        _cameraSocket.TargetFov -= _sprintFovIncrease;
    }

    private void OnADSToggle(bool _isAds)
    {
        this._isAds = _isAds;
        Debug.Log(_isAds);
        if (_isAds) _cameraSocket.TargetFov = 40;
        else        _cameraSocket.TargetFov = _initialFov;
    }
}
