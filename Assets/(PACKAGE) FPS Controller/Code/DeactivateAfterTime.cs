using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivateAfterTime : MonoBehaviour
{
    public GameObject objectToDeactivate;

    public float secondsToWait = 0.2f;

    private float timer = 0;

    private void OnEnable()
    {
        timer = secondsToWait;
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            objectToDeactivate.SetActive(false);
        }
    }
}
