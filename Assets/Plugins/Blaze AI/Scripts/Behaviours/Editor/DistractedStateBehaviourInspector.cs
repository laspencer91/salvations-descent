using UnityEditor;

namespace BlazeAISpace
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(DistractedStateBehaviour))]

    public class DistractedStateBehaviourInspector : Editor
    {
        SerializedProperty timeToReact,

        checkLocation,
        timeBeforeMovingToLocation,
        checkAnim,
        checkAnimT,
        timeToCheck,
        randomizePoint,
        randomizeRadius,

        searchLocationRadius,
        searchPoints,
        searchPointAnim,
        pointWaitTime,
        endSearchAnim,
        endSearchAnimTime,
        searchAnimsT,

        playAudioOnCheckLocation,
        playAudioOnSearchStart,
        playAudioOnSearchEnd;


        void OnEnable()
        {
            timeToReact = serializedObject.FindProperty("timeToReact");
            
            checkLocation = serializedObject.FindProperty("checkLocation");
            timeBeforeMovingToLocation = serializedObject.FindProperty("timeBeforeMovingToLocation");
            checkAnim = serializedObject.FindProperty("checkAnim");
            checkAnimT = serializedObject.FindProperty("checkAnimT");
            timeToCheck = serializedObject.FindProperty("timeToCheck");
            randomizePoint = serializedObject.FindProperty("randomizePoint");
            randomizeRadius = serializedObject.FindProperty("randomizeRadius");

            searchLocationRadius = serializedObject.FindProperty("searchLocationRadius");
            searchPoints = serializedObject.FindProperty("searchPoints");
            searchPointAnim = serializedObject.FindProperty("searchPointAnim");
            pointWaitTime = serializedObject.FindProperty("pointWaitTime");
            endSearchAnim = serializedObject.FindProperty("endSearchAnim");
            endSearchAnimTime = serializedObject.FindProperty("endSearchAnimTime");
            searchAnimsT = serializedObject.FindProperty("searchAnimsT");

            playAudioOnCheckLocation = serializedObject.FindProperty("playAudioOnCheckLocation");
            playAudioOnSearchStart = serializedObject.FindProperty("playAudioOnSearchStart");
            playAudioOnSearchEnd = serializedObject.FindProperty("playAudioOnSearchEnd");
        }


        public override void OnInspectorGUI () 
        {
            DistractedStateBehaviour script = (DistractedStateBehaviour) target;
            int spaceBetween = 15;


            EditorGUILayout.PropertyField(timeToReact);
            EditorGUILayout.Space(spaceBetween);

            
            EditorGUILayout.PropertyField(checkLocation);
            if (script.checkLocation) {
                EditorGUILayout.PropertyField(timeBeforeMovingToLocation);
                EditorGUILayout.Space();


                EditorGUILayout.PropertyField(checkAnim);
                EditorGUILayout.PropertyField(checkAnimT);
                EditorGUILayout.Space();


                EditorGUILayout.PropertyField(timeToCheck);
                EditorGUILayout.Space();
                

                EditorGUILayout.PropertyField(randomizePoint);
                if (script.randomizePoint) {
                    EditorGUILayout.PropertyField(randomizeRadius);
                }
                EditorGUILayout.Space();


                EditorGUILayout.PropertyField(playAudioOnCheckLocation);
                EditorGUILayout.Space(spaceBetween);


                EditorGUILayout.PropertyField(searchLocationRadius);
                EditorGUILayout.Space();
                if (script.searchLocationRadius) {
                    EditorGUILayout.PropertyField(searchPoints);
                    EditorGUILayout.PropertyField(searchPointAnim);
                    EditorGUILayout.PropertyField(pointWaitTime);
                    EditorGUILayout.Space();

                    EditorGUILayout.PropertyField(endSearchAnim);
                    EditorGUILayout.PropertyField(endSearchAnimTime);
                    EditorGUILayout.Space();

                    EditorGUILayout.PropertyField(searchAnimsT);
                    EditorGUILayout.Space();
                    
                    EditorGUILayout.PropertyField(playAudioOnSearchStart);
                    EditorGUILayout.PropertyField(playAudioOnSearchEnd);
                }
            }
            

            serializedObject.ApplyModifiedProperties();
        }
    }
}