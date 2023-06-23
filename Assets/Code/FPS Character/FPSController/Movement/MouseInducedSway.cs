using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseInducedSway : MonoBehaviour
{
    public float amount = 1;
    public float maxAmount = 0.2f;
    public float smoothness = 0.2f;

    private Vector3 initialLocalPosition;
    
    private void Start()
    {
        initialLocalPosition = transform.localPosition;
    }

    void Update()
    {
        float movementX = -Input.GetAxis("Mouse X") * amount;
        float movementY = -Input.GetAxis("Mouse Y") * amount;
        movementX = Mathf.Clamp(movementX, -maxAmount, maxAmount);
        movementY = Mathf.Clamp(movementY, -maxAmount, maxAmount);

        Vector3 finalPosition = new Vector3(movementX, movementY, 0);
        transform.localPosition = Vector3.Lerp(transform.localPosition, finalPosition + initialLocalPosition,
            Time.deltaTime * smoothness);
    }
}
