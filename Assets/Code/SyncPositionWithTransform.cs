using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncPositionWithTransform : MonoBehaviour
{
    public Transform cameraTransform; // Reference to the camera's transform

    public float positionSyncStrength = 200f;

    public float rotationSyncStrength = 100f;

    private Vector3 gunOffset; // Offset between the camera and gun

    private void Start()
    {
        // Calculate the initial offset between the camera and gun
        gunOffset = transform.position - cameraTransform.position;
    }

    private void LateUpdate()
    {
        // Set the position of the GunHolder to the camera's position plus the gun offset
        transform.position = Vector3.Lerp(transform.position, cameraTransform.position + gunOffset, positionSyncStrength * Time.deltaTime);

        // Calculate the target rotation by aligning the GunPivot's forward direction with the camera's forward direction
        Quaternion targetRotation = Quaternion.LookRotation(cameraTransform.forward, transform.up);

        // Smoothly interpolate the rotation using Quaternion.Lerp
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSyncStrength);
    }
}
