
using UnityEngine;

public static class AITool
{
    private static Transform playerTransform;

    public static bool IsPlayerInView(Transform originTransform, float maxAngle, float maxDistance) 
    {   
        if (playerTransform == null)
        {
            playerTransform = GameObject.FindWithTag("Player").transform;
        }

        // Calculate the direction from the enemy to the player
        Vector3 directionToPlayer = playerTransform.position - originTransform.position;
        float distanceToPlayer = directionToPlayer.magnitude;
        directionToPlayer.Normalize();

        // Calculate the angle between the enemy's forward vector and the direction to the player
        float angleToPlayer = Vector3.Angle(originTransform.forward, directionToPlayer);

        // Check if the player is within the detection angle
        return distanceToPlayer <= maxDistance && angleToPlayer <= maxAngle;
    }
}