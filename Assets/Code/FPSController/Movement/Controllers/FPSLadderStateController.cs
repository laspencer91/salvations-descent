using System;
using KinematicCharacterController;
using UnityEngine;

namespace _Systems.FPS_Character.FPSController.Scripts.Movement
{
    public class FPSLadderStateController : FPSMovementStateController, ICharacterController
    {
        public float climbSpeed = 1;
        
        private FPSMouseLook _fpsMouseLook;

        private FPSPlayer _fpsPlayer;

        private bool _isActiveState = false;
        
        private void Awake()
        {
            _fpsMouseLook = GetComponent<FPSMouseLook>();
            _fpsPlayer = GetComponent<FPSPlayer>();
        }

        public override void EnterState()
        {
            Motor.SetMovementCollisionsSolvingActivation(false);
            Motor.SetGroundSolvingActivation(false);
            _isActiveState = true;
            ResetInputs();
        }

        public override void ExitState()
        {
            Motor.SetMovementCollisionsSolvingActivation(true);
            Motor.SetGroundSolvingActivation(true);
            _isActiveState = false;
        }
        
        public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
        {
            currentRotation = _fpsMouseLook.cachedRotation;
        }

        public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
        {
            var upVector = Input.GetAxis("Vertical");
            currentVelocity = Vector3.up * upVector * climbSpeed;
            
            if (_jumpRequested)
            {
                _fpsPlayer.SetCharacterControllerState(CharacterControllerState.GroundMovement);
                currentVelocity += Vector3.back;
            }

            Collider[] col = new Collider[8];
            var colCount = Motor.CharacterOverlap(transform.position, transform.rotation, col, LayerMask.NameToLayer("Ladder"),
                QueryTriggerInteraction.Collide, 0.01f);

            if (colCount > 0)
            {
                Debug.Log(col.Length);
            }

        }

        public void BeforeCharacterUpdate(float deltaTime)
        {
            
        }

        public void PostGroundingUpdate(float deltaTime)
        {
            
        }

        public void AfterCharacterUpdate(float deltaTime)
        {
            
        }

        public bool IsColliderValidForCollisions(Collider coll)
        {
            return true;
        }

        public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
        {
        }

        public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint,
            ref HitStabilityReport hitStabilityReport)
        {
            var verticalInput = Input.GetAxis("Vertical");
            if (verticalInput < 0)
            {
                _fpsPlayer.SetCharacterControllerState(CharacterControllerState.GroundMovement);
            }
        }

        public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition,
            Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
        {
            
        }

        public void OnDiscreteCollisionDetected(Collider hitCollider)
        {
            
        }

        private void OnCollisionExit(Collision other)
        {
            if (_isActiveState)
            {
                if (other.gameObject.CompareTag("Ladder"))
                {
                    _fpsPlayer.SetCharacterControllerState(CharacterControllerState.GroundMovement);
                }
            }
        }
    }
}