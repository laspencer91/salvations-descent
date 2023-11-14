using System.Collections;
using System.Collections.Generic;
using _Systems.Helpers;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

public class AreaTrigger : SerializedMonoBehaviour
{
    [BoxGroup("Editor Only")]
    public Color TriggerVisualizationColor = new Color(0.1f, 0.8f, 0.1f, 0.25f);
    public Trigger Trigger;

    private void OnTriggerEnter(Collider other) 
    {
        if (GameObjectHelper.IsPlayer(other.gameObject))
        {
            Trigger.Emit();
        }
    }
    
    private void OnDrawGizmos()
    {
        if (GetComponent<BoxCollider>() != null)
        {
            BoxCollider collider = GetComponent<BoxCollider>();
            Vector3 center = collider.bounds.center;
            Vector3 size = collider.bounds.size;

            Gizmos.color = TriggerVisualizationColor;
            Gizmos.DrawCube(center, size);

            Color outlineColor = new Color(1f, 1f, 1f, 0.6f);
            Gizmos.color = outlineColor;
            Gizmos.DrawWireCube(center, size);
        }
    }
}
