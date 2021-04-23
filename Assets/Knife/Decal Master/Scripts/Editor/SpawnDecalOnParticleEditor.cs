using UnityEditor;

namespace Knife.DeferredDecals.Spawn
{
    [CustomEditor(typeof(SpawnDecalOnParticle))]
    [CanEditMultipleObjects]
    public class SpawnDecalOnParticleEditor : Editor
    {
        SerializedProperty spawnControl, spawnController, offset, parentOnCollide, rotationJitter;

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
            if(EditorGUI.EndChangeCheck())
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
        }
    }
}