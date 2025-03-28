using UnityEngine;
using UnityEditor;

namespace FMS_AdvancedZombieAI
{
    public class ZombieMaker : EditorWindow
    {
        private GameObject zombieModel;
        private AnimatorOverrideController animatorController;
        [MenuItem("Window/Advanced Zombie AI/Zombie AI Setup")]

        public static void ShowWindow()
        {
            GetWindow<ZombieMaker>("Zombie AI Setup");
        }

        private void OnGUI()
        {
            EditorGUILayout.Space();

            GUILayout.Label("Drag and drop your zombie game object here", EditorStyles.boldLabel);

            EditorGUILayout.Space();


            Event evt = Event.current;
            Rect dropArea = GUILayoutUtility.GetRect(0.0f, 50.0f, GUILayout.ExpandWidth(true));
            EditorGUILayout.Space();

            GUI.Box(dropArea, "Drop Zone");
            zombieModel = EditorGUILayout.ObjectField("Zombie Game Object", zombieModel, typeof(GameObject), true) as GameObject;


            switch (evt.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (!dropArea.Contains(evt.mousePosition))
                        break;

                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                    if (evt.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();

                        foreach (object draggedObject in DragAndDrop.objectReferences)
                        {
                            if (draggedObject is GameObject)
                            {
                                zombieModel = (GameObject)draggedObject;
                            }
                        }
                    }
                    break;
            }

            animatorController = EditorGUILayout.ObjectField("Animator Override Controller", animatorController, typeof(AnimatorOverrideController), false) as AnimatorOverrideController;
            EditorGUILayout.Space();
            if (GUILayout.Button("Setup Zombie AI"))
            {
                if (zombieModel != null)
                {
                    SetupZombieAI(zombieModel);
                }
                else
                {
                    Debug.LogWarning("Please drag and drop a zombie GameObject first.");
                }
            }
        }

        private void SetupZombieAI(GameObject zombie)
        {

            if (zombieModel != null)
            {

                if (zombie.GetComponent<Animator>() == null)
                {
                    zombie.AddComponent<Animator>();
                }
                if (zombie.GetComponent<SphereCollider>() == null)
                {
                    zombie.AddComponent<SphereCollider>();
                    SphereCollider col = zombie.GetComponent<SphereCollider>();
                    col.radius = 0.01f;
                }

                if (zombie.GetComponent<UnityEngine.AI.NavMeshAgent>() == null)
                {
                    zombie.AddComponent<UnityEngine.AI.NavMeshAgent>();
                }

                if (zombie.GetComponent<AdvancedZombieAI>() == null)
                {
                    zombie.AddComponent<AdvancedZombieAI>();
                }
                if (zombie.GetComponent<ZombieAttackHandler>() == null)
                {
                    zombie.AddComponent<ZombieAttackHandler>();
                }
                if (zombie.GetComponent<ZombieChaseHandler>() == null)
                {
                    zombie.AddComponent<ZombieChaseHandler>();
                }
                if (zombie.GetComponent<ZombieWanderingHandler>() == null)
                {
                    zombie.AddComponent<ZombieWanderingHandler>();
                }
                if (zombie.GetComponent<ZombieCrawlingHandler>() == null)
                {
                    zombie.AddComponent<ZombieCrawlingHandler>();
                }
                if (zombie.GetComponent<HeadIK>() == null)
                {
                    zombie.AddComponent<HeadIK>();
                }

                int zombieLayer = LayerMask.NameToLayer("Zombie");
                zombieModel.layer = zombieLayer;

                SetTag();

                Animator animator = zombieModel.GetComponent<Animator>();
                if (animator != null && animatorController != null)
                {
                    if (animator.runtimeAnimatorController == null || !(animator.runtimeAnimatorController is AnimatorOverrideController))
                    {
                        animator.runtimeAnimatorController = animatorController;
                    }
                }


                Debug.Log("Advanced Zombie AI Setup Complete!");

            }
        }

        private void SetTag()
        {
            if (!IsTagExists("Zombie"))
            {
                SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
                SerializedProperty tagsProp = tagManager.FindProperty("tags");

                tagsProp.InsertArrayElementAtIndex(0);
                SerializedProperty newTag = tagsProp.GetArrayElementAtIndex(0);
                newTag.stringValue = "Zombie";

                tagManager.ApplyModifiedProperties();
            }

            zombieModel.tag = "Zombie";
        }

        private bool IsTagExists(string tag)
        {
            string[] tags = UnityEditorInternal.InternalEditorUtility.tags;
            for (int i = 0; i < tags.Length; i++)
            {
                if (tags[i] == tag)
                {
                    return true;
                }
            }
            return false;
        }

    }
}
