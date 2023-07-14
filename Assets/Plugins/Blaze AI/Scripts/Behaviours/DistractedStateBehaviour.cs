using UnityEngine;

namespace BlazeAISpace
{
    [AddComponentMenu("Distracted State Behaviour/Distracted State Behaviour")]
    public class DistractedStateBehaviour : MonoBehaviour
    {
        [Header("REACTION TIME"), Tooltip("Time to pass (seconds) before turning to distraction location.")]
        public float timeToReact = 0.2f;


        [Header("CHECKING LOCATION"), Tooltip("If enabled, AI will move to distraction location.")]
        public bool checkLocation = true;
        [Tooltip("Time to pass (seconds) before moving to check location.")]
        public float timeBeforeMovingToLocation = 1f;
        [Tooltip("Animation to play when reaches the distraction destination.")]
        public string checkAnim;
        public float checkAnimT = 0.25f;
        [Tooltip("Amount of time (seconds) to stay in distraction destination before going back to patrolling.")]
        public float timeToCheck = 5f;
        [Tooltip("If enabled, the AI will go to a random point within the distraction location.")]
        public bool randomizePoint;
        [Min(0), Tooltip("Set the randomize radius.")]
        public float randomizeRadius = 5;

        
        [Header("SEARCH RADIUS"), Tooltip("If enabled, the AI after checking the distraction location will randomly search points within the radius of the distraction location. The radius is the AI's NavMesh Agent Height x 2. Check Location must be enabled for this to work.")]
        public bool searchLocationRadius;
        [Range(1, 10), Tooltip("The amount of random points to search.")]
        public int searchPoints = 3;
        [Tooltip("The animation name to play on each search point.")]
        public string searchPointAnim;
        [Min(0), Tooltip("The amount of time to wait in each search point.")]
        public float pointWaitTime = 3;
        [Tooltip("Animation to play after going through all search points. This is the exiting animation.")]
        public string endSearchAnim;
        [Min(0), Tooltip("Set how long you want the end animation to be playing.")]
        public float endSearchAnimTime = 3;
        public float searchAnimsT = 0.25f;


        [Tooltip("Play audio when AI reaches the distraction location.")]
        public bool playAudioOnCheckLocation;
        [Tooltip("Play an audio when AI begins searching.")]
        public bool playAudioOnSearchStart;
        [Tooltip("Play an audio when AI finishes searching.")]
        public bool playAudioOnSearchEnd;
        

        #region BEHAVIOUR VARS

        NormalStateBehaviour normalStateBehaviour;
        AlertStateBehaviour alertStateBehaviour;
        BlazeAI blaze;
        
        
        float _timeToCheck = 0f;
        float _timeToReact = 0f;
        float _timeBeforeMovingToLocation = 0f;
        float moveSpeed = 0;
        float turnSpeed = 0;


        bool turnedToLocation;
        bool playedLocationAudio;
        bool isIdle;
        bool isSearching;

        
        string moveAnim = "";
        string leftTurn = "";
        string rightTurn = "";


        int searchIndex = 0;


        Vector3 distractionPoint;
        Vector3 searchPointLocation;
        Vector3 lastPoint;

        #endregion

        #region UNITY METHODS

        void Start()
        {
            blaze = GetComponent<BlazeAI>();
            normalStateBehaviour = GetComponent<NormalStateBehaviour>();
            alertStateBehaviour = GetComponent<AlertStateBehaviour>();

            if (normalStateBehaviour == null) {
                Debug.Log("Distracted State Behaviour tried to get Normal State Behaviour component but found nothing. It's important to set it manually to get the movement and turning animations and speeds.");
            }

            if (alertStateBehaviour == null) {
                Debug.Log("Distracted State Behaviour tried to get Alert State Behaviour component but found nothing. It's important to set it manually to get the movement and turning animations and speeds.");
            }

            // force shut if not the same state
            if (blaze.state != BlazeAI.State.distracted) {
                enabled = false;
            }
        }

        void OnDisable()
        {
            ResetDistraction();
        }

        void Update()
        {   
            // if forced to stay idle by blaze public method
            if (blaze.stayIdle) {
                ReachedDistractionLocation();
                return;
            }


            GetSpeedsAndTurns();

            if (!PreparePoint()) {
                return;
            }


            // turn to distraction first
            if (!turnedToLocation) {
                _timeToReact += Time.deltaTime;
                
                if (_timeToReact >= timeToReact) {
                    // TurnTo() turns the agent and returns true when fully turned to point
                    if (blaze.TurnTo(distractionPoint, leftTurn, rightTurn, 0.25f)) {
                        turnedToLocation = true;
                        _timeToReact = 0f;
                    }
                }
                else {
                    // play idle anim
                    blaze.animManager.Play(normalStateBehaviour.idleAnim[0], checkAnimT);
                }
            }


            // can't go further if haven't completely turned
            if (!turnedToLocation) {
                return;
            }


            _timeBeforeMovingToLocation += Time.deltaTime;

            if (_timeBeforeMovingToLocation < timeBeforeMovingToLocation) {
                return;
            }


            // AI has reached distraction location and is now searching the radius
            if (isSearching) {
                if (blaze.MoveTo(searchPointLocation, moveSpeed, turnSpeed, moveAnim)) {
                    // stay idle
                    if (!IsSearchPointIdleFinished()) {
                        return;
                    }


                    if (searchIndex < searchPoints) {
                        SetSearchPoint();
                        return;
                    }


                    // reaching this line means the AI has went through all search points and is time to exit
                    EndSearchExit();
                    return;
                }

                return;
            }


            // if should check location
            if (checkLocation) {
                // MoveTo() moves the agent to the destination and returns true when reaches destination
                if (blaze.MoveTo(distractionPoint, moveSpeed, turnSpeed, moveAnim)) {
                    ReachedDistractionLocation();
                }
                else {
                    isIdle = false;
                }
            }
            else {
                turnedToLocation = false;
                isIdle = true;
                _timeBeforeMovingToLocation = 0f;
                blaze.SetState(blaze.previousState);
            }


            SetIdleState();
        }

