using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Trigger", menuName = "Game/Create Trigger")]
public class Trigger : ScriptableObject
{
    public string Name;

    public void Emit()
    {
        TriggerManager.CallTrigger(Name);
    }

    internal bool Is(string triggerName)
    {
        return Name == triggerName;
    }
}
