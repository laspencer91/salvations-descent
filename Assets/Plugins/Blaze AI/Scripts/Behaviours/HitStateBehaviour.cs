using System.Collections.Generic;
using UnityEngine;

namespace BlazeAISpace
{
    [AddComponentMenu("Hit State Behaviour/Hit State Behaviour")]
    public class HitStateBehaviour : MonoBehaviour
    {   
        [Header("HIT PROPERTIES"), Tooltip("Hit animation names and their durations. One will be chosen at random on every hit.")]
        public List<HitData> hitAnims;
        [Min(0), Tooltip("The animation transition from current animation to the hit animation.")]
        public float hitAnimT = 0.2f;
        [Min(0), Tooltip("The gap time between replaying the hit animations to avoid having the animation play on every single hit which may look bad on very fast and repitive attacks such as a machine gun.")]
        public float hitAnimGap = 0.3f;
        

        [Header("KNOCK OUT"), Min(0.1f), Tooltip("The duration in seconds to stay knocked out before getting up.")]
        public float knockOutDuration = 3;
        [Tooltip("The actual animation clip name of getting up from knock out if face up (lying on back).")]
        public string faceUpStandClipName;
        [Tooltip("The actual animation clip name of getting up from knock out when face down (facing ground).")]
        public string faceDownStandClipName;
        [Min(0), Tooltip("Ragdoll To Stand Speed: The transition speed from the ragdoll to the getting up animation.")]
        public float ragdollToStandSpeed = 0.3f;
        [Tooltip("Set the hip/pelvis bone of the AI so getting up after knock out is accurate.")]
        public Transform hipBone;

        [Header("KNOCK OUT FORCE"), Tooltip("If enabled, on knock out the ragdoll will use the natural velocity of the rigidbody at that moment in time with no additional force.")]
        public bool useNaturalVelocity;
        [Tooltip("If you don't want to use the natural velocity, you can add your own. This can also be changed dynamically through code before calling KnockOut() to add force to the ragdoll depending on the type of hit/weapon.")]
        public Vector3 knockOutForce;


        [Header("CANCEL ATTACK"), Tooltip("If set to true will cancel the attack if got hit or knocked out.")]
        public bool cancelAttackOnHit;
        
        
        [Header("AUDIO"), Tooltip("Play audio when hit. Set your audios in the audio scriptable in the General Tab in Blaze AI.")]
        public bool playAudio;
        [Tooltip("If enabled, a hit audio will always play when hit. If false, there's a 50/50 chance whether an audio will be played or not.")]
        public bool alwaysPlayAudio = true;

        
        [Header("CALL OTHERS"), Min(0), Tooltip("The radius to call other AIs when hit. You use this by calling blaze.Hit(player, true).")]
        public float callOthersRadius = 5;
        [Tooltip("The layers of the agents to call. You use this by calling blaze.Hit(player, true).")]
        public LayerMask agentLayersToCall;
        [Tooltip("Shows the call radius as a cyan wire sphere in the scene view.")]
        public bool showCallRadius;


        #region BEHAVIOUR VARIABLES

        BlazeAI blaze;

        bool playedAudio;

        float _duration = 0;
        float _gapTimer = 0;
        float hitDuration = 0;
        float getUpTimer = 0;

        [System.Serializable]
        public struct HitData 
        {
            [Tooltip("Set the animation name of the hit.")]
            public string animName;
            [Tooltip("Set the duration of the hit state for this animation.")]
            public float duration;
        }

        public enum RagdollState
        {
            None,
            Ragdoll,
            StandingUp,
            ResettingBones
        }

        public RagdollState ragdollState = RagdollState.None;

        class BoneTransform
        {
            public Vector3 Position { get; set; }
            public Quaternion Rotation { get; set; }
        }

        BoneTransform[] standUpBoneTransforms;
        BoneTransform[] ragdollBoneTransforms;
        Transform[] bones;
        Rigidbody hipBoneRb;

        float elapsedResetBonesTime;
        float posY;
        float facePos;
        float standingUpTimer = 0;
        bool getUpYonBul;
        string currGetUpAnim;
        int lastRegisteredKnockOuts = 0;

        #endregion
        
        #region UNITY METHODS

