using UnityEngine;
using UnityEditor;
using BlazeAISpace;

namespace BlazeAISpace
{
    [CustomEditor(typeof(BlazeAIDistanceCulling))]
    public class BlazeAIDistanceCullingInspector : Editor
    {
        SerializedProperty autoCatchCamera,
        playerOrCamera,
        distanceToCull,
        cycleFrames,
        disableBlazeOnly;


        void OnEnable()
        {
            autoCatchCamera = serializedObject.FindProperty("autoCatchCamera");
            playerOrCamera = serializedObject.FindProperty("playerOrCamera");
            distanceToCull = serializedObject.FindProperty("distanceToCull");
            cycleFrames = serializedObject.FindProperty("cycleFrames");
            disableBlazeOnly = serializedObject.FindProperty("disableBlazeOnly");
        }


        public override void OnInspectorGUI () 
        {
            BlazeAIDistanceCulling script = (BlazeAIDistanceCulling)target;
            
            EditorGUILayout.PropertyField(autoCatchCamera);
            
            if (!script.autoCatchCamera) {
                EditorGUILayout.PropertyField(playerOrCamera);
            }

            EditorGUILayout.Space(7);
            
            EditorGUILayout.PropertyField(distanceToCull);
            EditorGUILayout.PropertyField(cycleFrames);

            EditorGUILayout.Space(7);

            EditorGUILayout.PropertyField(disableBlazeOnly);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
