using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerManager : MonoBehaviour
{
    public static TriggerManager Instance;

    private ITriggerListener[] listeners;

    private void Awake() 
    {
        if (Instance != null)
        {
            DestroyImmediate(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        listeners = FindObjectsOfTypeByInterface<ITriggerListener>();
    }

    public static void CallTrigger(string triggerName)
    {
        TriggerManager.Instance._callTrigger(triggerName);
    }

    public void _callTrigger(string triggerName)
    {
        foreach (ITriggerListener listener in listeners)
        {
            listener.OnTrigger(triggerName);
        }
    }

    private T[] FindObjectsOfTypeByInterface<T>() where T : class
    {
        List<T> resultList = new List<T>();
        MonoBehaviour[] allComponents = FindObjectsOfType<MonoBehaviour>();

        foreach (MonoBehaviour component in allComponents)
        {
            if (component is T interfaceObject)
            {
                resultList.Add(interfaceObject);
            }
        }

        return resultList.ToArray();
    }
}
