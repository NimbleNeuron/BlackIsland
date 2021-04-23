using UnityEditor;

namespace Knife.DeferredDecals.Spawn
{
    [CustomEditor(typeof(SpawnDecalOnCollision))]
    [CanEditMultipleObjects]
    public class SpawnDecalOnCollisionEditor : Editor
    {
        SerializedProperty spawnControl, spawnController, offset, parentOnCollide, spawnOnCollisionEnter, spawnOnCollisionStay, rotationJitter;
        SerializedProperty spawnOnCollisionExit, tagCheck;

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();

            var selectedSpawnControl = (SpawnControl)spawnControl.enumValueIndex;
            EditorGUILayout.PropertyField(spawnControl);

            if (selectedSpawnControl == SpawnControl.ByReceiver)
            {

            } else if(selectedSpawnControl == SpawnControl.BySpawnController)
            {
                EditorGUILayout.PropertyField(spawnController);
                EditorGUILayout.PropertyField(offset);
                EditorGUILayout.PropertyField(parentOnCollide);
                EditorGUILayout.PropertyField(rotationJitter);
            }

            EditorGUILayout.PropertyField(spawnOnCollisionEnter);
            EditorGUILayout.PropertyField(spawnOnCollisionStay);
            EditorGUILayout.PropertyField(spawnOnCollisionExit);
            EditorGUILayout.PropertyField(tagCheck);

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }

        private void OnEnable()
        {
            spawnControl = serializedObject.FindProperty("SpawnControl");
            spawnController = serializedObject.FindProperty("SpawnController");
            offset = serializedObject.FindProperty("Offset");
            parentOnCollide = serializedObject.FindProperty("ParentOnCollide");
            rotationJitter = serializedObject.FindProperty("RotationJitter");

            spawnOnCollisionEnter = serializedObject.FindProperty("SpawnOnCollisionEnter");
            spawnOnCollisionStay = serializedObject.FindProperty("SpawnOnCollisionStay");
            spawnOnCollisionExit = serializedObject.FindProperty("SpawnOnCollisionExit");
            tagCheck = serializedObject.FindProperty("TagCheck");
        }
    }
}