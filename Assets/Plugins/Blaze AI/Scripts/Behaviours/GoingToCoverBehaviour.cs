using UnityEngine;
using UnityEngine.AI;

namespace BlazeAISpace
{
    [AddComponentMenu("Going To Cover Behaviour/Going To Cover Behaviour")]
    public class GoingToCoverBehaviour : MonoBehaviour
    {
        [Tooltip("Object layers that can take cover behind.")]
        public LayerMask coverLayers;
        [Range(-1f, 1f), Tooltip("The lower the number the better the hiding spot. From -1 (best) to 1 (worst)")]
        public float hideSensitivity = -0.25f;
        [Min(0), Tooltip("The search distance for cover. This can't be bigger than the [Distance From Enemy] property (automatically clamped if so)")]
        public float searchDistance = 25f;
        [Tooltip("Show search distance as a light blue sphere in scene view.")]
        public bool showSearchDistance;


        [Tooltip("The minimum height of cover obstacles to be eligible for search. Cover height is measured using collider.bounds.y. Use the GetCoverHeight script on any obstacle to get it's height.")]
        public float minCoverHeight = 1f;
        [Tooltip("If the chosen cover height is bigger or equals to this value then the high cover animation will play. If it's less than this then low cover animation will play.")]
        public float highCoverHeight = 3;
        [Tooltip("The animation name to play when in high cover. The cover is considered to be a high cover when it's height is bigger or equal to [High Cover Height] property. The height is calculated using collider.bounds.y. You can use the BlazeAI GetCoverHeight script to get the height of any obstacle.")]
        public string highCoverAnim;
        [Tooltip("The animation name to play when in low cover. The cover is considered to be a low cover when it's height is less than the [High Cover Height] property. The height is calculated using collider.bounds.y. You can use the BlazeAI GetCoverHeight script to get the height of any obstacle.")]
        public string lowCoverAnim;
        public float coverAnimT = 0.25f;


        [Tooltip("If set to true, the AI will rotate to match the cover normal. Meaning the back of the character will be on the cover. If set to false, will take cover in the same current rotation when reached the cover.")]
        public bool rotateToCoverNormal = true;
        public float rotateToCoverSpeed = 300f;
        [Tooltip("This will make the AI refrain from attacking and only do so after taking cover.")]
        public bool onlyAttackAfterCover;


        [Tooltip("If enabled, the AI will play an audio when going to cover. The audio is chosen randomly in the audio scriptable from the Going To Cover array.")]
        public bool playAudioOnGoingToCover;
        [Tooltip("If enabled, the AI will always play an audio when going to cover. If false, there's a 50/50 chance whether the AI will play an audio or not.")]
        public bool alwaysPlayAudio;
        

        #region BEHAVIOUR VARS

        BlazeAI blaze;
        CoverShooterBehaviour coverShooterBehaviour;

        public CoverProperties coverProps;
        public struct CoverProperties {
            public Transform cover;
            public Vector3 coverPoint;
            public float coverHeight;
            public BlazeAICoverManager coverManager;
        }

        public Transform lastCover { get; private set; }

        #endregion
        
        #region UNITY METHODS
        
        void Start()
        {
            blaze = GetComponent<BlazeAI>();    
            coverShooterBehaviour = GetComponent<CoverShooterBehaviour>();

            // force shut if not the same state
            if (blaze.state != BlazeAI.State.goingToCover) {
                enabled = false;
            }
        }

        void OnDrawGizmosSelected()
        {
            if (showSearchDistance) {
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireSphere(transform.position, searchDistance);
            }
        }

        void OnDisable()
        {
            RemoveCoverOccupation();
            blaze.ResetCenterPosition();
            blaze.tookCover = false;
        }