        #endregion

        #region CHECK LOCATION

        // get move & turn speeds/animations
        void GetSpeedsAndTurns()
        {
            if (blaze.previousState == BlazeAI.State.normal) {
                moveSpeed = normalStateBehaviour.moveSpeed;
                turnSpeed = normalStateBehaviour.turnSpeed;
                moveAnim = normalStateBehaviour.moveAnim;
                leftTurn = blaze.waypoints.leftTurnAnimNormal;
                rightTurn = blaze.waypoints.rightTurnAnimNormal;
            }


            if (blaze.previousState == BlazeAI.State.alert) {
                moveSpeed = alertStateBehaviour.moveSpeed;
                turnSpeed = alertStateBehaviour.turnSpeed;
                moveAnim = alertStateBehaviour.moveAnim;
                leftTurn = blaze.waypoints.leftTurnAnimAlert;
                rightTurn = blaze.waypoints.rightTurnAnimAlert;
            }
        }


        // select a random audio to play when reaching the distraction location
        void PlayAudioOnCheckLocation()
        {
            if (playedLocationAudio) {
                return;
            }


            if (blaze.IsAudioScriptableEmpty()) {
                return;
            }

                
            if (blaze.PlayAudio(blaze.audioScriptable.GetAudio(AudioScriptable.AudioType.DistractionCheckLocation))) {
                playedLocationAudio = true;
            }
        }


        // exit method out of distracted state when reaching location
        void ReachedDistractionLocation()
        {
            // play audio
            if (playAudioOnCheckLocation) {
                PlayAudioOnCheckLocation();
            }


            // play check location animation
            blaze.animManager.Play(checkAnim, checkAnimT);

            // run the timer
            _timeToCheck += Time.deltaTime;
            

            if (_timeToCheck >= timeToCheck) {
                if (searchLocationRadius) {
                    PlaySearchStartAudio();
                    SetSearchPoint();

                    isSearching = true;
                    return;
                }

                
                ResetDistraction();
                ResetStayIdle();
                
                
                blaze.SetState(blaze.previousState);
            }


            isIdle = true;
        }


        void ResetDistraction()
        {
            _timeToCheck = 0;
            _timeBeforeMovingToLocation = 0;
            _timeToReact = 0;
            searchIndex = 0;

            turnedToLocation = false;
            playedLocationAudio = false;
            isSearching = false;
        }


        void ResetStayIdle()
        {
            blaze.stayIdle = false;
        }


        void SetIdleState()
        {
            blaze.isIdle = isIdle;
        }

        bool PreparePoint()
        {
            if (blaze.endDestination == lastPoint) {
                return true;
            }

            lastPoint = blaze.endDestination;

            if (randomizePoint) {
                distractionPoint = blaze.RandomSpherePoint(blaze.endDestination, randomizeRadius, false);
                
                if (blaze.CalculateCornersDistanceFrom(blaze.endDestination, distractionPoint) > randomizeRadius) {
                    lastPoint = Vector3.zero;
                    return false;
                }

                return true;
            }

            distractionPoint = blaze.endDestination;
            return true;
        }
        

        #endregion

        #region SEARCHING

        // set the next search point
        void SetSearchPoint()
        {
            searchPointLocation = blaze.RandomSpherePoint(distractionPoint, (blaze.navmeshAgent.height * 2) + 2);
            
            // make sure never returns 0
            if (searchPointLocation == Vector3.zero) {
                SetSearchPoint();
                return;
            }
            
            searchIndex++;
            _timeToCheck = 0;
        }


        // play search point idle anim and return a bool whether the time has finished or not
        bool IsSearchPointIdleFinished()
        {
            blaze.animManager.Play(searchPointAnim, searchAnimsT);
            isIdle = true;
            _timeToCheck += Time.deltaTime;
            
            
            if (_timeToCheck >= pointWaitTime) {
                return true;
            }


            return false;
        }


        // play start search audio
        void PlaySearchStartAudio()
        {
            if (!playAudioOnSearchStart) {
                return;
            }


            if (blaze.IsAudioScriptableEmpty()) {
                return;
            }


            blaze.PlayAudio(blaze.audioScriptable.GetAudio(AudioScriptable.AudioType.SearchStart));
        }


        // play search end audio
        void PlaySearchEndAudio()
        {
            if (!playAudioOnSearchEnd) {
                return;
            }


            if (blaze.IsAudioScriptableEmpty()) {
                return;
            }


            blaze.PlayAudio(blaze.audioScriptable.GetAudio(AudioScriptable.AudioType.SearchEnd));
        }


        // exit the search and distracted state
        void EndSearchExit()
        {
            blaze.animManager.Play(endSearchAnim, searchAnimsT);
            PlaySearchEndAudio();

            _timeToCheck += Time.deltaTime;

            if (_timeToCheck >= endSearchAnimTime) {
                ResetDistraction();
                ResetStayIdle();

                blaze.SetState(blaze.previousState);
            }
        }

        #endregion
    }
}
