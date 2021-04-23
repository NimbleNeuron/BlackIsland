using UnityEngine;
using UnityEditor;

namespace Knife.DeferredDecals.Spawn
{
    [CustomPropertyDrawer(typeof(DecalSpawnController))]
    [CanEditMultipleObjects]
    public class DecalSpawnControllerDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty spawnMode, decalPrefab, spawner, destroyDelay;
            spawnMode = property.FindPropertyRelative("SpawnMode");
            decalPrefab = property.FindPropertyRelative("DecalPrefab");
            spawner = property.FindPropertyRelative("Spawner");
            destroyDelay = property.FindPropertyRelative("DestroyDelay");

            if(property.isExpanded)
            {
                float height = EditorGUIUtility.singleLineHeight;
                height += EditorGUI.GetPropertyHeight(spawnMode);
                if (spawnMode.enumValueIndex == (int)SpawnType.Instantiate)
                    height += EditorGUIUtility.singleLineHeight;
                else if (spawnMode.enumValueIndex == (int)SpawnType.Pool)
                    height += EditorGUIUtility.singleLineHeight;

                height += EditorGUIUtility.singleLineHeight;

                return height;
            }
            else
            {
                float height = EditorGUIUtility.singleLineHeight;
                return height;
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //base.OnGUI(position, property, label);
            SerializedProperty spawnMode, decalPrefab, spawner, destroyDelay;
            spawnMode = property.FindPropertyRelative("SpawnMode");
            decalPrefab = property.FindPropertyRelative("DecalPrefab");
            spawner = property.FindPropertyRelative("Spawner");
            destroyDelay = property.FindPropertyRelative("DestroyDelay");

            position.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(position, property, false);

            if (property.isExpanded)
            {
                EditorGUI.indentLevel++;
                position.y += EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(position, spawnMode);

                position.y += EditorGUI.GetPropertyHeight(spawnMode);
                if (spawnMode.enumValueIndex == (int)SpawnType.Instantiate)
                {
                    EditorGUI.PropertyField(position, decalPrefab);
                    position.y += EditorGUI.GetPropertyHeight(decalPrefab);
                }
                else if (spawnMode.enumValueIndex == (int)SpawnType.Pool)
                {
                    EditorGUI.PropertyField(position, spawner);
                    position.y += EditorGUI.GetPropertyHeight(spawner);
                }

                EditorGUI.PropertyField(position, destroyDelay);
                EditorGUI.indentLevel--;
            }

            if (EditorGUI.EndChangeCheck())
            {
                property.serializedObject.ApplyModifiedProperties();
            }
        }
    }
}