        void Update()
        {
            // if blaze is in friendly mode -> exit the state
            if (blaze.friendly) {
                blaze.SetState(BlazeAI.State.attack);
                return;
            }


            // call others that are not alerted yet and start the timer until attack
            coverShooterBehaviour.CallOthers();
            
            if (!onlyAttackAfterCover) {
                coverShooterBehaviour.TimeUntilAttack();
            }

            
            // if target exists
            if (blaze.enemyToAttack) {
                // if cover property is not null -> means AI has set a cover
                if (coverProps.cover) {
                    // save last cover
                    lastCover = coverProps.cover;

                    // moves agent to point and returns true when reaches destination
                    if (blaze.MoveTo(coverProps.coverPoint, coverShooterBehaviour.moveSpeed, coverShooterBehaviour.turnSpeed, coverShooterBehaviour.moveAnim, coverShooterBehaviour.idleMoveT)) {
                        TakeCover();
                    }
                    else {
                        blaze.ResetCenterPosition();
                    }

                    return;
                }
            
                
                if (blaze.hitWhileInCover) {
                    FindCover(lastCover);
                    return;
                }


                FindCover();
                return;
            }
            

            // if there's no target return to cover shooter behaviour
            blaze.SetState(BlazeAI.State.attack);
        }  

        #endregion

        #region COVER METHODS

        // search for cover
        public void FindCover(Transform coverToAvoid = null)
        {   
            blaze.hitWhileInCover = false;
            blaze.tookCover = false;

            Collider[] coverColls = new Collider[20];
            int hits = Physics.OverlapSphereNonAlloc(transform.position, searchDistance, coverColls, coverLayers);
            int hitReduction = 0;
            Transform playerCover = coverShooterBehaviour.GetTargetCover();


            // eliminate bad cover options
            for (int i=0; i<hits; i++) {
                float distance = Vector3.Distance(blaze.ValidateYPoint(coverColls[i].transform.position), blaze.enemyColPoint);

                if (distance + 2 >= coverShooterBehaviour.distanceFromEnemy || distance > blaze.vision.visionDuringAttackState.sightRange ||
                    coverColls[i].bounds.size.y < minCoverHeight || coverColls[i].transform == coverToAvoid) {
                    
                    coverColls[i] = null;
                    hitReduction++;

                    continue;
                }
                
                // calculate the change cover frequency -> don't take same cover twice in a row
                if (hits >= 2 && coverToAvoid == null) {
                    if (CalculateChangeCoverFrequency() && lastCover != null) {
                        if (lastCover.IsChildOf(coverColls[i].transform)) {
                            coverColls[i] = null;
                            hitReduction++;
                            continue;
                        }
                    }
                }
                

                // if player is hiding behind cover -> remove that cover as being eligible
                if (playerCover != null) {
                    if (playerCover.IsChildOf(coverColls[i].transform)) {
                        coverColls[i] = null;
                        hitReduction++;
                        continue;
                    }
                }


                // check if other agents are already occupying/moving to the same cover by reading the cover manager component
                BlazeAICoverManager coverManager = coverColls[i].transform.GetComponent<BlazeAICoverManager>();


                // if cover manager doesn't exist -> continue
                if (!coverManager) {
                    continue;
                }


                // cover manager exists and not occupied -> continue
                if (coverManager.occupiedBy == null || coverManager.occupiedBy == transform) {
                    continue;
                }


                // reaching this far means cover manager exists and is occupied -> so remove as a potential cover
                coverColls[i] = null;
                hitReduction++;
            }


            hits -= hitReduction;
            System.Array.Sort(coverColls, ColliderArraySortComparer);


            // if no covers found
            if (hits <= 0) {
                NoCoversFound();
                return;
            }


            NavMeshHit hit = new NavMeshHit();
            NavMeshHit hit2 = new NavMeshHit();
            NavMeshHit closestEdge = new NavMeshHit();
            NavMeshHit closestEdge2 = new NavMeshHit();

            
            // if found obstacles
            for (int i = 0; i < hits; i++) {
                Vector3 boundSize = coverColls[i].GetComponent<Collider>().bounds.size;
                float passedCenterPosH = -1;
                
                if (coverColls[i].bounds.size.y - 0.2f <= blaze.defaultCenterPos.y) {
                    passedCenterPosH = blaze.defaultCenterPos.y/2;
                }
                else {
                    passedCenterPosH = blaze.defaultCenterPos.y;
                }

                
                if (NavMesh.SamplePosition(coverColls[i].transform.position, out hit, boundSize.x + boundSize.z, NavMesh.AllAreas)) {
                    if (!NavMesh.FindClosestEdge(hit.position, out closestEdge, NavMesh.AllAreas)) {
                        continue;
                    }


                    if (Vector3.Dot(closestEdge.normal, (blaze.enemyColPoint - closestEdge.position).normalized) < hideSensitivity) {
                        if (!blaze.IsPathReachable(closestEdge.position)) {
                            continue;
                        }

                        if (coverShooterBehaviour.CheckIfTargetSeenFromPoint(closestEdge.position, true, passedCenterPosH)) {
                            continue;
                        }

                        ChooseCover(closestEdge, coverColls[i]);
                        return;
                    }
                    else {
                        // Since the previous spot wasn't facing "away" enough from the target, we'll try on the other side of the object
                        if (NavMesh.SamplePosition(coverColls[i].transform.position - (blaze.enemyColPoint - hit.position).normalized * 2, out hit2, boundSize.x + boundSize.z, NavMesh.AllAreas)) {
                            if (!NavMesh.FindClosestEdge(hit2.position, out closestEdge2, NavMesh.AllAreas)) {
                                continue;
                            }

                            if (Vector3.Dot(closestEdge2.normal, (blaze.enemyColPoint - closestEdge2.position).normalized) < hideSensitivity) {
                                if (!blaze.IsPathReachable(closestEdge2.position)) {
                                    continue;
                                }

                                if (coverShooterBehaviour.CheckIfTargetSeenFromPoint(closestEdge2.position, true, passedCenterPosH)) {
                                    continue;
                                }

                                ChooseCover(closestEdge2, coverColls[i]);
                                return;
                            }
                        }
                        else {
                            continue;
                        }
                    }
                }
                else {
                    continue;
                }
            }


            // if reached this point then no cover was found
            NoCoversFound();
        }

