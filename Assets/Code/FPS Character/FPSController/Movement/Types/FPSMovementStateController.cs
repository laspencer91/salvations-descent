using System;
using KinematicCharacterController;
using UnityEngine;

public abstract class FPSMovementStateController : MonoBehaviour
{
    public KinematicCharacterMotor Motor;
    
    // Inputs
    protected Vector3 _inputVector;					// The normalized direction vector of the input
    protected bool    _jumpRequested;					// If a jump has been requested between FixedUpdates. Sent by player
    protected bool    _sprintRequested;			    // If a sprint has been requested between FixedUpdates

    public abstract void EnterState();

    public abstract void ExitState();
    
    /**
	 * Important that we have the key requests since the Character update is fired from the Fixed Update System
	 */
    public void SetInputs(FPSInput fpsInput)
    {
        Debug.Log("Motor", Motor);
        if (fpsInput.JumpKey)   _jumpRequested = true;
        if (fpsInput.SprintKey) _sprintRequested = true;
		
        _inputVector = fpsInput.InputMotion;
    }
    
    /**
	 * Reset inputs should be called after character update
	 */
    protected void ResetInputs()
    {
        _jumpRequested = false;
        _sprintRequested = false;
        _inputVector = Vector3.zero;
    }
}