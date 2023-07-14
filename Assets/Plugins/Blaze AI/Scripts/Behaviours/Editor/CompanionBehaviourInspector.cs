using UnityEngine;
using UnityEditor;

namespace BlazeAISpace
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(CompanionBehaviour))]
    public class CompanionBehaviourInspector : Editor
    {
        SerializedProperty follow,
        followIfDistance,
        movingWindow,

        idleAnim,
        moveAnim,

        moveSpeed,
        turnSpeed,

        stayPutAnim,
        animsT,

        wanderAround,
        wanderPointTime,
        wanderMoveSpeed,
        wanderIdleAnims,
        wanderMoveAnim,

        moveAwayOnContact,
        layersToAvoid,

        playAudioForIdleAndWander,
        audioTimer,
        playAudioOnFollowState;


        void OnEnable()
        {
            follow = serializedObject.FindProperty("follow");
            followIfDistance = serializedObject.FindProperty("followIfDistance");
            movingWindow = serializedObject.FindProperty("movingWindow");

            idleAnim = serializedObject.FindProperty("idleAnim");
            moveAnim = serializedObject.FindProperty("moveAnim");

            moveSpeed = serializedObject.FindProperty("moveSpeed");
            turnSpeed = serializedObject.FindProperty("turnSpeed");

            stayPutAnim = serializedObject.FindProperty("stayPutAnim");
            animsT = serializedObject.FindProperty("animsT");

            wanderAround = serializedObject.FindProperty("wanderAround");
            wanderPointTime = serializedObject.FindProperty("wanderPointTime");
            wanderMoveSpeed = serializedObject.FindProperty("wanderMoveSpeed");
            wanderIdleAnims = serializedObject.FindProperty("wanderIdleAnims");
            wanderMoveAnim = serializedObject.FindProperty("wanderMoveAnim");

            moveAwayOnContact = serializedObject.FindProperty("moveAwayOnContact");
            layersToAvoid = serializedObject.FindProperty("layersToAvoid");

            playAudioForIdleAndWander = serializedObject.FindProperty("playAudioForIdleAndWander");
            audioTimer = serializedObject.FindProperty("audioTimer");
            playAudioOnFollowState = serializedObject.FindProperty("playAudioOnFollowState");
        }


        public override void OnInspectorGUI () 
        {
            CompanionBehaviour script = (CompanionBehaviour) target;
            int spaceBetween = 20;
            

            EditorGUILayout.LabelField("FOLLOW", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(follow);
            EditorGUILayout.PropertyField(followIfDistance);
            EditorGUILayout.PropertyField(movingWindow);

            EditorGUILayout.Space(spaceBetween);

            EditorGUILayout.LabelField("SPEEDS & ANIMS", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(idleAnim);
            EditorGUILayout.PropertyField(moveAnim);
            EditorGUILayout.Space(5);
            EditorGUILayout.PropertyField(moveSpeed);
            EditorGUILayout.PropertyField(turnSpeed);

            EditorGUILayout.Space(5);
            EditorGUILayout.PropertyField(stayPutAnim);
            EditorGUILayout.PropertyField(animsT);

            EditorGUILayout.Space(spaceBetween);

            EditorGUILayout.LabelField("WANDER AROUND", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(wanderAround);
            if (script.wanderAround) {
                EditorGUILayout.PropertyField(wanderPointTime);
                EditorGUILayout.PropertyField(wanderMoveSpeed);
                EditorGUILayout.PropertyField(wanderIdleAnims);
                EditorGUILayout.PropertyField(wanderMoveAnim);
            }

            EditorGUILayout.Space(spaceBetween);

            EditorGUILayout.LabelField("SKIN & CONTACT", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(moveAwayOnContact);
            if (script.moveAwayOnContact) {
                EditorGUILayout.PropertyField(layersToAvoid);
            }
            

            EditorGUILayout.Space(spaceBetween);

            EditorGUILayout.LabelField("AUDIOS", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(playAudioForIdleAndWander);
            if (script.playAudioForIdleAndWander) {
                EditorGUILayout.PropertyField(audioTimer);
            }
            EditorGUILayout.PropertyField(playAudioOnFollowState);


            serializedObject.ApplyModifiedProperties();
        } 
    }
}
