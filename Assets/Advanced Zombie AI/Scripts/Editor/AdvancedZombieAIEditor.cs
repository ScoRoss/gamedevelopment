using UnityEngine;
using UnityEditor;

namespace FMS_AdvancedZombieAI
{
    [CustomEditor(typeof(AdvancedZombieAI))]
    public class AdvancedZombieAIEditor : Editor
    {
        SerializedProperty healthProp;

        SerializedProperty wanderingModeProp;
        SerializedProperty waypointsToWanderProp;
        SerializedProperty stateProp;
        SerializedProperty motionStateProp;
        SerializedProperty wanderingRadiusProp;
        SerializedProperty followPlayerMovementOffsetProp;
        SerializedProperty wanderingSpeedProp;

        SerializedProperty fieldOfViewProp;
        SerializedProperty viewDistanceProp;
        SerializedProperty showGizmosProp;

        SerializedProperty chasingSpeedProp;
        SerializedProperty losePlayerTimeProp;
        SerializedProperty randomMovementOffsetProp;

        SerializedProperty attackRangeProp;
        SerializedProperty attackCooldownProp;
        SerializedProperty attackForceProp;


        SerializedProperty headWeightProp;
        SerializedProperty bodyWeightProp;

        SerializedProperty takeExtraDamageIfShotOnHeadProp;
        SerializedProperty zombieHeadProp;
        SerializedProperty headDamageProp;

        SerializedProperty willCrawlAfterDepletingHealthProp;
        SerializedProperty crawlingHealthProp;
        SerializedProperty crawlingWanderSpeedProp;
        SerializedProperty crawlingChaseSpeedProp;
        SerializedProperty moveDelayAfterFallProp;


        bool wanderingFoldout, detectionFoldout, chasingfoldout, attackFoldout, headIKProp = false;


        private void OnEnable()
        {
            healthProp = serializedObject.FindProperty("health");

            willCrawlAfterDepletingHealthProp = serializedObject.FindProperty("willCrawlAfterDepletingHealth");
            crawlingHealthProp = serializedObject.FindProperty("crawlingHealth");
            moveDelayAfterFallProp = serializedObject.FindProperty("moveDelayAfterFall");
            crawlingWanderSpeedProp = serializedObject.FindProperty("crawlingWanderSpeed");
            crawlingChaseSpeedProp = serializedObject.FindProperty("crawlingChaseSpeed");
            wanderingModeProp = serializedObject.FindProperty("wanderingMode");
            stateProp = serializedObject.FindProperty("actionState");
            waypointsToWanderProp = serializedObject.FindProperty("waypointsToWander");
            motionStateProp = serializedObject.FindProperty("motionState");
            wanderingRadiusProp = serializedObject.FindProperty("wanderingRadius");
            followPlayerMovementOffsetProp = serializedObject.FindProperty("followPlayerMovementOffset");
            wanderingSpeedProp = serializedObject.FindProperty("wanderingSpeed");
            fieldOfViewProp = serializedObject.FindProperty("fieldOfView");
            viewDistanceProp = serializedObject.FindProperty("viewDistance");
            showGizmosProp = serializedObject.FindProperty("ShowGizmos");

            chasingSpeedProp = serializedObject.FindProperty("chasingSpeed");
            losePlayerTimeProp = serializedObject.FindProperty("losePlayerTime");
            randomMovementOffsetProp = serializedObject.FindProperty("randomMovementOffset");

            attackRangeProp = serializedObject.FindProperty("attackRange");
            attackCooldownProp = serializedObject.FindProperty("attackCooldown");
            attackForceProp = serializedObject.FindProperty("attackForce");


            headWeightProp = serializedObject.FindProperty("headWeight");
            bodyWeightProp = serializedObject.FindProperty("bodyWeight");

            takeExtraDamageIfShotOnHeadProp = serializedObject.FindProperty("extraDamageIfShotOnHead");
            zombieHeadProp = serializedObject.FindProperty("zombieHead");
            headDamageProp = serializedObject.FindProperty("headDamage");

        }


