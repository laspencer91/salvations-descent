using UnityEditor;

namespace BlazeAISpace
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(GoingToCoverBehaviour))]
    public class GoingToCoverBehaviourInspector : Editor
    {
        SerializedProperty coverLayers,
        hideSensitivity,
        searchDistance,
        showSearchDistance,

        minCoverHeight,
        highCoverHeight,
        highCoverAnim,
        lowCoverAnim,
        coverAnimT,

        rotateToCoverNormal,
        rotateToCoverSpeed,

        onlyAttackAfterCover,

        playAudioOnGoingToCover,
        alwaysPlayAudio;


        void OnEnable()
        {
            coverLayers = serializedObject.FindProperty("coverLayers");
            hideSensitivity = serializedObject.FindProperty("hideSensitivity");
            searchDistance = serializedObject.FindProperty("searchDistance");
            showSearchDistance = serializedObject.FindProperty("showSearchDistance");

            minCoverHeight = serializedObject.FindProperty("minCoverHeight");
            highCoverHeight = serializedObject.FindProperty("highCoverHeight");
            highCoverAnim = serializedObject.FindProperty("highCoverAnim");
            lowCoverAnim = serializedObject.FindProperty("lowCoverAnim");
            coverAnimT = serializedObject.FindProperty("coverAnimT");

            rotateToCoverNormal = serializedObject.FindProperty("rotateToCoverNormal");
            rotateToCoverSpeed = serializedObject.FindProperty("rotateToCoverSpeed");

            onlyAttackAfterCover = serializedObject.FindProperty("onlyAttackAfterCover");

            playAudioOnGoingToCover = serializedObject.FindProperty("playAudioOnGoingToCover");
            alwaysPlayAudio = serializedObject.FindProperty("alwaysPlayAudio");
        }


        public override void OnInspectorGUI () 
        {
            GoingToCoverBehaviour script = (GoingToCoverBehaviour) target;
            int spaceBetween = 15;


            EditorGUILayout.LabelField("GENERAL", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(coverLayers);
            EditorGUILayout.PropertyField(hideSensitivity);
            EditorGUILayout.PropertyField(searchDistance);
            EditorGUILayout.PropertyField(showSearchDistance);


            EditorGUILayout.Space(spaceBetween);
            EditorGUILayout.LabelField("COVERS HEIGHT", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(minCoverHeight);
            EditorGUILayout.PropertyField(highCoverHeight);
            EditorGUILayout.PropertyField(highCoverAnim);
            EditorGUILayout.PropertyField(lowCoverAnim);
            EditorGUILayout.PropertyField(coverAnimT);


            EditorGUILayout.Space(spaceBetween);
            EditorGUILayout.LabelField("ROTATE TO COVER", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(rotateToCoverNormal);
            if (script.rotateToCoverNormal) {
                EditorGUILayout.PropertyField(rotateToCoverSpeed);
            }


            EditorGUILayout.Space(spaceBetween);
            EditorGUILayout.LabelField("ATTACK AFTER COVER", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(onlyAttackAfterCover);


            EditorGUILayout.Space(spaceBetween);
            EditorGUILayout.LabelField("AUDIO", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(playAudioOnGoingToCover);
            if (script.playAudioOnGoingToCover) {
                EditorGUILayout.PropertyField(alwaysPlayAudio);
            }
            

            serializedObject.ApplyModifiedProperties();
        }
    }
}
