using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Knife.Tools
{
    [CustomPropertyDrawer(typeof(DecalPlacementTool.DecalTemplate))]
    public class DecalTemplateDrawer : PropertyDrawer
    {
        static Dictionary<DecalPlacementTool.DecalTemplate, RenderTexture> cachedPreviews = new Dictionary<DecalPlacementTool.DecalTemplate, RenderTexture>();
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var template = GetDecalTemplate(property);

            if (property.isExpanded)
            {
                return base.GetPropertyHeight(property, label) + EditorGUIUtility.singleLineHeight + EditorGUIUtility.singleLineHeight + EditorGUI.GetPropertyHeight(property.FindPropertyRelative("InstancedUV")) + template.PreviewSize;
            }
            else
            {
                return base.GetPropertyHeight(property, label);
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            label.text = property.displayName;
            //base.OnGUI(position, property, label);
            var template = GetDecalTemplate(property);

            if (template == null)
                return;

            RenderTexture preview;
            if(!cachedPreviews.TryGetValue(template, out preview))
            {
                preview = template.CreatePreview();
                cachedPreviews.Add(template, preview);
            }

            Rect foldoutRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, property.displayName);
            EditorGUI.BeginProperty(position, label, property);
            if (property.isExpanded)
            {
                EditorGUI.indentLevel++;
                Rect materialPropRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight);
                Rect colorPropRect = new Rect(materialPropRect.x, materialPropRect.y + EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight);
                Rect tilingOffsetPropRect = new Rect(colorPropRect.x, colorPropRect.y + EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight);
                Rect previewPropRect = new Rect(tilingOffsetPropRect.x, tilingOffsetPropRect.y + EditorGUI.GetPropertyHeight(property.FindPropertyRelative("InstancedUV")), template.PreviewSize, template.PreviewSize);

                EditorGUI.BeginChangeCheck();
                EditorGUI.PropertyField(materialPropRect, property.FindPropertyRelative("DecalMaterial"));
                EditorGUI.PropertyField(colorPropRect, property.FindPropertyRelative("InstancedColor"));
                EditorGUI.PropertyField(tilingOffsetPropRect, property.FindPropertyRelative("InstancedUV"), true);

                if (EditorGUI.EndChangeCheck())
                {
                    cachedPreviews.Remove(template);
                    preview.Release();
                    GameObject.DestroyImmediate(preview, true);
                    preview = template.CreatePreview();
                    cachedPreviews.Add(template, preview);
                }

                EditorGUI.LabelField(previewPropRect, new GUIContent(preview));
                EditorGUI.indentLevel--;
            }
            EditorGUI.EndProperty();
        }

        DecalPlacementTool.DecalTemplate GetDecalTemplate(SerializedProperty property)
        {
            var targetObject = property.serializedObject.targetObject;
            var targetObjectClassType = targetObject.GetType();

            if (property.name.Equals("data"))
            {
                int indexOpen = property.propertyPath.IndexOf("[");
                int indexClose = property.propertyPath.IndexOf("]");
                string elementIndexStr = property.propertyPath.Substring(indexOpen + 1, indexClose - indexOpen - 1);
                int elementIndex = int.Parse(elementIndexStr);
                int firstDot = property.propertyPath.IndexOf(".");
                string arrayPropertyName = property.propertyPath.Substring(0, firstDot);

                var field = targetObjectClassType.GetField(arrayPropertyName);
                var value = field.GetValue(targetObject);

                var templatesArray = value as List<DecalPlacementTool.DecalTemplate>;
                if (templatesArray == null)
                {
                    return null;
                }

                var template = templatesArray[elementIndex];

                if (template == null)
                {
                    //Debug.Log("Null template for" + property.propertyPath);
                }
                else
                {
                    //Debug.Log("HAS TEMPLATE!");
                }
                return template;
            }
            else
            {
                var field = targetObjectClassType.GetField(property.propertyPath);
                var value = field.GetValue(targetObject);

                var template = value as DecalPlacementTool.DecalTemplate;

                if (template == null)
                {
                    //Debug.Log("Null template for" + property.propertyPath);
                }
                else
                {
                    //Debug.Log("HAS TEMPLATE!");
                }
                return template;
            }
        }
    }
}