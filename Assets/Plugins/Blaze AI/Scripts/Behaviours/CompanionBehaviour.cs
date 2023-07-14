using UnityEngine;

namespace BlazeAISpace
{
    [AddComponentMenu("Companion Behaviour/Companion Behaviour")]
    public class CompanionBehaviour : MonoBehaviour
    {
        [Tooltip("If enabled, the AI will follow the target.")]
        public bool follow = true;
        [Tooltip("The AI will follow when the distance is equal or bigger than this and stop when it's smaller.")]
        public float followIfDistance = 5;
        [Range(0f, 1f), Tooltip("To avoid stuttering movement, this value sets the duration in seconds the AI must pass moving before stopping.")]
        public float movingWindow = 0.3f;


        [Tooltip("The idle animation name to play.")]
        public string idleAnim;
        [Tooltip("The movement animation name to play when following.")]
        public string moveAnim;


        [Tooltip("Speed of movement.")]
        public float moveSpeed = 5;
        [Tooltip("Speed of turning in movement.")]
        public float turnSpeed = 5;


        [Tooltip("The animation to play when follow is set to false.")]
        public string stayPutAnim;


        [Tooltip("If enabled, the AI will wander around the player radius. The radius will be the AIs navmesh agent height * 2 + 0.5.")]
        public bool wanderAround;
        [Tooltip("The time to spend in each wander point before moving again. A random value will be generated between the two set values. For a constant number, set the two values to the same number.")]
        public Vector2 wanderPointTime = new Vector2(3, 5);
        [Tooltip("The movement speed when the AI is wandering.")]
        public float wanderMoveSpeed = 1;
        [Tooltip("A random animation name will be chosen from this list on each wander point. Add animation like looking around, getting bored, etc...")]
        public string[] wanderIdleAnims;
        [Tooltip("The movement animation when the AI is moving to a wandering point. This should most probably be a walking animation but it's up to you.")]
        public string wanderMoveAnim;


        [Tooltip("The transition time between all animations.")]
        public float animsT = 0.25f;


        [Tooltip("If enabled, the AI will have a skin the radius of the navmesh agent radius + 0.1. So if the radius is 0.3 the skin radius will be 0.4. And if any layer set below comes in contact. The AI will move away to a random position.")]
        public bool moveAwayOnContact;
        [Tooltip("If any game object with these layers comes in contact. The AI will move away to a random position.")]
        public LayerMask layersToAvoid;


        [Tooltip("If enabled, the companion AI will play an audio every set time when the follow distance is reached. Whether the AI is wandering around or staying idle doesn't matter. It'll play in both cases.")]
        public bool playAudioForIdleAndWander;
        [Tooltip("A random time will be generated between the two set values. For a constant time, set the two values to the same number.")]
        public Vector2 audioTimer = new Vector2(15, 30);
        [Tooltip("The AI will play audio when follow == false (ex: Ok, I'll stay here) and when follow == true (ex: I'm coming). Set different audio clips for each state in the audio scriptable to give more detail by the AI whether it's following or staying put.")]
        public bool playAudioOnFollowState;


        #region SYSTEM VARAIBLES

        BlazeAI blaze;

        bool stayPutAudioPlayed;
        bool followAudioPlayed;
        bool choseAudioTime;
        bool wanderPointGenerated;
        bool moveToAvoidContact;
        bool isMoving;

        float _audioTimer = 0;
        float _chosenAudioTime;
        float _wanderPointTimer = 0;
        float _chosenWanderPointTime;
        float isMovingTimer = 0;

        string chosenWanderIdleAnim;

        Vector3 wanderPoint;

        #endregion

        #region UNITY METHODS

        void Start()
        {
            blaze = GetComponent<BlazeAI>();


            // if companion mode is disabled -> disable
            if (!blaze.companionMode) {
                enabled = false;
            }
            

            // if companion mode enabled but not correct state -> disable
            if (blaze.companionMode && (blaze.state != BlazeAI.State.normal && blaze.state != BlazeAI.State.alert)) {
                enabled = false;
            }


            if (follow) {
                followAudioPlayed = true;
            }
        }


