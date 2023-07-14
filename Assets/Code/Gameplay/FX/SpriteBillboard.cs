using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SpriteBillboard : MonoBehaviour
{
    private Quaternion rotationOffset;

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
        rotationOffset = transform.localRotation;
    }

    private void LateUpdate()
    {
        Quaternion targetRotation = Quaternion.LookRotation(transform.position - mainCamera.transform.position);
        transform.rotation = targetRotation * rotationOffset;
    }
}