using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ground Movement Profile", menuName = "Game/Ground Movement Profile")]
public class FPSGroundMovementProfile : ScriptableObject {
	[SerializeField] public float _acceleration;
	[SerializeField] public float _airAcceleration;
	[SerializeField] public float _friction;
	[SerializeField] public float _airFriction;
	[SerializeField] public float _baseMaxSpeed;
	[SerializeField] public float _sprintSpeedMultiplier;
	[SerializeField] public float _gravity;
	[SerializeField] public float _maxFallSpeed;
    
	[Header("Jumping")]
	[SerializeField] public float _jumpPower;
	[SerializeField] public float _jumpCooldown;
}