        void OnDisable()
        {
            ResetFlags();
        }

        
        void Update()
        {
            // if MoveToLocation() API called
            if (blaze.movedToLocation) {
                MoveToCalledLocation();
                return;
            }


            // physics skin to check if any game object comes in contact
            CheckSkin();


            // if skin detected anything came in contact -> move to another point around companion
            if (moveToAvoidContact) {
                MoveToAvoidContact();
                return;
            }


            if (follow) {
                PlayFollowAudio();


                // calculate distance to the object we need to follow
                float distanceToTarget = (blaze.companionTo.position - transform.position).sqrMagnitude;
                float followDistance = followIfDistance;


                // increase distance if wandering
                if (wanderPointGenerated) {
                    followDistance += (wanderPoint - transform.position).sqrMagnitude; 
                }

                // if too far -> move to companion
                if (distanceToTarget >= (followDistance * followDistance)) {
                    FollowTarget();
                    return;
                }

                // to avoid stuttering movement -> once AI has moved it must continue to a total of [movingWindow] seconds before stopping
                if (!CheckIfGoodToStop()) {
                    FollowTarget();
                    return;
                }

                // distance has been reached -> wander or stay still
                if (!wanderAround) {
                    Idle();
                    return;
                }

                
                // wandering
                Wander();
                return;
            }


            // if should not follow then stay put -> play animation and audio
            StayPut();
        }

        #endregion

        #region BEHAVIOUR METHODS

        void Idle()
        {
            isMovingTimer = 0;
            isMoving = false;

            blaze.animManager.Play(idleAnim, animsT);
            wanderPointGenerated = false;
            
            IdleAudio();
        }


        void IdleAudio()
        {
            if (!playAudioForIdleAndWander) {
                _audioTimer = 0;
                return;
            }


            if (blaze.IsAudioScriptableEmpty()) {
                return;
            }


            if (blaze.agentAudio.isPlaying) {
                return;
            }


            if (!choseAudioTime) {
                _chosenAudioTime = Random.Range(audioTimer.x, audioTimer.y);
                choseAudioTime = true;
                _audioTimer = 0;
            }


            _audioTimer += Time.deltaTime;

            if (_audioTimer >= _chosenAudioTime) {
                _audioTimer = 0;
                choseAudioTime = false;

                blaze.PlayAudio(blaze.audioScriptable.GetAudio(AudioScriptable.AudioType.CompanionIdleAndWander));
            }
        }


        void FollowTarget()
        {
            wanderPointGenerated = false;

            // check target to follow isn't empty
            if (blaze.companionTo == null) {
                Idle();
                Debug.Log("There is no Target To Follow set in the Companion Behaviour.");
                return;
            }


            // move the AI to the target position
            if (blaze.MoveTo(blaze.companionTo.position, moveSpeed, turnSpeed, moveAnim, animsT)) {
                Idle();
                return;
            }
            else {
                isMoving = true;
                isMovingTimer += Time.deltaTime;
            }


            // if previous MoveTo() generated a flag that the player's path is unreachable -> stay idle
            if (!blaze.isPathReachable) {
                Idle();
                return;
            }
        }


        void StayPut()
        {
            blaze.animManager.Play(stayPutAnim, animsT);
            followAudioPlayed = false;


            if (!playAudioOnFollowState) {
                return;
            }


            if (blaze.IsAudioScriptableEmpty()) {
                return;
            }


            if (stayPutAudioPlayed) {
                return;
            }


            if (blaze.PlayAudio(blaze.audioScriptable.GetAudio(AudioScriptable.AudioType.CompanionOnStop))) {
                stayPutAudioPlayed = true;
            }   
        }


