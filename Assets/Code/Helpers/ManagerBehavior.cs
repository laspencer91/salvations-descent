using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ManagerBehavior<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance;

    private void Awake() 
    {
        if (Instance != null)
        {
            Debug.LogWarning(this.name + " already has a Singleton in existence. This new one will be destroyed.");
            DestroyImmediate(gameObject);
            return;
        }

        Instance = this as T;
        SingletonAwake();
    }

    protected abstract void SingletonAwake();
}
