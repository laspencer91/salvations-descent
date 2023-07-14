using UnityEditor;

namespace BlazeAISpace
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(AlertTagBehaviour))]
    public class AlertTagBehaviourInspector : Editor
    {
        SerializedProperty checkLocation,
        onSightAnim,
        onSightDuration,
        reachedLocationAnim,
        reachedLocationDuration,
        animT,
        playAudio,
        audioIndex,
        callOtherAgents,
        callRange,
        showCallRange,
        otherAgentsLayers,
        randomizeCallPosition;


        void OnEnable()
        {
            checkLocation = serializedObject.FindProperty("checkLocation");
            onSightAnim = serializedObject.FindProperty("onSightAnim");
            onSightDuration = serializedObject.FindProperty("onSightDuration");
            reachedLocationAnim = serializedObject.FindProperty("reachedLocationAnim");
            reachedLocationDuration = serializedObject.FindProperty("reachedLocationDuration");
            animT = serializedObject.FindProperty("animT");
            playAudio = serializedObject.FindProperty("playAudio");
            audioIndex = serializedObject.FindProperty("audioIndex");
            callOtherAgents = serializedObject.FindProperty("callOtherAgents");
            callRange = serializedObject.FindProperty("callRange");
            showCallRange = serializedObject.FindProperty("showCallRange");
            otherAgentsLayers = serializedObject.FindProperty("otherAgentsLayers");
            randomizeCallPosition = serializedObject.FindProperty("randomizeCallPosition");
        }

        public override void OnInspectorGUI()
        {
            AlertTagBehaviour script = (AlertTagBehaviour) target;
            int spaceBetween = 20;

            EditorGUILayout.LabelField("CHECK LOCATION", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(checkLocation);

            EditorGUILayout.Space(spaceBetween);

            EditorGUILayout.LabelField("ANIMATIONS", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(onSightAnim);
            EditorGUILayout.PropertyField(onSightDuration);
            EditorGUILayout.Space(5);
            if (script.checkLocation) {
                EditorGUILayout.PropertyField(reachedLocationAnim);
                EditorGUILayout.PropertyField(reachedLocationDuration);
            }
            EditorGUILayout.PropertyField(animT);

            EditorGUILayout.Space(spaceBetween);

            EditorGUILayout.LabelField("AUDIO", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(playAudio);
            if (script.playAudio) {
                EditorGUILayout.PropertyField(audioIndex);
            }

            EditorGUILayout.Space(spaceBetween);

            EditorGUILayout.LabelField("CALL OTHERS", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(callOtherAgents);
            if (script.callOtherAgents) {
                EditorGUILayout.PropertyField(callRange);
                EditorGUILayout.PropertyField(showCallRange);
                EditorGUILayout.PropertyField(otherAgentsLayers);
                EditorGUILayout.PropertyField(randomizeCallPosition);
            }


            serializedObject.ApplyModifiedProperties();
        }
    }
}
