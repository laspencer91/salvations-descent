using System.Collections;
using System.Collections.Generic;
using _Systems.Audio;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class EnemyHeadBehavior : MonoBehaviour, IDamageable
{
    public int HeadShotDamageMultiplier = 2;

    public AudioEvent HeadshotAudioEvent;

    private MeleeEnemyController enemyController;

    private AudioSource audioSource;

    private void Awake() 
    {
        enemyController = GetComponentInParent<MeleeEnemyController>();
        audioSource = GetComponentInParent<AudioSource>();
    }

    public void TakeDamage(int damage)
    {
        HeadshotAudioEvent.Play2DSound();
        enemyController.TakeDamage(damage * HeadShotDamageMultiplier);

        GameManager.RecordHeadshot();
    }
}
