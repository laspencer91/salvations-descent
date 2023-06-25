using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ground Movement Profile", menuName = "Game/Ground Movement Profile")]
public class FPSGroundMovementProfile : ScriptableObject {
	[SerializeField] public float Acceleration;
	[SerializeField] public float Friction;
	[SerializeField] public float AirFriction;
	[SerializeField] public float BaseMaxSpeed;
	[SerializeField] public float SprintSpeedMultiplier;
	[SerializeField] public float Gravity;
	[SerializeField] public float MaxFallSpeed;
    
	[Header("Jumping")]
	[SerializeField] public float JumpPower;
	[SerializeField] public float JumpCooldown;
}
