using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

abstract class BehaviorStateManager<T> : MonoBehaviour, IStateMachine<T>
{
    public abstract void TransitionToState(T state);

    // This method is to ensure that all states are initialized correctly before the controller executes.
    protected void Awake() 
    {
        var allStateBehaviors = GetComponents<AIStateBehavior<T>>();
        foreach (var state in allStateBehaviors)
        {
            state.AwakeState();
        }

        AwakeStateController();
    }

    /// <summary> This takes place of MonoBehavior Awake, but is still called in the Awake Cycle.  </summary>
    protected abstract void AwakeStateController();

    // ---------------------------- EDITOR FUNCTION --------------------------------
    #if UNITY_EDITOR
    /// <summary> Editor Utility Function to get list of AIStateBehaviors. </summary>
    protected List<ValueDropdownItem<AIStateBehavior<T>>> GetStateBehaviorDropdownList()
    {
        if (Application.isPlaying) 
        {
            return null;
        }

        AIStateBehavior<T>[] behaviors = GetComponents<AIStateBehavior<T>>();
        var dropdownList = new List<ValueDropdownItem<AIStateBehavior<T>>>();

        foreach (var behavior in behaviors)
        {
            dropdownList.Add(new ValueDropdownItem<AIStateBehavior<T>>(behavior.GetType().Name, behavior));
            behavior.enabled = false;
        }

        return dropdownList;
    }
    #endif
}