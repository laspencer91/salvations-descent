using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaEnemyTrackerBehavior : MonoBehaviour
{
    public string Name = "Enemy Tracker";
    public Trigger TriggerToCallOnEnemiesAllDead;
    public List<Transform> TrackedEnemies = new List<Transform>();

    private bool allEnemiesDead = false;

    private void FixedUpdate()
    {
        if (!allEnemiesDead)
        {
            CheckAllEnemiesDead();
        }
    }

    private void CheckAllEnemiesDead()
    {
        for (int i = TrackedEnemies.Count - 1; i >= 0; i--)
        {
            if (TrackedEnemies[i] == null)
            {
                TrackedEnemies.RemoveAt(i);
            }
        }

        if (TrackedEnemies.Count <= 0)
        {
            // All enemies are dead, call your function here
            OnAllEnemiesDead();
            allEnemiesDead = true;
        }
    }

    private void OnAllEnemiesDead()
    {
        // This is where you can add the functionality to be executed when all enemies are dead.
        Debug.Log("All enemies are dead!");
        TriggerToCallOnEnemiesAllDead.Emit();
    }
}
