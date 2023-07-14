using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using _Systems.Helpers;

public class EnemyDamageTrigger : MonoBehaviour
{
    private SphereCollider sphereCollider;

    public LayerMask CollisionLayerMask;

    private void Start()
    {
        sphereCollider = GetComponent<SphereCollider>();
        if (sphereCollider == null)
        {
            Debug.LogError("SphereCollider component not found!");
        }
    }

    public FPSPlayer GetPlayerCollision() 
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position + sphereCollider.center, sphereCollider.radius, CollisionLayerMask);
        
        foreach (Collider collider in colliders)
        {
            // Handle the trigger collision
            FPSPlayer player = collider.gameObject.GetComponent<FPSPlayer>();
            if (player)
            {
                return player;
            }
        }

        return null;
    }
}
