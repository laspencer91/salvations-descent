using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CharacterController))]
public class CharacterControllerEvents : MonoBehaviour
{
    [HideInInspector] public UnityEvent onLanding;

    private CharacterController characterController;

    private bool wasInAir;

    private void Awake() 
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        DetectLanding();
    }

    private void DetectLanding() 
    {
        // Check if the character was in the air in the previous frame
        bool isInAir = !characterController.isGrounded;

        // Check if the character landed on the ground in the current frame
        if (wasInAir && !isInAir)
        {
            // Fire the landing event
            onLanding.Invoke();
        }

        // Update the flag for the next frame
        wasInAir = isInAir;
    }
}
