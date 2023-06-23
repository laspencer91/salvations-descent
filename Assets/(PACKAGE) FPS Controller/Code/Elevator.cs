using System;
using System.Collections;
using System.Collections.Generic;
using _Systems.Helpers;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    public Transform pointATransform;

    public Transform pointBTransform;

    public float elevatorSpeed = 5;

    public bool autoReturn;
    
    public float elevatorDirectionChangeCooldown = 5;
    
    private Transform currentTargetTransform;
        
    private bool isTransitioning;
    
    private float transitionDelayTimer;

    private ElevatorState state = ElevatorState.Idle;
    
    void Start()
    {
        pointATransform.position = transform.position;
        currentTargetTransform = pointATransform;
    }
    
    void Update()
    {
        if (state == ElevatorState.Moving)
        {
            var currentPosition = transform.position;
            var toTargetVector = currentTargetTransform.position - currentPosition;
            var targetPosition = Vector3.ClampMagnitude(toTargetVector.normalized * elevatorSpeed * Time.deltaTime, toTargetVector.magnitude);
            transform.position = currentPosition + targetPosition;
            
            if (Vector3.Distance(transform.position, currentTargetTransform.position) <= .05f)
            {
                state = ElevatorState.CoolingDown;
                transitionDelayTimer = elevatorDirectionChangeCooldown;
            }
        }
        else if (state == ElevatorState.CoolingDown)
        {
            transitionDelayTimer -= Time.deltaTime;
            if (transitionDelayTimer <= 0)
            {
                state = ElevatorState.Idle;
                if (autoReturn && currentTargetTransform == pointBTransform)
                {
                    BeginMovement();
                }
            }
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.IsPlayer())
        {
            if (state == ElevatorState.Idle)
            {
                BeginMovement();
            }
        }
    }

    public void BeginMovement()
    {
        if (state == ElevatorState.Idle)
        {
            state = ElevatorState.Moving;
            if (currentTargetTransform == pointATransform)
            {
                currentTargetTransform = pointBTransform;
            }
            else
            {
                currentTargetTransform = pointATransform;
            }
        }
    }
}

enum ElevatorState
{
    Idle,
    Moving,
    CoolingDown
}
