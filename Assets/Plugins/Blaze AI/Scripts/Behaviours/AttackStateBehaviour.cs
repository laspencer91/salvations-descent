using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace BlazeAISpace
{
    [AddComponentMenu("Attack State Behaviour/Attack State Behaviour")]
    public class AttackStateBehaviour : MonoBehaviour
    {
        [Min(0), Tooltip("Won't be considered if root motion is used.")]
        public float moveSpeed = 5f;
        [Min(0)] public float turnSpeed = 5f;
        public string idleAnim;
        public string moveAnim;
        [Min(0)] public float idleMoveT = 0.25f;


        [Tooltip("If enabled, will adapt the distancing calculations to suit that of a ranged AI. So for example: if the target is unreachable but within attack distance then the AI will attack.")]
        public bool ranged;
        [Min(0), Tooltip("The idle distance between the AI and the target waiting to attack. If distance between the AI and the target is more than this value, the AI will chase the player to close the distance.")]
        public float distanceFromEnemy = 5f;
        [Min(0), Tooltip("The distance between AI and target when actually attacking. For example: if this is a ranged AI you want this value to be far since ranged enemies attack from a distance but if melee then this should be close.")]
        public float attackDistance = 1f;
        [Tooltip("Will check if any of these layers exist before attacking and if so will refrain from attacking until none of these layers exist. You can use this to avoid friendly fire with other AI agents.")]
        public LayerMask layersCheckOnAttacking;
        [Tooltip("Add your attacks. One will be chosen at random on each attack. If only one is set then that one will be launched every time.")]
        public Attacks[] attacks;
        public UnityEvent attackEvent;


        [Tooltip("If enabled, the AI will attack every certain interval and not wait to be called by the manager.")]
        public bool attackInIntervals;
        [Min(0), Tooltip("The amount of time to pass before attacking. A value will be randomized between the two inputs. For a constant value set the two inputs to the same value.")]
        public Vector2 attackInIntervalsTime = new Vector2(1, 4);


        [Tooltip("The AIs have the ability to call other AIs for help when they see the target. Enabling this will call other agents to the location. If disabled, no AIs will be called.")]
        public bool callOthers;
        [Tooltip("The radius of the call.")]
        [Min(0)] public float callRadius;
        [Tooltip("Select the layers of the Blaze AI agents you want to call.")]
        public LayerMask agentLayersToCall;
        [Tooltip("The call runs once every certain time. Here you can specifiy the amount of time (seconds) to pass everytime before calling other agents.")]
        public float callOthersTime;
        [Tooltip("Show the call radius as a white sphere in the scene view.")]
        public bool showCallRadius;
        [Tooltip("If enabled, this AI will be called by other agents if they are in attack state. If disabled, this AI won't be called.")]
        public bool receiveCallFromOthers;


        [Tooltip("If enabled, the AI will back away if the target moves in too close.")]
        public bool moveBackwards;
        [Min(0), Tooltip("If the distance between the target and the AI is less than this the AI will back away.")]
        public float moveBackwardsDistance = 5f;
        [Min(0), Tooltip("Won't be considered if root motion is used.")]
        public float moveBackwardsSpeed = 3f;
        public string moveBackwardsAnim;
        public float moveBackwardsAnimT = 0.25f;
        

        [Tooltip("If enabled, will turn to face the target if need be when waiting to attack. Will use the turn speed found in Blaze AI > Waypoints > Turn Speed.")]
        public bool turnToTarget = true;
        [Tooltip("If dot product between the AI and it's target is less or equal to this value then turning will occur.")]
        [Range(0, 1)] public float turnSensitivity = 0.7f;
        [Tooltip("Use the alert turning animations found in Blaze AI > Waypoints class.")]
        public bool useTurnAnims;


        [Tooltip("Will enable the AI to strafe in either direction when waiting to attack target.")]
        public bool strafe;
        public StrafeDirection strafeDirection = StrafeDirection.LeftAndRight;
        [Min(0), Tooltip("Won't be considered if root motion is used.")]
        public float strafeSpeed = 3f;
        [Min(-1), Tooltip("The amount of time to strafe for. A value will be randomized between the two inputs. For a constant value, set both inputs to the same value. For infinity and never stop strafing set both values to -1.")]
        public Vector2 strafeTime = new Vector2(3, 5);
        [Min(0), Tooltip("The amount of time to wait before strafing again.")]
        public Vector2 strafeWaitTime = new Vector2(0.3f, 1);
        [Tooltip("Left strafe animation name.")]
        public string leftStrafeAnim;
        [Tooltip("Right strafe animation name.")]
        public string rightStrafeAnim;
        [Tooltip("Transition time from current animation to the strafing animation.")]
        public float strafeAnimT = 0.25f;
        [Tooltip("Set all the layers you want to avoid when strafing that includes other Blaze AI agents.")]
        public LayerMask strafeLayersToAvoid;


        [Min(0), Tooltip("The speed to rotate to the enemy when reaching attack position.")]
        public float onAttackRotateSpeed = 10f;
        [Min(0), Tooltip("Keep rotating/facing enemy while attacking. Good for ranged enemies when you always want them to be looking at the target.")]
        public bool onAttackRotate;


        [Tooltip("When the AI moves to player location in attack state like in Hit or in a chase and finds no enemy the AI will search the radius.")]
        public bool searchLocationRadius;
        [Tooltip("The amount of time to pass in seconds before starting the search.")]
        public float timeToStartSearch = 2;
        [Range(1, 10), Tooltip("The number of points to randomly search.")]
        public int searchPoints = 3;
        [Tooltip("The animation name to play when reaching the search point.")]
        public string searchPointAnim;
        [Tooltip("The amount of time to wait in each search point.")]
        public float pointWaitTime = 3;
        [Tooltip("The animation to play when searching has finished.")]
        public string endSearchAnim;
        [Min(0), Tooltip("The amount of time (seconds) the animation should play for.")]
        public float endSearchAnimTime = 3;
        public float searchAnimsT = 0.25f;

        public bool playAudioOnSearchStart;
        public bool playAudioOnSearchEnd;


        [Tooltip("When the AI is in attack state and there's no hostile at the end location, this animation will play and after the animation time passes the AI will return to alert patrolling. Only works if search empty location is disabled.")]
        public string returnPatrolAnim;
        public float returnPatrolAnimT = 0.25f;
        [Min(0), Tooltip("The duration of the animation after target disappearance to return to alert patrolling.")]
        public float returnPatrolTime = 3f;
        public bool playAudioOnReturnPatrol;


        [Tooltip("Play a random audio from the audio scriptable when AI is waiting for it's turn to attack target. Set the audios from the audio scriptable in Blaze AI > General tab.")]
        public bool playAttackIdleAudio;
        [Min(0), Tooltip("The amount of time to pass (seconds) to play audio. A random number will be generated between the two set values. For a constant time, set the two properties to the same value.")]
        public Vector2 attackIdleAudioTime = new Vector2(3, 10);


        [Tooltip("Play a random audio when AI is chasing target. Set it in the audio scriptable.")]
        public bool playAudioOnChase;
        [Tooltip("If set to true, on chase the AI will always play an audio on chase. If false, there will be a 50/50 chance whether a chase audio will be played or not.")]
        public bool alwaysPlayOnChase;


        [Tooltip("Play a random audio when AI is moving to attack. Set audio from audio scriptable in the Move To Attack array.")]
        public bool playAudioOnMoveToAttack;
        [Tooltip("If set to true, on moving to attack the AI will always play an audio on chase. If false, there will be a 50/50 chance whether a chase audio will be played or not.")]
        public bool alwaysPlayOnMoveToAttack;
        

        public enum StrafeDirection 
        {
            Left,
            Right,
            LeftAndRight
        }

        [System.Serializable]
        public struct Attacks 
        {
            public string attackAnim;
            [Min(0.1f)] public float attackDuration;
            public float animT;
            public bool useAudio;
            public int audioIndex;
        }


        #region BEHAVIOUR VARS

        BlazeAI blaze;
        BlazeAIEnemyManager enemyManager;
        AlertStateBehaviour alertStateBehaviour;
        public int chosenAttackIndex { get; private set; }

        int checkPathElapsed = 0;
        int checkPathFrames = 3;
        int strafeDir = 0;
        int strafeCheckPathElapsed = 0;
        int agentPriority;
        int searchIndex = 0;
        
        bool idle;
        bool startAttackTimer;
        bool startAttackInIntervals;
        bool targetChangedTag;
        bool isStrafing;
        bool isStrafeWait;
        bool turningToTarget;
        bool returnedToAlert;
        bool calculatedLastEnemyPos { get; set; }
        bool isSearching;
        bool returnPatrolAudioPlayed;
        bool isReachable;
        bool attackIdleAudioPlayed;
        bool shouldPlayChaseAudio;
        bool chaseAudioPlayed;
        bool chaseAudioIsReady;
        bool moveToAttackAudioPlayed;
        bool isAddedToManagerInAttacks;
        
        float attackDuration;
        float _attackDuration;
        float _intervalAttackTime;
        float intervalAttackTime;
        float _callOthers;
        float _timeToReturnAlert;
        float _strafeTime;
        float _strafeWaitTime;
        float _strafingTimer;
        float _strafeWaitTimer;
        float searchTimeElapsed;
        float attackIdleAudTimer = 0f;
        float chosenAttackIdleAudTime = -1;
        float collSize = 0f;

        Vector3 targetPosOnAttack;
        Vector3 lastEnemyPos;
        Vector3 searchLocation;
        Vector3 closestPointToTarget;
        Vector3 enemyPoint = Vector3.zero;
        Transform previousEnemy;
        AudioClip lastPlayedChaseAudio;
        
        #endregion

        #region UNITY METHODS
        
        void Start()
        {
            blaze = GetComponent<BlazeAI>();
            alertStateBehaviour = GetComponent<AlertStateBehaviour>();
            agentPriority = blaze.navmeshAgent.avoidancePriority;

            ValidateDistance();
            ReadyChaseAudio();

            // force shut if not the same state
            if (blaze.state != BlazeAI.State.attack) {
                enabled = false;
            }
        }

        void OnDrawGizmosSelected() 
        {
            // show the call radius
            if (callOthers && showCallRadius) {
                Gizmos.color = Color.white;
                Gizmos.DrawWireSphere(transform.position, callRadius);
            }

            // fire a red ray to target
            if (blaze) {
                if (blaze.enemyToAttack) {
                    Vector3 dir = blaze.enemyColPoint - (transform.position + blaze.centerPosition);
                    Debug.DrawRay(transform.position + blaze.centerPosition, dir, Color.red, 0.1f);
                }
            }
        }

        void OnValidate()
        {   
            if (distanceFromEnemy < moveBackwardsDistance) {
                moveBackwardsDistance = distanceFromEnemy - 0.5f;
            }

            if (moveBackwardsDistance >= distanceFromEnemy) {
                moveBackwardsDistance = distanceFromEnemy - 0.5f;
            }

            if (blaze == null) {
                blaze = GetComponent<BlazeAI>();
            }

            if (blaze == null) return;

            ValidateDistance();
        }

        void OnDisable()
        {
            if (blaze != null) {
                blaze.navmeshAgent.avoidancePriority = agentPriority;
            }
            
            previousEnemy = null;
            calculatedLastEnemyPos = false;
            returnPatrolAudioPlayed = false;
            
            ResetSearching();
            ResetAttackIdleAudio();
            ResetChaseAudio();
            ResetMoveToAttackAudio();
        }

        void OnEnable()
        {
            if (blaze == null) {
                Start();
            }
            
            checkPathElapsed = checkPathFrames;
            IdlePosition();
            ReadyChaseAudio();
        }

        void Update()
        {
            // exit state if AI turned friendly
            if (blaze.friendly) {
                NoTarget();
                return;
            }
            

            // track the attack timer
            AttackTimer();
            
            // if interval attacks are enabled
            IntervalAttackTimer();

            // if calling others is enabled
            CallOthers();

            // force the strafing flags to false if strafe is not enabled
            ValidateFlags();

            
            // if target exists
            if (blaze.enemyToAttack) {
                calculatedLastEnemyPos = false;
                ResetSearching();
                
                // if currently turning to face target
                if (turningToTarget) {
                    IdlePosition();
                    return;
                }                
                
                Engage(blaze.enemyToAttack.transform);

                if (strafe) {
                    if (isStrafing) {
                        Strafe();
                    }
                    else {
                        StrafeWaitTimer();
                    }
                }

                return;
            }

            
            // REACHING THIS POINT MEANS ENEMY DOESN'T EXIST -> GOT OUT OF VISION

            
            StopStrafe();


            // if mid-attack -> quit
            if (startAttackTimer) {
                return;
            }


            // if mid turn -> continue turning and quit
            if (turningToTarget) {
                IdlePosition();
                return;
            }

            
            // check if another agent called this one
            if (blaze.checkEnemyPosition != Vector3.zero) {
                ResetSearching();
                GoToLocation(blaze.checkEnemyPosition);
                return;
            }


            // search within radius of empty location
            if (isSearching) {
                if (blaze.MoveTo(searchLocation, alertStateBehaviour.moveSpeed, alertStateBehaviour.turnSpeed, alertStateBehaviour.moveAnim)) {
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

            
            // if target changed tag to something non-hostile
            if (targetChangedTag) {
                NoTarget();
                return;
            }


            // make a randomized point from the target to avoid the AIs stopping so close to each other
            if (!calculatedLastEnemyPos) {
                calculatedLastEnemyPos = true;
                lastEnemyPos = blaze.RandomSpherePoint(blaze.enemyColPoint, blaze.navmeshAgent.height + 1);
            }
            

            // if target goes out of vision -> go to last location
            if (lastEnemyPos == Vector3.zero) {
                GoToLocation(blaze.enemyColPoint);
                return;
            }

            
            GoToLocation(lastEnemyPos);
        }

        #endregion

        #region ATTACK
        
        // engage the target
        void Engage(Transform target)
        {
            _timeToReturnAlert = 0;

            // check target tag hasn't changed to something non-hostile
            if (System.Array.IndexOf(blaze.vision.hostileTags, target.tag) < 0) {
                targetChangedTag = true;
                return;
            }
            
            targetChangedTag = false;
            returnedToAlert = false;

            AddManagerAndSetInterval(target);
            

            // check if enemy reachable every 5 frames
            if (checkPathElapsed >= checkPathFrames) 
            {
                checkPathElapsed = 0;
                CheckHeightAndReachable();
            }

            checkPathElapsed++;


            // blaze keeps track of distance to the current targeted enemy using sqr magnitude in this property
            float distance = blaze.distanceToEnemySqrMag;
            float[] setDistances = SetDistances();
            float minDistance = setDistances[0];
            float backupDist = setDistances[1];

            if (!isReachable) 
            {
                NotReachableBehaviour(target, distance, minDistance, backupDist);
                return;
            }
            
            ReachableBehaviour(target, distance, minDistance, backupDist);
        }

        // idle position -> waiting to attack
        void IdlePosition()
        {
            idle = true;

            // play audios
            PlayAttackIdleAudio();
            ReadyChaseAudio();

            // check if turn to target is enabled
            if (turnToTarget) {
                Vector3 enemyPos = new Vector3(blaze.enemyColPoint.x, transform.position.y, blaze.enemyColPoint.z);
                Vector3 toOther = (enemyPos - transform.position).normalized;
                float dotProd = Vector3.Dot(toOther, transform.forward);

                // if AI is currently turning to target
                if (turningToTarget) {
                    // TurnTo() turns the AI and returns true when dot == 0.97f and false otherwise
                    if (!blaze.TurnTo(enemyPos, blaze.waypoints.leftTurnAnimAlert, blaze.waypoints.rightTurnAnimAlert, blaze.waypoints.turningAnimT, 7, useTurnAnims)) {
                        return;
                    }

                    turningToTarget = false;
                }
                else {
                    // if turning should be done
                    if (dotProd <= turnSensitivity) {
                        turningToTarget = true;
                        return;
                    }
                }
            }

            // trigger strafing or idle anim
            if (strafe) {
                TriggerStrafe();
                return;
            }

            blaze.animManager.Play(idleAnim, idleMoveT);
        }

        // move the AI to target position
        void MoveToTarget(Vector3 pos, bool runIdle=false)
        {
            if (startAttackTimer) return;

            if (blaze.MoveTo(pos, moveSpeed, turnSpeed, moveAnim, idleMoveT)) {
                if (runIdle) {
                    IdlePosition();
                    return;
                }
            }

            idle = false;
            isStrafing = false;

            if (!blaze.isAttacking) {
                PlayChaseAudio();
                return;
            }
            
            PlayMoveToAttackAudio();
        }

        // launch attack
        void Attack()
        {
            if (attacks.Length <= 0) {
                blaze.isAttacking = false;
                return;
            }

            if (startAttackTimer) return;


            Vector3 startDir = transform.position + blaze.centerPosition;
            Vector3 targetDir = blaze.enemyColPoint - (transform.position + blaze.centerPosition);

            int layers = layersCheckOnAttacking | blaze.vision.hostileAndAlertLayers;
            float rayDistance = Vector3.Distance(blaze.enemyColPoint, transform.position + blaze.centerPosition) + 5;

            // check if there's a layer between AI and target before attack
            if (!blaze.CheckTargetVisibleWithRay(blaze.enemyToAttack.transform, startDir, targetDir, rayDistance, layers)) {
                IdlePosition();
                return;
            }


            // choose a random attack
            int index = Random.Range(0, attacks.Length);
            // public property to check attack index 
            chosenAttackIndex = index;

            // play attack anim
            blaze.animManager.Play(attacks[index].attackAnim, attacks[index].animT);

            // trigger event
            attackEvent.Invoke();
            
            // play attack audio if exists
            if (attacks[index].useAudio) {
                if (!blaze.IsAudioScriptableEmpty()) {
                    blaze.PlayAudio(blaze.audioScriptable.GetAudio(AudioScriptable.AudioType.Attacks, attacks[index].audioIndex));
                }
            }

            // save the target position on the attack frame to rotate to
            targetPosOnAttack = blaze.enemyToAttack.transform.position;

            // these vars are used in AttackTimer() method
            attackDuration = attacks[index].attackDuration;
            startAttackTimer = true;

            idle = false;
        }

        // set the min distance for the AI according to conditions
        float[] SetDistances()
        {
            float[] arr = new float[2];
            float minDistance = 0f;
            float backupDistTemp = 0f;


            // set the min distance according to attack and/or enemy manager call            
            if (enemyManager.callEnemies) 
            {
                if (blaze.isAttacking) {
                    minDistance = attackDistance;
                    backupDistTemp = 0f;

                    arr[0] = minDistance;
                    arr[1] = backupDistTemp;
                    
                    return arr;
                }
            }
            else {
                blaze.StopAttack();
            }


            if (isStrafing) {
                minDistance = distanceFromEnemy + 1;
            }
            else {
                minDistance = distanceFromEnemy;
            }


            if (moveBackwards) {
                backupDistTemp = moveBackwardsDistance;
            }

        
            arr[0] = minDistance;
            arr[1] = backupDistTemp;

            return arr;
        }

        // the attack duration timer
        void AttackTimer()
        {
            if (!blaze.isAttacking) {
                startAttackTimer = false;
                _attackDuration = 0;
            }


            if (startAttackTimer) {
                // set priority to the highest to avoid being pushed by other agents while attacking
                blaze.navmeshAgent.avoidancePriority = 0;


                // rotate to target
                if (onAttackRotate) {
                    blaze.RotateTo(blaze.enemyColPoint, onAttackRotateSpeed);
                }
                else {
                    if (_attackDuration <= 0.1) {
                        blaze.RotateTo(targetPosOnAttack, onAttackRotateSpeed);
                    }
                }

                
                // attack timer
                _attackDuration += Time.deltaTime;

                if (_attackDuration >= attackDuration) {
                    StopAttack();
                }
            }
            else {
                // return to the original priority level after attack
                blaze.navmeshAgent.avoidancePriority = agentPriority;
            }
        }

        // time before attacking (if attack in Intervals mode is enabled)
        void IntervalAttackTimer()
        {
            if (startAttackInIntervals) {
                _intervalAttackTime += Time.deltaTime;

                if (_intervalAttackTime >= intervalAttackTime) {
                    blaze.Attack();
                    PlayMoveToAttackAudio();
                    startAttackInIntervals = false;
                    _intervalAttackTime = 0f;
                }
            }
        }

        // add enemy manager to target
        void AddEnemyManager(Transform currentEnemy, bool addToManager = true)
        {
            if (currentEnemy == null) {
                return;
            }

            if (currentEnemy == previousEnemy && addToManager == isAddedToManagerInAttacks) {
                return;
            }


            enemyManager = currentEnemy.GetComponent<BlazeAIEnemyManager>();
            
            if (enemyManager == null) {
                enemyManager = currentEnemy.gameObject.AddComponent(typeof(BlazeAIEnemyManager)) as BlazeAIEnemyManager;
            }

            
            if (addToManager) {
                enemyManager.AddEnemy(blaze);
            }

            
            previousEnemy = currentEnemy;
            isAddedToManagerInAttacks = addToManager;
        }

        void StopAttack()
        {
            blaze.isAttacking = false;
            startAttackTimer = false;
            _attackDuration = 0f;
            ResetMoveToAttackAudio();
        }

        void AddManagerAndSetInterval(Transform target)
        {
            if (!attackInIntervals) {
                AddEnemyManager(target);
                return;
            }
            
            AddEnemyManager(target, false);
    
            if (startAttackInIntervals) {
                return;
            }

            if (blaze.isAttacking) {
                return;
            }
            
            intervalAttackTime = Random.Range(attackInIntervalsTime.x, attackInIntervalsTime.y);
            startAttackInIntervals = true;
        }

        void CheckHeightAndReachable()
        {
            RaycastHit heightHit;
            Collider enemyColl = GetCollider(blaze.enemyToAttack);

            if (Physics.Raycast(enemyColl.bounds.min, -Vector3.up, out heightHit, Mathf.Infinity, blaze.groundLayers)) 
            {
                if (heightHit.distance >= blaze.navmeshAgent.height) {
                    isReachable = false;
                    return;
                }
                
                isReachable = blaze.IsPathReachable(blaze.enemyColPoint);
                return;
            }

            // if didn't catch anything from the previous operation -> (from the bottom) -> it could mean the target's min point is clipping the ground
            if (Physics.Raycast(blaze.enemyColPoint, -Vector3.up, out heightHit, Mathf.Infinity, blaze.groundLayers)) 
            {   
                isReachable = blaze.IsPathReachable(blaze.enemyColPoint);
                return;
            }
            
            isReachable = blaze.IsPointOnNavMesh(enemyColl.bounds.min, 0.3f);

            if (ranged) {
                enemyPoint = blaze.ValidateYPoint(blaze.enemyColPoint);
            }
        }

        void ReachableBehaviour(Transform target, float distance, float minDistance, float backupDist)
        {
            // check distance isn't too close
            if (distance + 0.3f > (backupDist * backupDist)) 
            {
                // if distance is bigger than min distance -> move
                if (distance > (minDistance * minDistance)) 
                {
                    // if already in idle position don't move if distance difference is 2f or less to avoid bad frozen movement 
                    if (!blaze.isAttacking && idle) 
                    {
                        float tempDist = distance - (minDistance * minDistance);
                        
                        if (tempDist <= minDistance + 2f) {
                            IdlePosition();
                            return;
                        }
                    }

                    MoveToTarget(blaze.enemyColPoint);
                    return;
                }
                
                // if reached min distance and is attacking true -> launch attack
                if (blaze.isAttacking) 
                {
                    Attack();
                    return;
                }

                IdlePosition();
                return;
            }
            
            // reaching this point means target is too close and should move backwards
            MoveBackwards(blaze.enemyColPoint);
        }

        void NotReachableBehaviour(Transform target, float distance, float minDistance, float backupDist)
        {
            if (startAttackTimer) {
                return;
            }
            
            if (ranged) 
            {
                if (distance > (backupDist * backupDist)) 
                {
                    if (distance > (minDistance * minDistance)) 
                    {
                        if (blaze.ClosestNavMeshPoint(enemyPoint, collSize, out closestPointToTarget)) 
                        {
                            MoveToTarget(closestPointToTarget, true);
                            return;
                        }

                        IdlePosition();
                        return;
                    }

                    if (blaze.isAttacking) 
                    {
                        Attack();
                        return;
                    }

                    IdlePosition();
                    return;
                }

                MoveBackwards(blaze.enemyColPoint);
                return;
            }


            blaze.StopAttack();                
            
            if (distance + 0.3f > (backupDist * backupDist)) 
            {
                IdlePosition();
                return;
            }

            MoveBackwards(blaze.enemyColPoint);
            return;
        }

        #endregion
        
        #region CALL OTHERS
        
        // call nearby agents to target location
        void CallOthers()
        {
            // if call others isn't enabled or no target
            if (!callOthers || !blaze.enemyToAttack) {
                return;
            }


            // time to pass before firing
            _callOthers += Time.deltaTime;
            if (_callOthers < callOthersTime) {
                return;
            }

            _callOthers = 0;


            Collider[] callOthersColl = new Collider[20];
            int callOthersNum = Physics.OverlapSphereNonAlloc(transform.position, callRadius, callOthersColl, agentLayersToCall);

            for (int i=0; i<callOthersNum; i+=1) 
            {
                BlazeAI script = callOthersColl[i].GetComponent<BlazeAI>();
                AttackStateBehaviour attackBehaviour = callOthersColl[i].GetComponent<AttackStateBehaviour>();
                CoverShooterBehaviour coverShooterBehaviour = callOthersColl[i].GetComponent<CoverShooterBehaviour>();


                if (callOthersColl[i].transform.IsChildOf(blaze.enemyToAttack.transform)) {
                    continue;
                }

                // if current item is the AI itself -> skip
                if (callOthersColl[i].transform.IsChildOf(transform)) {
                    continue;
                }


                // if doesn't have Blaze AI
                if (!script) {
                    continue;
                }


                // if doesn't have this AttackStateBehaviour script
                if (!attackBehaviour && !coverShooterBehaviour) {
                    continue;
                }


                // check if doesn't receive calls
                if (attackBehaviour) {
                    if (!attackBehaviour.receiveCallFromOthers) {
                        continue;
                    }
                }
                else {
                    if (!coverShooterBehaviour.receiveCallFromOthers) {
                        continue;
                    }
                }


                // if agent already has a target then don't call
                if (script.enemyToAttack) {
                    continue;
                }


                // if other agent has seen the target after this agent then don't call
                if (script.captureEnemyTimeStamp > blaze.captureEnemyTimeStamp) {
                    continue;
                }

                
                // set the check enemy position of the other agents to target position
                script.checkEnemyPosition = script.RandomSpherePoint(blaze.enemyColPoint);


                // turn the agents to attack state
                script.TurnToAttackState();
            }
        }

        #endregion
    
        #region GETTING CALLED BY OTHERS
        
        void GoToLocation(Vector3 location)
        {
            // moves AI to location and returns true when reaches location
            if (blaze.MoveTo(location, moveSpeed, turnSpeed, moveAnim, idleMoveT)) {
                NoTarget();
            }
            
            idle = false;
            PlayChaseAudio();
        }
        
        #endregion

        #region RETURN PATROL/ALERT && SEARCHING
        
        // reached location and no hostile found
        void NoTarget()
        {
            if (blaze.companionMode) {
                ReturnToAlert();
            }

            // prevent chase audio from playing
            if (playAudioOnChase) {
                ResetChaseAudio();
            }

            // search empty location
            if (searchLocationRadius) {
                blaze.animManager.Play(idleAnim, returnPatrolAnimT);
                searchTimeElapsed += Time.deltaTime;

                if (searchTimeElapsed >= timeToStartSearch) {
                    PlaySearchStartAudio();
                    SetSearchPoint();
                    
                    isSearching = true;
                    return;
                }

                return;
            }

            if (!returnedToAlert) {
                blaze.SetState(BlazeAI.State.returningToAlert);
            }

            returnedToAlert = true;
            turningToTarget = false;

            PlayReturnPatrolAudio();
            ReturnToAlertIdle();

            _timeToReturnAlert += Time.deltaTime;
            if (_timeToReturnAlert >= returnPatrolTime) {
                ReturnToAlert();
            }
        }
        
        // play return animation
        void ReturnToAlertIdle()
        {
            if (returnPatrolAnim.Length == 0) {
                if (alertStateBehaviour.idleAnim.Length > 0) {
                    blaze.animManager.Play(alertStateBehaviour.idleAnim[0], returnPatrolAnimT);
                }
            }
            else {
                blaze.animManager.Play(returnPatrolAnim, returnPatrolAnimT);
            }

            
            idle = true;
            ResetEnemyManager();
        }
        
        // exit attack state and return to alert
        void ReturnToAlert()
        {
            _timeToReturnAlert = 0;
            blaze.SetState(BlazeAI.State.alert);
        }

        // play start search audio
        void PlayReturnPatrolAudio()
        {
            // if audio already played -> return
            if (returnPatrolAudioPlayed) {
                return;
            }


            if (!playAudioOnReturnPatrol) {
                return;
            }


            if (blaze.IsAudioScriptableEmpty()) {
                return;
            }


            blaze.PlayAudio(blaze.audioScriptable.GetAudio(AudioScriptable.AudioType.ReturnPatrol));
            returnPatrolAudioPlayed = true;
        }

        // set the next search point
        void SetSearchPoint()
        {
            searchLocation = blaze.RandomSpherePoint(transform.position, (blaze.navmeshAgent.height * 2) + 2);
            
            // make sure never returns 0
            if (searchLocation == Vector3.zero) {
                SetSearchPoint();
                return;
            }
            
            searchIndex++;
            searchTimeElapsed = 0;
        }

        // returns whether the idle time has finished in the search point or not
        bool IsSearchPointIdleFinished()
        {
            blaze.animManager.Play(searchPointAnim, searchAnimsT);

            searchTimeElapsed += Time.deltaTime;

            if (searchTimeElapsed >= pointWaitTime) {
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

            searchTimeElapsed += Time.deltaTime;

            if (searchTimeElapsed >= endSearchAnimTime) {
                idle = true;
                ResetEnemyManager();
                ReturnToAlert();
                blaze.SetState(BlazeAI.State.alert);
            }
        }

        void ResetSearching()
        {
            searchIndex = 0;
            isSearching = false;
            searchTimeElapsed = 0f;
        }
        
        #endregion
    
        #region STRAFING

        // this gets called first in IdlePosition() to trigger the strafe
        void TriggerStrafe()
        {   
            // quit if already waiting or strafing
            if (isStrafeWait || isStrafing) {
                return;
            } 

            
            // strafe direction
            if (strafeDirection == StrafeDirection.Left) {
                strafeDir = 0;
            }
            else if (strafeDirection == StrafeDirection.Right) {
                strafeDir = 1;
            }
            else {
                strafeDir = Random.Range(0, 2);
            }


            // set strafe time
            if (strafeTime.x == -1 && strafeTime.y == -1) {
                _strafeTime = Mathf.Infinity;
            }
            else {
                _strafeTime = Random.Range(strafeTime.x, strafeTime.y);
            }


            // set strafe wait time and start wait timer
            _strafeWaitTime = Random.Range(strafeWaitTime.x, strafeWaitTime.y);

            // strafe wait timer used in StrafeWaitTimer() 
            isStrafeWait = true;
        }
        
        // check if can strafe move and trigger the movement 
        void Strafe()
        {
            if (!idle) {
                StopStrafe();
                return;
            }

            StrafeMovement(strafeDir);
        }

        // the actual strafing movement
        void StrafeMovement(int direction)
        {   
            RaycastHit hit;
            int layersToHit = blaze.vision.hostileAndAlertLayers | strafeLayersToAvoid | blaze.vision.layersToDetect;
            
            Vector3 strafePoint = Vector3.zero;
            Vector3 offsetPlayer;
            Vector3 transformDir;
            Vector3 offset;

            string strafeAnim;
            string moveDir;


            // if direction is left
            if (direction == 0) {
                offsetPlayer = blaze.enemyColPoint - transform.position;
                offsetPlayer = Vector3.Cross(offsetPlayer, Vector3.up);

                strafePoint = blaze.ValidateYPoint(offsetPlayer);
                strafePoint = transform.position + new Vector3(strafePoint.x, 0, strafePoint.z + 0.5f);
                strafePoint = strafePoint + (transform.right * (blaze.distanceToEnemy - 1));

                transformDir = -transform.right;

                // to check from an offset if enemy will not be visible
                offset = transform.TransformPoint(new Vector3(-(blaze.navmeshAgent.radius + 0.7f), 0f, 0f) + blaze.centerPosition);

                // set the anim and move dir
                strafeAnim = leftStrafeAnim;
                moveDir = "left";
            }
            else {
                offsetPlayer = transform.position - blaze.enemyColPoint;
                offsetPlayer = Vector3.Cross(offsetPlayer, Vector3.up);

                strafePoint = blaze.ValidateYPoint(offsetPlayer);
                strafePoint = transform.position + new Vector3(strafePoint.x, 0, strafePoint.z + 0.5f);
                strafePoint = strafePoint + (-transform.right * (blaze.distanceToEnemy - 1));

                transformDir = transform.right;

                // to check from an offset if enemy will not be visible
                offset = transform.TransformPoint(new Vector3((blaze.navmeshAgent.radius + 0.7f), 0f, 0f) + blaze.centerPosition);
                
                // set the anim and move dir
                strafeAnim = rightStrafeAnim;
                moveDir = "right";
            }
            
            
            // check if point reachable and has navmesh every 5 frames
            if (strafeCheckPathElapsed >= 5) {
                strafeCheckPathElapsed = 0;

                // if agent isn't on navmesh
                if (!blaze.IsPointOnNavMesh(blaze.ValidateYPoint(transform.position), blaze.navmeshAgent.radius)) {
                    StopStrafe();
                    return;
                }
                
                Vector3 goToPoint = blaze.ValidateYPoint((transform.position) + (transformDir * (blaze.navmeshAgent.radius * 2 + (blaze.navmeshAgent.height / 2) )));
                if (!blaze.IsPathReachable(goToPoint)) {
                    StopOrChangeStrafeDirection();
                    return;
                }
            }

            strafeCheckPathElapsed++;


            // check if there's an obstacle in strafe direction
            RaycastHit[] hitResults = new RaycastHit[7];
            List<RaycastHit> hitList = new List<RaycastHit>();
            int hits = Physics.SphereCastNonAlloc(transform.position + blaze.centerPosition, 0.15f, transformDir, hitResults, (blaze.navmeshAgent.radius * 2) + blaze.navmeshAgent.height/2, layersToHit);
        
            // filter
            for (int i=0; i<hits; i++) {
                // if sphere cast hits the same AI collider -> ignore
                if (transform.IsChildOf(hitResults[i].transform) || hitResults[i].transform.IsChildOf(transform)) {
                    continue;
                }

                if (hitResults[i].distance == 0 || hitResults[i].point == Vector3.zero) {
                    continue;
                }

                hitList.Add(hitResults[i]);
            }

            if (hitList.Count > 0) {
                StopOrChangeStrafeDirection();
                return;
            }
            
            
            layersToHit = blaze.vision.hostileAndAlertLayers | blaze.vision.layersToDetect;
            
            // to check from an offset if enemy will not be visible
            if (!blaze.CheckTargetVisibleWithRay(blaze.enemyToAttack.transform, offset, blaze.enemyColPoint - offset, Vector3.Distance(blaze.enemyColPoint, transform.position) + 5, layersToHit)) {
                StopOrChangeStrafeDirection();
                return;
            }
            
        
            isStrafing = true;
            blaze.RotateTo(blaze.enemyToAttack.transform.position, 20);
            blaze.MoveTo(strafePoint, strafeSpeed, 0, strafeAnim, strafeAnimT, moveDir);
        
            
            _strafingTimer += Time.deltaTime;
            if (_strafingTimer >= _strafeTime) {
                StopStrafe();
            }
        }

        void StopStrafe()
        {
            isStrafing = false;
            _strafingTimer = 0f;
        }

        // wait to strafe
        void StrafeWaitTimer()
        {
            if (!idle) {
                isStrafeWait = false;
                _strafeWaitTimer = 0f;
                return;
            }


            if (isStrafeWait) {
                blaze.animManager.Play(idleAnim, idleMoveT);  
                _strafeWaitTimer += Time.deltaTime;
                
                if (_strafeWaitTimer >= _strafeWaitTime) {
                    _strafeWaitTimer = 0f;

                    // in Update() if isStrafing fires Strafe()
                    isStrafing = true;
                    isStrafeWait = false;
                }
            }
        }

        void StopOrChangeStrafeDirection()
        {
            if (strafeDirection == StrafeDirection.LeftAndRight) {
                if (strafeDir == 0) strafeDir = 1;
                else strafeDir = 0;

                blaze.animManager.Play(idleAnim, idleMoveT);
            }
            else{
                StopStrafe();
            }
        }
        
        #endregion
    
        #region MOVE BACKWARDS
        
        // back away from target
        void MoveBackwards(Vector3 target)
        {
            Vector3 targetPosition = transform.position - (transform.forward * (blaze.navmeshAgent.height - 0.25f));
            Vector3 backupPoint = blaze.ValidateYPoint(targetPosition);
            RaycastHit hit;

            backupPoint = new Vector3 (backupPoint.x, transform.position.y, backupPoint.z + 0.5f);
            int layers = blaze.vision.layersToDetect | alertStateBehaviour.obstacleLayers | agentLayersToCall;


            // check if obstacle is behind
            if (Physics.Raycast(transform.position + blaze.centerPosition, -transform.forward, out hit, blaze.navmeshAgent.radius * 2 + 0.3f, layers)) {
                IdlePosition();
                return;
            }

            
            // if point isn't reachable
            if (!blaze.IsPathReachable(backupPoint)) {
                IdlePosition();
                return;
            }

            
            // if strafing we need further precision to check if moving backwards is possible
            // to prevent disabling strafing to find the AI moves backwards a very neglibile distance which makes it look very bad
            if (isStrafing) 
            {
                if (Physics.Raycast(transform.position + blaze.centerPosition, -transform.forward, out hit, moveBackwardsDistance / 1.5f + blaze.navmeshAgent.radius, layers)) {
                    IdlePosition();
                    return;
                }
                else {
                    Vector3 checkPoint = new Vector3(backupPoint.x, transform.position.y, backupPoint.z + (moveBackwardsDistance / 2f));

                    if (!blaze.IsPathReachable(checkPoint)) {
                        IdlePosition();
                        return;
                    }
                }
            }
            

            // back away
            blaze.RotateTo(target, turnSpeed);
            blaze.MoveTo(backupPoint, moveBackwardsSpeed, 0f, moveBackwardsAnim, moveBackwardsAnimT, "backwards");

            
            // cancel strafing when backing away
            idle = false;
            isStrafing = false;
            isStrafeWait = false;
        }
        
        #endregion
    
        #region MISC

        // force the strafing flags to false if strafe is not enabled
        void ValidateFlags()
        {
            if (!strafe) {
                isStrafing = false;
                isStrafeWait = false;
            }
        }

        void ValidateDistance()
        {
            if (blaze.vision == null) return;

            if (moveBackwardsDistance >= blaze.vision.visionDuringAttackState.sightRange) 
            {
                if (blaze.vision.visionDuringAttackState.sightRange - 3 <= 0) 
                {
                    moveBackwardsDistance = 0;
                    Debug.Log("Move Backwards Distance can't be bigger or equal to Sight Range in Vision During Attack State. Has been reset to 0.");
                    return;
                }
                
                moveBackwardsDistance = blaze.vision.visionDuringAttackState.sightRange - 3;
                Debug.Log("Move Backwards Distance can't be bigger or equal to Sight Range in Vision During Attack State.");
            }
        }

        void ResetEnemyManager()
        {
            if (enemyManager) {
                previousEnemy = null;
                enemyManager.RemoveEnemy(blaze);
            }
        }

        Collider GetCollider(GameObject go)
        {
            return go.GetComponent<Collider>();
        }

        #endregion

        #region AUDIOS

        // play audios when AI is waiting for it's turn to attack
        void PlayAttackIdleAudio()
        {
            if (!playAttackIdleAudio || blaze.IsAudioScriptableEmpty()) {
                return;
            }


            // when the attack idle audio finishes -> reset
            if (attackIdleAudioPlayed && !blaze.agentAudio.isPlaying) {
                ResetAttackIdleAudio();
                return;
            }


            // if audio is playing -> return
            if (attackIdleAudioPlayed) {
                return;
            }


            // generate a random time for when passed -> audio is played
            if (chosenAttackIdleAudTime == -1) {
                chosenAttackIdleAudTime = Random.Range(attackIdleAudioTime.x, attackIdleAudioTime.y);
            }


            // timer to pass before playing audio
            attackIdleAudTimer += Time.deltaTime;
            if (attackIdleAudTimer < chosenAttackIdleAudTime) {
                return;
            }


            // make the blaze audio play a random audio and returns true if passed audio is valid and is playing
            if (blaze.PlayAudio(blaze.audioScriptable.GetAudio(AudioScriptable.AudioType.AttackIdle))) {
                attackIdleAudioPlayed = true;
                return;
            }


            // reaching this means the passed audio clip to play is null 
            ResetAttackIdleAudio();
        }

        // reset all the timers and flags of playing attack idle audio
        void ResetAttackIdleAudio()
        {
            attackIdleAudTimer = 0;
            chosenAttackIdleAudTime = -1;
            attackIdleAudioPlayed = false;
        }

        // ready the audio to play on chase
        void ReadyChaseAudio()
        {
            if (blaze == null) return;


            if (blaze.IsAudioScriptableEmpty() || !playAudioOnChase) {
                shouldPlayChaseAudio = false;
                chaseAudioPlayed = false;
                return;
            }

            
            if (chaseAudioIsReady) {
                return;
            }


            chaseAudioPlayed = false;
            chaseAudioIsReady = true;


            if (alwaysPlayOnChase) {
                shouldPlayChaseAudio = true;
                return;
            }


            // randomize chance whether to play or not
            int rand = Random.Range(0, 2);
            
            if (rand == 0) {
                shouldPlayChaseAudio = false;
                return;
            }

            shouldPlayChaseAudio = true;
        }

        // play the chosen chase audio
        void PlayChaseAudio()
        {
            if (blaze.IsAudioScriptableEmpty() || !shouldPlayChaseAudio) {
                chaseAudioIsReady = false;
                return;
            }


            if (chaseAudioPlayed) {
                return;
            }


            lastPlayedChaseAudio = blaze.audioScriptable.GetAudio(AudioScriptable.AudioType.Chase);

            if (blaze.PlayAudio(lastPlayedChaseAudio)) {
                chaseAudioPlayed = true;
            }

            chaseAudioIsReady = false;
        }

        // reset all chase audio flags
        void ResetChaseAudio()
        {
            chaseAudioIsReady = false;
            chaseAudioPlayed = false;
            shouldPlayChaseAudio = false;
        }

        // play audio when moving to attack
        void PlayMoveToAttackAudio()
        {
            if (blaze.IsAudioScriptableEmpty() || !playAudioOnMoveToAttack || moveToAttackAudioPlayed) {
                return;
            }

            moveToAttackAudioPlayed = true;

            if (alwaysPlayOnMoveToAttack) {
                blaze.PlayAudio(blaze.audioScriptable.GetAudio(AudioScriptable.AudioType.MoveToAttack));
                return;
            }

            int rand = Random.Range(0, 2);
            if (rand == 0) {
                return;
            }

            blaze.PlayAudio(blaze.audioScriptable.GetAudio(AudioScriptable.AudioType.MoveToAttack));
        }

        void ResetMoveToAttackAudio()
        {
            moveToAttackAudioPlayed = false;
        }

        #endregion
    }
}
