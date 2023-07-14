using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using _Systems.Audio;

public class Crate : MonoBehaviour, IDamageable
{
    public int Hitpoints = 1;

    public GameObject DestructionEffectPrefab;

    public AudioEvent DestructionSoundEffect;

    public void TakeDamage(int damage)
    {
        Hitpoints -= damage;

        if (Hitpoints <= 0) 
        {
            Instantiate(DestructionEffectPrefab, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }
}