        // choose and save the passed cover
        void ChooseCover(NavMeshHit hit, Collider cover)
        {   
            BlazeAICoverManager coverMang = cover.transform.GetComponent<BlazeAICoverManager>();
            
            if (coverMang == null) {
                coverMang = cover.transform.gameObject.AddComponent<BlazeAICoverManager>() as BlazeAICoverManager;
            }
            
            coverMang.occupiedBy = transform;


            // save the cover properties
            coverProps.coverManager = coverMang;
            coverProps.cover = cover.transform;
            
            coverProps.coverPoint = hit.position;
            coverProps.coverHeight = cover.bounds.size.y;

            PlayGoingToCoverAudio();
        }

        public void RemoveCoverOccupation()
        {
            // set the current cover to null
            coverProps.cover = null;

            // if doesn't have cover manager -> return
            if (!coverProps.coverManager) {
                return;
            }

            // if cover manager shows that cover isn't occupied -> return
            if (coverProps.coverManager.occupiedBy == null) {
                return;
            }

            // if cover manager shows that cover is occupied by a different AI -> return
            if (coverProps.coverManager.occupiedBy != transform) {
                return;
            }

            // if reached this point means -> cover manager exists and this current AI occupies/occupied it
            coverProps.coverManager.occupiedBy = null;
        }

        // no covers have been found
        void NoCoversFound()
        {
            RemoveCoverOccupation();
            coverShooterBehaviour.FoundNoCover();
        }

        // taking cover
        void TakeCover()
        {
            // high cover
            if (coverProps.coverHeight >= highCoverHeight) {
                blaze.animManager.Play(highCoverAnim, coverAnimT);
            }

            // low cover
            if (coverProps.coverHeight < highCoverHeight) {
                blaze.animManager.Play(lowCoverAnim, coverAnimT);
            }


            LowerCenterPosition();
            RotateToCoverNormal();
            CheckCoverBlown();


            if (onlyAttackAfterCover) {
                coverShooterBehaviour.TimeUntilAttack();
            }
            

            blaze.tookCover = true;
        }

