using System.Collections;
using System.Collections.Generic;
using _Systems.Audio;
using Sirenix.OdinInspector;
using UnityEngine;

public class TriggerDoorBehavior : MonoBehaviour, ITriggerListener
{
    [Tooltip("Begin door movement on this trigger.")]
    [BoxGroup("Trigger")] [Required] public Trigger ExecuteOnTrigger;
    [Tooltip("Play an audio event when the door is triggered.")]
    [BoxGroup("Trigger")] public AudioEvent AudioEventOnTrigger;
    [Tooltip("If you want a trigger to be emitted when the door is fully moved, use this.")]
    [BoxGroup("Trigger")] public Trigger EmitTriggerOnEnd;

    public Vector3 OpenOffset;
    public float MoveDuration = 1.0f;
    private Vector3 initialPosition;
    private Vector3 targetPosition;
    private AudioSource audioSource;
    private bool hasBeenTriggered = false;

    private void Awake() 
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        initialPosition = transform.position;
        targetPosition = transform.position + OpenOffset;
    }

    public void OnTrigger(string triggerName)
    {
        if (!hasBeenTriggered && ExecuteOnTrigger.Is(triggerName))
        {
            hasBeenTriggered = true;
            StartCoroutine(MoveToTargetPosition());
        }
    }

    [Button]
    public void TestMove()
    {
        StartCoroutine(MoveToTargetPosition());
    }

    private IEnumerator MoveToTargetPosition()
    {
        float elapsedTime = 0.0f;
        Vector3 startingPosition = transform.position;

        // Play Audio
        if (AudioEventOnTrigger && audioSource)
        {
            AudioEventOnTrigger.Play(audioSource);
        }

        while (elapsedTime < MoveDuration)
        {
            transform.position = Vector3.Lerp(startingPosition, targetPosition, elapsedTime / MoveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
        if (EmitTriggerOnEnd)
        {
            EmitTriggerOnEnd.Emit();
        }
    }
}