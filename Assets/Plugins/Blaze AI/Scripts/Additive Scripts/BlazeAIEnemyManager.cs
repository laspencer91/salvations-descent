using System.Collections.Generic;
using UnityEngine;


public class BlazeAIEnemyManager : MonoBehaviour
{
    [HideInInspector] public List<BlazeAI> enemiesScheduled  = new List<BlazeAI>();
    [Tooltip("The amount of time in seconds to send an enemy to attack.")]
    public float attackTimer = 5f;
    [Tooltip("Setting this to false won't let enemies attack instead they'll just be in attack idle state.")]
    public bool callEnemies = true;

    float _timer = 0f;
    
    
    void Update ()
    {
        // run the timer
        _timer += Time.deltaTime;
        if (_timer < attackTimer) {
            return;
        }
        _timer = 0f;


        // filter the enemies
        if (enemiesScheduled.Count > 0) {
            for (int i=0; i<enemiesScheduled.Count; i+=1) {
                // if this AI doesn't even have a target -> remove from manager
                if (!enemiesScheduled[i].enemyToAttack) {
                    RemoveEnemy(enemiesScheduled[i]);
                    continue;
                }

                // if this AI has a target but it's not this -> remove from manager
                if (!enemiesScheduled[i].enemyToAttack.transform.IsChildOf(transform)) {
                    RemoveEnemy(enemiesScheduled[i]);
                    continue;
                }

                // if this AI is dead -> remove from manager
                if (enemiesScheduled[i].state == BlazeAI.State.death) {
                    RemoveEnemy(enemiesScheduled[i]);
                    continue;
                }
            }
        }


        // randomize enemies list and choose one to attack
        if (enemiesScheduled.Count > 0 && callEnemies) {
            // if any of the enemies is attacking -> return
            for (int i=0; i<enemiesScheduled.Count; i+=1) {
                // check if any enemy is attacking
                if (enemiesScheduled[i].isAttacking) {
                    _timer = 0f;
                    return;
                }
            }


            // randomize the enemies
            Shuffle();
            
            
            // choose the first enemy in list after shuffling to attack
            enemiesScheduled[0].Attack();
        }
    }

    // remove a specific enemy from the list
    public void RemoveEnemy(BlazeAI enemy)
    {
        if (enemiesScheduled.IndexOf(enemy) < 0) return;
        enemiesScheduled.Remove(enemy);
    }


    // add an enemy to the scheduled list
    public void AddEnemy(BlazeAI enemy)
    {
        if (enemiesScheduled.IndexOf(enemy) >= 0) return;
        enemiesScheduled.Add(enemy);
    }


    // shuffle the enemiesScheduled list to choose a random one
    void Shuffle()
    {
        var count = enemiesScheduled.Count;
        var last = count - 1;

        for (var i = 0; i < last; ++i) {
            var r = Random.Range(i, count);
            var tmp = enemiesScheduled[i];

            enemiesScheduled[i] = enemiesScheduled[r];
            enemiesScheduled[r] = tmp;
        }
    }
}