using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class EnemyExplosionBehavior : MonoBehaviour
{
    public float explosionForce = 50f;
    public float explosionRadius = 5f;
    public float UpwardForceModifier = 2f;
    public Vector3 NonTransformSpawnOffset = Vector3.zero;
    public List<GameObjectSpawnSetup> enemyPiecesPrefab;

    [Button("Test Explode")]
    public void Explode()
    {
        // Spawn enemy pieces and apply explosion force
        SpawnPiecesAndExplode();
    }

    private void SpawnPiecesAndExplode()
    {
        // Calculate the explosion force direction
        Vector3 explosionPos = transform.position + (NonTransformSpawnOffset / 2);
        foreach (GameObjectSpawnSetup spawnSetup in enemyPiecesPrefab)
        {
            GameObject enemyPiece;

            if (spawnSetup.SpawnOnTransform)
            {
                enemyPiece = Instantiate(spawnSetup.Prefab, spawnSetup.SpawnOnTransform.position, spawnSetup.SpawnOnTransform.rotation);
            }
            else
            {
                Vector3 randomOffset = new Vector3(UnityEngine.Random.Range(-0.5f, 0.5f), 0, UnityEngine.Random.Range(-0.5f, 0.5f));
                enemyPiece = Instantiate(spawnSetup.Prefab, transform.position + NonTransformSpawnOffset + randomOffset, transform.rotation);
            }
        
            Rigidbody pieceRb = enemyPiece.GetComponent<Rigidbody>();
            if (pieceRb != null)
            {
                pieceRb.AddExplosionForce(explosionForce, explosionPos, explosionRadius, UpwardForceModifier);
            }

            // Optionally, you can apply random rotation to the enemy pieces to make the explosion effect more natural
            Vector3 randomRotation = new Vector3(UnityEngine.Random.Range(0, 360f), UnityEngine.Random.Range(0, 360f), UnityEngine.Random.Range(0, 360f));
            enemyPiece.transform.Rotate(randomRotation);
            Destroy(enemyPiece, spawnSetup.TimeToLive);
        }
    }
}

[Serializable]
public struct GameObjectSpawnSetup 
{
    [Optional]
    public Transform SpawnOnTransform;

    [Required]
    public GameObject Prefab;

    public float TimeToLive;
}