        void PlayFollowAudio()
        {
            stayPutAudioPlayed = false;


            if (!playAudioOnFollowState) {
                return;
            }


            if (blaze.IsAudioScriptableEmpty()) {
                return;
            }


            if (followAudioPlayed) {
                return;
            }


            if (blaze.PlayAudio(blaze.audioScriptable.GetAudio(AudioScriptable.AudioType.CompanionOnFollow))) {
                followAudioPlayed = true;
            } 
        }


        void GenerateWanderPoint()
        {
            wanderPoint = blaze.RandomSpherePoint(blaze.companionTo.position, (blaze.navmeshAgent.height * 2) + 0.5f);
            float distanceToPoint = blaze.CalculateCornersDistanceFrom(transform.position, wanderPoint);
            
            if (distanceToPoint > followIfDistance) {
                GenerateWanderPoint();
                return;
            }


            if (wanderPoint == Vector3.zero) {
                GenerateWanderPoint();
                return;
            }

            _chosenWanderPointTime = Random.Range(wanderPointTime.x, wanderPointTime.y);
            
            if (wanderIdleAnims.Length > 0) {
                chosenWanderIdleAnim = wanderIdleAnims[Random.Range(0, wanderIdleAnims.Length)];
            }
            
            wanderPointGenerated = true;
        }


        void Wander()
        {
            if (!CheckIfGoodToStop()) return;
            

            if (!wanderPointGenerated) {
                GenerateWanderPoint();
            }

            
            if (!blaze.IsPathReachable(wanderPoint)) {
                Idle();
                return;
            }


            wanderPointGenerated = true;
            IdleAudio();


            if (blaze.MoveTo(wanderPoint, wanderMoveSpeed, turnSpeed, wanderMoveAnim, animsT)) {
                blaze.animManager.Play(chosenWanderIdleAnim, animsT);

                _wanderPointTimer += Time.deltaTime;
                
                if (_wanderPointTimer >= _chosenWanderPointTime) {
                    GenerateWanderPoint();
                    _wanderPointTimer = 0;
                }
            }
        }


        void MoveToCalledLocation()
        {
            if (blaze.MoveTo(blaze.endDestination, moveSpeed, turnSpeed, moveAnim, animsT)) {
                Idle();
            }
        }


        void CheckSkin()
        {
            if (!moveAwayOnContact) {
                return;
            }


            // if AI has been commanded to move to a location -> don't move AI
            if (blaze.movedToLocation) {
                return;
            }


            Collider[] hitColliders = new Collider[1];
            int numColliders = Physics.OverlapSphereNonAlloc(transform.position + blaze.centerPosition, blaze.navmeshAgent.radius + 0.1f, hitColliders, layersToAvoid);

            
            if (numColliders <= 0) {
                return;
            }


            // generate a random point
            wanderPoint = blaze.RandomSpherePoint(blaze.companionTo.position, (blaze.navmeshAgent.height * 2) + 1);
            
            // make sure the generated destination isn't actually far using the path corners
            float distanceToPoint = blaze.CalculateCornersDistanceFrom(transform.position, wanderPoint);
            
            if (distanceToPoint > followIfDistance) {
                CheckSkin();
                return;
            }

            moveToAvoidContact = true;
            _wanderPointTimer = 0;
        }


        void MoveToAvoidContact()
        {
            if (follow) {
                PlayFollowAudio();
            }

            if (blaze.MoveTo(wanderPoint, moveSpeed, turnSpeed, moveAnim, animsT)) {
                moveToAvoidContact = false;
            }
        }


        bool CheckIfGoodToStop()
        {
            if (isMoving) {
                if (isMovingTimer < movingWindow) return false;
                else return true;
            }

            return true;
        }

        void ResetFlags()
        {
            choseAudioTime = false;
            wanderPointGenerated = false;
            _wanderPointTimer = 0;
            moveToAvoidContact = false;
            isMoving = false;
            isMovingTimer = 0;
        }

        #endregion
    }
}