        // rotate to cover
        void RotateToCoverNormal()
        {
            if (!rotateToCoverNormal) {
                return;
            }

            RaycastHit hit;
            int layers = coverLayers;

            Vector3 dir = coverProps.cover.position - transform.position;
            Vector3 coverNormal = Vector3.zero;

            // get normal
            if (Physics.Raycast(transform.position, dir, out hit, Mathf.Infinity, layers)) {
                if (coverProps.cover.IsChildOf(hit.transform) || hit.transform.IsChildOf(coverProps.cover)) {
                    coverNormal = hit.normal;
                    coverNormal.z = 0;
                }
            }
            
            // if hasn't hit the correct cover yet -> return
            if (coverNormal == Vector3.zero) {
                return;
            }

            // rotate to normal
            Quaternion rot = Quaternion.FromToRotation(Vector3.forward, coverNormal);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, rotateToCoverSpeed * Time.deltaTime);
        }

        // check if target has compromised AI cover
        void CheckCoverBlown()
        {
            Vector3 startDir = transform.position + blaze.centerPosition;
            Vector3 targetDir = blaze.enemyColPoint - (transform.position + blaze.centerPosition);
            float rayDistance = Vector3.Distance(blaze.enemyColPoint, transform.position + blaze.centerPosition) + 5;
            
            if (blaze.CheckTargetVisibleWithRay(blaze.enemyToAttack.transform, startDir, targetDir, rayDistance, Physics.AllLayers))
            {
                if (coverShooterBehaviour.coverBlownDecision == CoverShooterBehaviour.CoverBlownDecision.AlwaysAttack) {
                    AttackFromCover();
                }
                else if (coverShooterBehaviour.coverBlownDecision == CoverShooterBehaviour.CoverBlownDecision.TakeCover) {
                    FindCover(coverProps.cover);
                }
                else {
                    int rand = Random.Range(0, 2);

                    if (rand == 0) {
                        FindCover(coverProps.cover);
                    }
                    else {
                        AttackFromCover();
                    }
                }

                RemoveCoverOccupation();
            }
        }

        void AttackFromCover()
        {
            blaze.Attack();
        }

        int ColliderArraySortComparer(Collider A, Collider B)
        {
            if (A == null && B != null)
            {
                return 1;
            }
            else if (A != null && B == null)
            {
                return -1;
            }
            else if (A == null && B == null)
            {
                return 0;
            }
            else
            {
                return Vector3.Distance(transform.position, A.transform.position).CompareTo(Vector3.Distance(transform.position, B.transform.position));
            }
        }

        // return true or false for whether to change last cover or not
        bool CalculateChangeCoverFrequency()
        {
            // if change cover frequency is less or equal to zero -> don't change cover
            if (coverShooterBehaviour.changeCoverFrequency <= 0) {
                return false;
            }


            // if change cover frequency is more or equal to 10 -> change cover
            if (coverShooterBehaviour.changeCoverFrequency >= 10) {
                return true;
            }


            // calculate the odds and chance of changing cover
            int odds = 10 - coverShooterBehaviour.changeCoverFrequency;
            int chanceOfChangingCover = Random.Range(1, 10);

            if (chanceOfChangingCover > odds) {
                return true;
            }


            return false;
        }

        void LowerCenterPosition()
        {
            if (blaze.centerPosition.y <= (coverProps.coverHeight - 0.2f)) return;

            if (coverProps.coverHeight - 0.4f <= 0) {
                blaze.centerPosition = new Vector3(blaze.defaultCenterPos.x, blaze.defaultCenterPos.y/2, blaze.defaultCenterPos.z);
                return;
            }

            blaze.centerPosition = new Vector3(blaze.defaultCenterPos.x, coverProps.coverHeight - 0.4f, blaze.defaultCenterPos.z);
        }

        #endregion

        #region AUDIO

        void PlayGoingToCoverAudio()
        {
            if (blaze.IsAudioScriptableEmpty() || !playAudioOnGoingToCover) {
                return;
            }

            
            if (alwaysPlayAudio) {
                blaze.PlayAudio(blaze.audioScriptable.GetAudio(AudioScriptable.AudioType.GoingToCover));
                return;
            }

            
            int rand = Random.Range(0, 2);
            if (rand == 0) {
                return;
            }


            blaze.PlayAudio(blaze.audioScriptable.GetAudio(AudioScriptable.AudioType.GoingToCover));
        }

        #endregion
    }
}
