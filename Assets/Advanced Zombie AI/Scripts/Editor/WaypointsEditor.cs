using UnityEngine;
using UnityEditor;

namespace FMS_AdvancedZombieAI
{
    [CustomEditor(typeof(Waypoints))]

    public class WaypointsEditor : Editor
    {
        SerializedProperty toDoProp;
        SerializedProperty timeToWaitProp;
        SerializedProperty customAnimationProp;

        private void OnEnable()
        {

            toDoProp = serializedObject.FindProperty("toDo");
            timeToWaitProp = serializedObject.FindProperty("timeToWait");
            customAnimationProp = serializedObject.FindProperty("customAnimation");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(toDoProp);

            if  ((Waypoints.ToDo)toDoProp.enumValueIndex != Waypoints.ToDo.Nothing)
            {
                EditorGUILayout.PropertyField(timeToWaitProp, true);
                EditorGUILayout.Space();

            }


            if ((Waypoints.ToDo)toDoProp.enumValueIndex == Waypoints.ToDo.Custom)
            {
                EditorGUILayout.PropertyField(customAnimationProp, true);
            }
            serializedObject.ApplyModifiedProperties();

        }
    }
}
