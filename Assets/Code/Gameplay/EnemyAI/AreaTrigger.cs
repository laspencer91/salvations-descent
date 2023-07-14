using System.Collections;
using System.Collections.Generic;
using _Systems.Helpers;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

public class AreaTrigger : SerializedMonoBehaviour
{
    private void OnTriggerEnter(Collider other) 
    {
        if (GameObjectHelper.IsPlayer(other.gameObject))
        {
            TriggerManager.CallTrigger(gameObject.name);
        }
    }
}
