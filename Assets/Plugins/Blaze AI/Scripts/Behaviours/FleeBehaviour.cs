using UnityEngine;
using UnityEngine.Events;

namespace BlazeAISpace
{
    [AddComponentMenu("Flee Behaviour/Flee Behaviour")]
    public class FleeBehaviour : MonoBehaviour
    {   
        [Tooltip("The distance to run to away from the target.")]
        public float distanceRun = 10;

        public float moveSpeed = 5;
        public float turnSpeed = 5;

        public string moveAnim;
        public float moveAnimT = 0.25f;

        [Tooltip("Go to a specific position.")]
        public bool goToPosition;
        [Tooltip("Set the specific position to go to.")]
        public Vector3 setPosition;
        [Tooltip("Shows the specific position point in the scene view (green circle marked as flee position)")]
        public bool showPosition;
        [Tooltip("Fire an event when the specific position is reached.")]
        public UnityEvent reachEvent;

        [Tooltip("Play an audio when fleeing. Set the audio in the audio scriptable. Fleeing array.")]
        public bool playAudio;
        [Tooltip("If enabled, an audio will always play when fleeing. If set to false, there is a 50/50 chance whether an audio will be played or not.")]
        public bool alwaysPlayAudio;


        BlazeAI blaze;
        GameObject lastEnemy;
        Vector3 fleePosition;

        public Vector3 fleeingTo {
            get {
                return fleePosition;
            }
        }

        bool isMoving;
        float cornersDist;
        int _framesElapsed = 0;


        #region UNITY BEHAVIOURS

        void Start()
        {
            blaze = GetComponent<BlazeAI>();

            if (blaze.state != BlazeAI.State.attack) {
                enabled = false;
            }
        }

        void OnDisable()
        {
            // reset flags, except if hit state
            if (blaze.state != BlazeAI.State.hit) {
                lastEnemy = null;
                blaze.isFleeing = false;
            }
        }

        void OnEnable()
        {
            _framesElapsed = 5;
            
            if (blaze == null) {
                blaze = GetComponent<BlazeAI>();
            }

            blaze.isFleeing = true;
            PlayAudio();
        }

        void OnValidate()
        {
            if (blaze == null) {
                blaze = GetComponent<BlazeAI>();
            }

            if (goToPosition && setPosition == Vector3.zero) {
                setPosition = transform.position;
            }
        }

        #if UNITY_EDITOR
        void OnDrawGizmosSelected()
        {
            if (blaze == null) return;

            if (goToPosition && showPosition) 
            {
                if (blaze.groundLayers.value == 0) {
                    Debug.LogWarning("Ground layers property not set. Make sure to set the ground layers in the main Blaze inspector (general tab) in order to see the flee points visually.");
                }

                RaycastHit hit;
                if (Physics.Raycast(setPosition, -Vector3.up, out hit, Mathf.Infinity, blaze.groundLayers)) {
                    Debug.DrawRay(transform.position, setPosition - transform.position, new Color(1f, 0.3f, 0f), 0.1f);
                    Debug.DrawRay(setPosition, hit.point - setPosition, new Color(1f, 0.3f, 0f), 0.1f);

                    UnityEditor.Handles.color = new Color(0.3f, 1f, 0f);
                    UnityEditor.Handles.DrawWireDisc(hit.point, blaze.transform.up, 0.5f);
                    UnityEditor.Handles.Label(hit.point + new Vector3(0, 1, 0), "Flee Position");
                }
            }
        }
        #endif

        void Update()
        {
            if (blaze.enemyToAttack != null) {
                lastEnemy = blaze.enemyToAttack;
                Flee();
                return;
            }
            
            if (lastEnemy != null) {
                Flee();
                return;
            }

            blaze.SetState(BlazeAI.State.alert);
        }

        #endregion

        #region BEHAVIOUR

        void Flee()
        {
            if (goToPosition) 
            {
                if (_framesElapsed >= 5) 
                {
                    cornersDist = blaze.CalculateCornersDistanceFrom(transform.position, setPosition);
                    _framesElapsed = 0;
                }
                else {
                    _framesElapsed++;
                }
                
                float radius = blaze.navmeshAgent.radius * 2;
                
                if (cornersDist <= radius) 
                {
                    reachEvent.Invoke();
                    blaze.SetState(BlazeAI.State.alert);
                    return;
                }

                Move(setPosition);
                return;
            }

            
            if (isMoving) 
            {
                Move(fleePosition);
                return;
            }


            float distance = (transform.position - lastEnemy.transform.position).sqrMagnitude;
            if (distance >= distanceRun * distanceRun) 
            {
                if (blaze.enemyToAttack == null) 
                {
                    blaze.SetState(BlazeAI.State.alert);
                    return;
                }
            }

            
            Vector3 fleeDir = lastEnemy.transform.position - transform.position;
            fleePosition = transform.position - fleeDir;

            if (!blaze.IsPathReachable(fleePosition)) {
                fleePosition = blaze.RandomSpherePoint(transform.position, distanceRun);
            }

            Move(fleePosition);
        }

        void Move(Vector3 pos)
        {
            if (blaze.MoveTo(pos, moveSpeed, turnSpeed, moveAnim, moveAnimT)) {
                isMoving = false;
                return;
            }

            isMoving = true;
        }

        void PlayAudio()
        {
            if (blaze == null) return;

            if (blaze.IsAudioScriptableEmpty() || !playAudio) {
                return;
            }

            if (!alwaysPlayAudio) {
                int rand = Random.Range(0, 2);
                if (rand == 0) {
                    return;
                }
            }

            blaze.PlayAudio(blaze.audioScriptable.GetAudio(AudioScriptable.AudioType.Fleeing));
        }

        #endregion
    }
}