        public override void OnInspectorGUI()
        {
            serializedObject.Update();


            GUIStyle boldFoldoutStyle = new GUIStyle(EditorStyles.foldout);
            boldFoldoutStyle.fontStyle = FontStyle.Bold;

            EditorGUILayout.BeginVertical("box");

            EditorGUILayout.PropertyField(motionStateProp);
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical("box");

            EditorGUILayout.PropertyField(stateProp);

            EditorGUILayout.LabelField("Read Only", EditorStyles.centeredGreyMiniLabel);
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();




            wanderingFoldout = EditorGUILayout.Foldout(wanderingFoldout, new GUIContent("Wander Settings", "Wander Settings"), true, boldFoldoutStyle);

            if (wanderingFoldout)
            {
                EditorGUILayout.BeginVertical("box");

                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(wanderingModeProp);

                EditorGUILayout.PropertyField(wanderingSpeedProp);


                if ((AdvancedZombieAI.WanderingMode)wanderingModeProp.enumValueIndex == AdvancedZombieAI.WanderingMode.FollowPlayer)
                {
                    EditorGUILayout.PropertyField(followPlayerMovementOffsetProp, true);
                }

                if ((AdvancedZombieAI.WanderingMode)wanderingModeProp.enumValueIndex == AdvancedZombieAI.WanderingMode.RandomWandering)
                {
                    EditorGUILayout.PropertyField(wanderingRadiusProp, true);
                }

                if ((AdvancedZombieAI.WanderingMode)wanderingModeProp.enumValueIndex == AdvancedZombieAI.WanderingMode.WaypointWandering)
                {
                    EditorGUILayout.PropertyField(waypointsToWanderProp, true);
                }
                EditorGUI.indentLevel--;
                EditorGUILayout.EndVertical();


            }

            EditorGUILayout.Space();


            detectionFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(detectionFoldout, "Detection Settings");

            if (detectionFoldout)
            {
                EditorGUILayout.BeginVertical("box");

                EditorGUI.indentLevel++;

                EditorGUILayout.PropertyField(fieldOfViewProp);
                EditorGUILayout.PropertyField(viewDistanceProp);
                EditorGUILayout.PropertyField(showGizmosProp);

                EditorGUI.indentLevel--;

                EditorGUILayout.EndVertical();


            }
            EditorGUILayout.EndFoldoutHeaderGroup();


            EditorGUILayout.Space();


            chasingfoldout = EditorGUILayout.BeginFoldoutHeaderGroup(chasingfoldout, "Chase Settings");

            if (chasingfoldout)
            {
                EditorGUILayout.BeginVertical("box");

                EditorGUI.indentLevel++;

                EditorGUILayout.PropertyField(chasingSpeedProp);
                EditorGUILayout.PropertyField(losePlayerTimeProp);
                EditorGUILayout.PropertyField(randomMovementOffsetProp);
                EditorGUI.indentLevel--;

                EditorGUILayout.EndVertical();

            }
            EditorGUILayout.EndFoldoutHeaderGroup();


            EditorGUILayout.Space();


            attackFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(attackFoldout, "Attack Settings");

            if (attackFoldout)
            {
                EditorGUILayout.BeginVertical("box");

                EditorGUI.indentLevel++;

                EditorGUILayout.PropertyField(attackRangeProp);
                EditorGUILayout.PropertyField(attackCooldownProp);
                EditorGUILayout.PropertyField(attackForceProp);
                EditorGUI.indentLevel--;

                EditorGUILayout.EndVertical();

            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            EditorGUILayout.Space();

            EditorGUILayout.Space();

            headIKProp = EditorGUILayout.BeginFoldoutHeaderGroup(headIKProp, "Head IK");

            if (headIKProp)
            {
                EditorGUILayout.BeginVertical("box");

                EditorGUI.indentLevel++;

                EditorGUILayout.PropertyField(headWeightProp);
                EditorGUILayout.PropertyField(bodyWeightProp);
                EditorGUI.indentLevel--;

                EditorGUILayout.EndVertical();

            }
            EditorGUILayout.EndFoldoutHeaderGroup();


            EditorGUILayout.Space();
            EditorGUILayout.Space();


            EditorGUILayout.PropertyField(willCrawlAfterDepletingHealthProp);

            if (willCrawlAfterDepletingHealthProp.boolValue)
            {
                EditorGUILayout.BeginVertical("box");

                EditorGUI.indentLevel++;

                EditorGUILayout.PropertyField(crawlingHealthProp);
                EditorGUILayout.PropertyField(moveDelayAfterFallProp);
                EditorGUILayout.PropertyField(crawlingWanderSpeedProp);
                EditorGUILayout.PropertyField(crawlingChaseSpeedProp);
                EditorGUI.indentLevel--;

                EditorGUILayout.EndVertical();

            }
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(takeExtraDamageIfShotOnHeadProp);

            if (takeExtraDamageIfShotOnHeadProp.boolValue)
            {
                EditorGUILayout.BeginVertical("Box");

                EditorGUI.indentLevel++;

                EditorGUILayout.PropertyField(zombieHeadProp);
                EditorGUILayout.PropertyField(headDamageProp);

                EditorGUI.indentLevel--;

                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.PropertyField(healthProp);
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();

            serializedObject.ApplyModifiedProperties();

        }

        private void OnSceneGUI()
        {
            AdvancedZombieAI zombieAI = (AdvancedZombieAI)target;
            if (zombieAI.ShowGizmos)
            {
                Handles.color = Color.white;
                Handles.DrawWireArc(zombieAI.transform.position, Vector3.up, Vector3.forward, 360, zombieAI.viewDistance);

                Vector3 LAngle = Angle(zombieAI.transform.eulerAngles.y, -zombieAI.fieldOfView / 2);
                Vector3 RAngle = Angle(zombieAI.transform.eulerAngles.y, zombieAI.fieldOfView / 2);


                Handles.DrawLine(zombieAI.transform.position, zombieAI.transform.position + LAngle * zombieAI.viewDistance);
                Handles.DrawLine(zombieAI.transform.position, zombieAI.transform.position + RAngle * zombieAI.viewDistance);
            }

        }

        private Vector3 Angle(float Y, float angle)
        {
            angle += Y;

            return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad));
        }
    }
}
