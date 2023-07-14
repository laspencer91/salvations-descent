using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIStateBehavior<StateTypes> : MonoBehaviour
{
    protected IStateMachine<StateTypes> stateMachine;

    protected void Awake() 
    {
        stateMachine = GetComponent<IStateMachine<StateTypes>>();
    }

    public abstract void AwakeState();

    public abstract void EnterState();

    public abstract void ExitState();
}

