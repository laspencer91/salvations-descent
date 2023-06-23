using System;
using _Systems.Helpers;
using UnityEngine;

public class CollisionRespawner : MonoBehaviour
{
    public Transform transformToRespawnAt;

    public GameObject playerGameObject;
    
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.IsPlayer())
        {
            playerGameObject = other.gameObject;
        }
    }

    private void LateUpdate()
    {
        if (playerGameObject != null)
        {
            var charCont = playerGameObject.GetComponent<FPSGroundStateController>();
            charCont.Motor.SetPosition(transformToRespawnAt.position);
            playerGameObject = null;
        }
    }
}
