using UnityEngine;

namespace BlazeAISpace
{
    [AddComponentMenu("Surprised State Behaviour/Surprised State Behaviour")]
    public class SurprisedStateBehaviour : MonoBehaviour
    {
        [Tooltip("The surprised animation to play.")]
        public string anim;
        [Tooltip("The animation transition.")]
        public float animT = 0.25f;
        [Tooltip("The duration to stay in this state and playing the animation.")]
        public float duration;

        [Tooltip("Set your audios in the audio scriptable in the General Tab in Blaze AI.")]
        public bool playAudio;

        BlazeAI blaze;
        float _duration = 0f;

        bool playedAudio;
        bool turningDone;

        
        void Start()
        {
            blaze = GetComponent<BlazeAI>();


            // force shut if not the same state
            if (blaze.state != BlazeAI.State.surprised) {
                enabled = false;
            }
        }


        void OnDisable()
        {
            Reset();
        }
        

        void Update()
        {
            // only turn if turning hasn't finished
            if (!turningDone) {
                // turn to face enemy -> this function returns true when done
                if (blaze.TurnTo(blaze.enemyPosOnSurprised, blaze.waypoints.leftTurnAnimAlert, blaze.waypoints.rightTurnAnimAlert, blaze.waypoints.turningAnimT, 20)) {
                    turningDone = true;
                }

                return;
            }
            
            
            // play animation
            blaze.animManager.Play(anim, animT);
            
            
            // play audio
            if (playAudio && !playedAudio) {
                if (!blaze.IsAudioScriptableEmpty()) {
                    if (blaze.PlayAudio(blaze.audioScriptable.GetAudio(AudioScriptable.AudioType.SurprisedState))) {
                        playedAudio = true;
                    }
                }
            }
            

            // timer to quit surprised state
            _duration += Time.deltaTime;
            if (_duration >= duration) {
                Reset();
                blaze.SetState(BlazeAI.State.attack);
            }
        }


        void Reset()
        {
            turningDone = false;
            _duration = 0f;
            playedAudio = false;
        }
    }
}

