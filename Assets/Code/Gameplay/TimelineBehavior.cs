using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TimelineBehavior : MonoBehaviour
{
    public List<TimelineEvent> events;

    public void Run()
    {
       StartCoroutine(RunTimelineSequence());
    }

    private IEnumerator RunTimelineSequence()
    {
        foreach (var timelineEvent in events)
        {
            yield return new WaitForSeconds(timelineEvent.TimeToTriggerSeconds);

            // Trigger the event
            timelineEvent.OnEventTrigger?.Invoke();
        }
    }
}

[Serializable]
public class TimelineEvent
{
    [Tooltip("This is how many seconds to wait after the previous event is triggered.")]
    public float TimeToTriggerSeconds;

    public UnityEvent OnEventTrigger;
}