        void Start()
        {
            blaze = GetComponent<BlazeAI>();  

            // force shut if not the same state
            if (blaze.state != BlazeAI.State.hit) {
                enabled = false;
            }      
        }

        void OnDisable()
        {
            ResetTimers();
            blaze.hitEnemy = null;
            lastRegisteredKnockOuts = 0;
        }

        void OnEnable()
        {
            if (blaze == null) {
                blaze = GetComponent<BlazeAI>();  
            }
            
            // enable ragdoll if knock out is registered
            if (blaze.knockOutRegister != lastRegisteredKnockOuts) {
                if (hipBone != null) {
                    hipBoneRb = hipBone.GetComponent<Rigidbody>();  
                }

                if (bones == null || bones.Length <= 0) {
                    PrepareBones();
                }
                
                TriggerRagdoll();
            }
        }

        // show the call others radius
        void OnDrawGizmosSelected() 
        {
            if (!showCallRadius) {
                return;
            }

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, callOthersRadius);
        }
        
        void Update()
        {
            // cancel attack on hit
            if (cancelAttackOnHit) {
                blaze.StopAttack();
            }

            
            if (blaze.knockOutRegister != lastRegisteredKnockOuts) {
                OnEnable();
                return;
            }


            switch (ragdollState) 
            {
                case RagdollState.None:
                    HitBehaviour();
                    break;

                case RagdollState.Ragdoll:
                    RagdollBehaviour();
                    break;

                case RagdollState.StandingUp:
                    StandingUpBehaviour();
                    break;

                case RagdollState.ResettingBones:
                    ResettingBonesBehaviour();
                    break;
            }
        }

        #endregion

        #region BEHAVIOUR METHODS

        void HitBehaviour()
        {
            // check if a hit was registered
            if (blaze.hitRegistered) 
            {
                blaze.hitRegistered = false;
                int chosenHitIndex = -1;

                if (hitAnims.Count > 0) 
                {
                    chosenHitIndex = Random.Range(0, hitAnims.Count);
                    hitDuration = hitAnims[chosenHitIndex].duration;
                }
                else {
                    Debug.LogWarning("No hit animations added.");
                }
                

                if (_duration == 0) {
                    if (chosenHitIndex > -1) blaze.animManager.Play(hitAnims[chosenHitIndex].animName, hitAnimT, true);
                }
                else 
                {
                    if (_gapTimer >= hitAnimGap) 
                    {
                        if (chosenHitIndex > -1) blaze.animManager.Play(hitAnims[chosenHitIndex].animName, hitAnimT, true);
                        _gapTimer = 0;
                    }
                }
                
                _duration = 0;

                
                // call others
                if (blaze.callOthersOnHit) {
                    CallOthers();
                }

                // play hit audio
                PlayAudio();
            }


            _gapTimer += Time.deltaTime;


            // hit duration timer
            _duration += Time.deltaTime;

            if (_duration >= hitDuration) {
                FinishHitState();
            }
        }

        // exit hit state and turn to either alert or attack state
        void FinishHitState()   
        {
            ResetTimers();
            ragdollState = RagdollState.None;
            
            // if AI was in cover -> return to cover state
            if (blaze.hitWhileInCover && blaze.coverShooterMode) {
                blaze.SetState(BlazeAI.State.goingToCover);
                return;
            }


            if (blaze.isFleeing) {
                blaze.SetState(BlazeAI.State.attack);
                return;
            }


            // if the enemy that did the hit is passed -> set AI to go to enemy location
            if (blaze.hitEnemy) {
                // check the passed enemy isn't the same AI
                if (blaze.hitEnemy.transform.IsChildOf(transform)) {
                    blaze.ChangeState("alert");
                    return;
                }
                
                blaze.SetEnemy(blaze.hitEnemy);
                return;
            }

            
            // if an enemy is already targeted -> go to attack state
            if (blaze.enemyToAttack) {
                blaze.SetState(BlazeAI.State.attack);
                return;
            }


            // if nothing -> turn alert
            blaze.ChangeState("alert");
        }

        // play the hit audio
        void PlayAudio()
        {
            if (!playAudio) {
                return;
            }
            
            if (playedAudio) {
                return;
            }

            if (blaze.IsAudioScriptableEmpty()) {
                return;
            }

            if (!alwaysPlayAudio) {
                int rand = Random.Range(0, 2);
                if (rand == 0) {
                    return;
                }
            }

            if (blaze.PlayAudio(blaze.audioScriptable.GetAudio(AudioScriptable.AudioType.Hit))) {
                playedAudio = true;
            }
        }

