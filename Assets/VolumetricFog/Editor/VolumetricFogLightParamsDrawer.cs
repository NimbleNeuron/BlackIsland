using UnityEditor;
using UnityEngine;

namespace VolumetricFogAndMist {
    [CustomPropertyDrawer(typeof(VolumetricFog.PointLightParams))]
    public class VolumetricFogLightParamsDrawer : PropertyDrawer {

        // Draw the property inside the given rect
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

            EditorGUI.BeginProperty(position, label, property);
            float lineHeight = EditorGUIUtility.standardVerticalSpacing + EditorGUIUtility.singleLineHeight;
            position.height = EditorGUIUtility.singleLineHeight;

            SerializedProperty light = property.FindPropertyRelative("light");

            EditorGUI.PropertyField(position, light, new GUIContent("Light " + (GetIndex(property) + 1)));

            if (light.objectReferenceValue == null) {
                EditorGUI.indentLevel++;
                SerializedProperty positionProp = property.FindPropertyRelative("position");
                position.y += lineHeight;
                EditorGUI.PropertyField(position, positionProp);

                SerializedProperty colorProp = property.FindPropertyRelative("color");
                position.y += lineHeight;
                EditorGUI.PropertyField(position, colorProp);

                SerializedProperty rangeProp = property.FindPropertyRelative("range");
                position.y += lineHeight;
                EditorGUI.PropertyField(position, rangeProp);

                SerializedProperty rangeMultiplierProp = property.FindPropertyRelative("rangeMultiplier");
                position.y += lineHeight;
                EditorGUI.PropertyField(position, rangeMultiplierProp);

                SerializedProperty intensityProp = property.FindPropertyRelative("intensity");
                position.y += lineHeight;
                EditorGUI.PropertyField(position, intensityProp);

                SerializedProperty intensityMultiplierProp = property.FindPropertyRelative("intensityMultiplier");
                position.y += lineHeight;
                EditorGUI.PropertyField(position, intensityMultiplierProp);
                EditorGUI.indentLevel--;
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            float lineHeight = EditorGUIUtility.standardVerticalSpacing + EditorGUIUtility.singleLineHeight;

            SerializedProperty light = property.FindPropertyRelative("light");
            float totalHeight = light.objectReferenceValue == null ? lineHeight * 7 : lineHeight;
            return totalHeight;
        }

        int GetIndex(SerializedProperty property) {
            string s = property.propertyPath;
            int bracket = s.LastIndexOf("[");
            if (bracket >= 0) {
                string indexStr = s.Substring(bracket + 1, s.Length - bracket - 2);
                int index;
                if (int.TryParse(indexStr, out index)) {
                    return index;
                }
            }
            return 0;
        }

    }


}