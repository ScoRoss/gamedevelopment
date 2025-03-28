using UnityEngine;
using UnityEditor;

namespace FMS_AdvancedZombieAI
{
    [CustomEditor(typeof(ZombieSpawner))]
    public class ZombieSpawnerEditor : Editor
    {
        SerializedProperty zombiePrefabsProp;
        SerializedProperty spawnModeProp;
        SerializedProperty spawnIntervalProp;


        private void OnEnable()
        {
            zombiePrefabsProp = serializedObject.FindProperty("zombiePrefabs");
            spawnModeProp = serializedObject.FindProperty("spawnMode");
            spawnIntervalProp = serializedObject.FindProperty("spawnInterval");

        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(zombiePrefabsProp);
            EditorGUILayout.PropertyField(spawnModeProp);

            if ((ZombieSpawner.SpawnMode)spawnModeProp.enumValueIndex == ZombieSpawner.SpawnMode.OverTime)
            {
                EditorGUILayout.PropertyField(spawnIntervalProp);

            }


            serializedObject.ApplyModifiedProperties();

        }
    }
}