        // call others
        void CallOthers()
        {
            Collider[] agentsColl = new Collider[5];
            int agentsCollNum = Physics.OverlapSphereNonAlloc(transform.position + blaze.centerPosition, callOthersRadius, agentsColl, agentLayersToCall);
        
            for (int i=0; i<agentsCollNum; i++) {
                BlazeAI script = agentsColl[i].GetComponent<BlazeAI>();

                // if caught collider is that of the same AI -> skip
                if (transform.IsChildOf(agentsColl[i].transform)) {
                    continue;
                }


                // if the caught collider is actually the current AI's enemy (AI vs AI) -> skip
                if (blaze.enemyToAttack != null) {
                    if (blaze.enemyToAttack.transform.IsChildOf(agentsColl[i].transform)) {
                        continue;
                    }
                }

                
                // if script doesn't exist -> skip
                if (script == null) {
                    continue;
                }


                // reaching this point means current item is a valid AI

                if (blaze.hitEnemy) {
                    script.SetEnemy(blaze.hitEnemy, true, true);
                    continue;
                }
                

                // if no enemy has been passed
                // make it a random point within the destination
                Vector3 randomPoint = script.RandomSpherePoint(transform.position);
                
                script.ChangeState("alert");
                script.MoveToLocation(randomPoint);
            }
        }

        // reset the timers of hit duration
        void ResetTimers()
        {
            _duration = 0;
            _gapTimer = 0;
            getUpTimer = 0;
            elapsedResetBonesTime = 0;
            standingUpTimer = 0;
            playedAudio = false;
        }

        #endregion
    
        #region RAGDOLL

        void TriggerRagdoll()
        {
            lastRegisteredKnockOuts = blaze.knockOutRegister;
            posY = transform.position.y;

            elapsedResetBonesTime = 0;
            _duration = 0;
            standingUpTimer = 0;
            
            blaze.animManager.ResetLastState();
            PlayAudio();
            
            if (useNaturalVelocity) {
                blaze.EnableRagdoll();
            }
            else 
            {
                blaze.EnableRagdoll();

                if (hipBoneRb != null) {
                    Vector3 dir = transform.TransformDirection(knockOutForce);
                    hipBoneRb.AddForce(dir, ForceMode.Impulse);
                }
            }

            if (blaze.callOthersOnHit) {
                CallOthers();
            }
            
            ragdollState = RagdollState.Ragdoll;
        }

        void RagdollBehaviour()
        {
            if (!IsRagdollSleeping()) {
                return;
            }

            _duration += Time.deltaTime;

            if (_duration < knockOutDuration) {
                return;
            }

            AlignRotationToHips();
            AlignPositionToHips();
            PopulateBoneTransforms(ragdollBoneTransforms);

            elapsedResetBonesTime = 0;
            _duration = 0;
            ragdollState = RagdollState.ResettingBones;
        }

        bool IsRagdollSleeping()
        {
            List<Collider> colls = blaze.GetRagdollColliders();
            int sleepingRB = 0;

            foreach (Collider c in colls) {
                if (Mathf.Abs(c.attachedRigidbody.velocity.x) <= 0.3f && Mathf.Abs(c.attachedRigidbody.velocity.y) <= 0.3f && Mathf.Abs(c.attachedRigidbody.velocity.z) <= 0.3f) {
                    sleepingRB++;
                }
            }

            if (colls.Count > 1) {
                // if half of the rigidbodies are rested then consider ragdoll has finished moving
                if (sleepingRB >= colls.Count / 2) {
                    return true;
                }

                return false;
            }
            
            if (colls.Count == sleepingRB) {
                return true;
            }

            return false;
        }
        
        void StandingUpBehaviour()
        {
            standingUpTimer += Time.deltaTime;
            if (standingUpTimer >= blaze.anim.GetCurrentAnimatorStateInfo(0).length) {
                standingUpTimer = 0;
                FinishHitState();
            }
        }

        void ResettingBonesBehaviour()
        {   
            getUpYonBul = true;
            
            if (getUpYonBul)
            {
                facePos = Vector3.Dot(hipBone.forward, Vector3.up);

                if (facePos > 0) {
                    currGetUpAnim = faceUpStandClipName;
                }
                else {
                    currGetUpAnim = faceDownStandClipName;
                }

                PopulateAnimationStartBoneTransforms(currGetUpAnim, standUpBoneTransforms);
                getUpYonBul = false;
            }
            
            elapsedResetBonesTime += Time.deltaTime;
            float elapsedPercentage = elapsedResetBonesTime / ragdollToStandSpeed;
            
            
            for (int boneIndex = 0; boneIndex < bones.Length; boneIndex++)
            {
                bones[boneIndex].localPosition = Vector3.Lerp(
                    ragdollBoneTransforms[boneIndex].Position,
                    standUpBoneTransforms[boneIndex].Position,
                    elapsedPercentage
                );

                bones[boneIndex].localRotation = Quaternion.Lerp(
                    ragdollBoneTransforms[boneIndex].Rotation,
                    standUpBoneTransforms[boneIndex].Rotation,
                    elapsedPercentage
                );
            }

            transform.position = new Vector3(transform.position.x, Mathf.Lerp(transform.position.y, posY, elapsedPercentage), transform.position.z);

            if (elapsedPercentage >= 1)
            {
                blaze.DisableRagdoll();
                blaze.anim.Play(currGetUpAnim, 0, 0);
                transform.position = new Vector3(transform.position.x, posY, transform.position.z);
                ragdollState = RagdollState.StandingUp;
            }
        }

        void AlignRotationToHips()
        {
            facePos = Vector3.Dot(hipBone.forward, Vector3.up);

            Vector3 originalHipsPosition = hipBone.position;
            Quaternion originalHipsRotation = hipBone.rotation;
            Vector3 desiredDirection = Vector3.zero;

            if (facePos > 0)
            {
                desiredDirection = hipBone.up * -1;
                desiredDirection.y = 0;

                desiredDirection.Normalize();

                Quaternion fromToRotation = Quaternion.FromToRotation(transform.forward, desiredDirection);
                transform.rotation *= fromToRotation;
            }

            hipBone.position = originalHipsPosition;
            hipBone.rotation = originalHipsRotation;
        }

        void AlignPositionToHips()
        {
            Vector3 originalHipsPosition = hipBone.position;
            transform.position = hipBone.position;

            Vector3 positionOffset = standUpBoneTransforms[0].Position;
            positionOffset.y = 0;
            positionOffset = transform.rotation * positionOffset;

            transform.position -= positionOffset;

            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hitInfo, blaze.groundLayers))
            {
                transform.position = new Vector3(transform.position.x, hitInfo.point.y, transform.position.z);
            }

            hipBone.position = originalHipsPosition;
        }

        void PopulateBoneTransforms(BoneTransform[] boneTransforms)
        {
            for (int boneIndex = 0; boneIndex < bones.Length; boneIndex++)
            {
                boneTransforms[boneIndex].Position = bones[boneIndex].localPosition;
                boneTransforms[boneIndex].Rotation = bones[boneIndex].localRotation;
            }
        }

        void PopulateAnimationStartBoneTransforms(string clipName, BoneTransform[] boneTransforms)
        {
            Vector3 positionBeforeSampling = transform.position;
            Quaternion rotationBeforeSampling = transform.rotation;

            foreach (AnimationClip clip in blaze.anim.runtimeAnimatorController.animationClips)
            {
                if (clip.name == clipName)
                {
                    clip.SampleAnimation(gameObject, 0);
                    PopulateBoneTransforms(standUpBoneTransforms);
                    break;
                }
            }

            transform.position = positionBeforeSampling;
            transform.rotation = rotationBeforeSampling;
        }

        void PrepareBones()
        {
            bones = blaze.GetRagdollTransforms();
            standUpBoneTransforms = new BoneTransform[bones.Length];
            ragdollBoneTransforms = new BoneTransform[bones.Length];

            for (int boneIndex = 0; boneIndex < bones.Length; boneIndex++) {
                standUpBoneTransforms[boneIndex] = new BoneTransform();
                ragdollBoneTransforms[boneIndex] = new BoneTransform();
            }
        }
        
        #endregion
    }
}
