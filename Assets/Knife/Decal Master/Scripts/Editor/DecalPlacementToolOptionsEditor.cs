using UnityEditor;

namespace Knife.Tools
{
    [CustomEditor(typeof(DecalPlacementToolOptions))]
    public class DecalPlacementToolOptionsEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var positionJitter = serializedObject.FindProperty("PositionJitter");
            var rotationJitter = serializedObject.FindProperty("RotationJitter");
            var sizeJitter = serializedObject.FindProperty("SizeJitter");
            var parentToHitted = serializedObject.FindProperty("ParentToHittedRenderer");
            var projOffset = serializedObject.FindProperty("ProjectionOffset");
            var projDistance = serializedObject.FindProperty("ProjectionDistance");
            var order = serializedObject.FindProperty("SortingOrder");
            var minPaintingDistance = serializedObject.FindProperty("MinPaintingDistance");
            var rotateToNext = serializedObject.FindProperty("RotateToNext");
            var lineAsOneDecal = serializedObject.FindProperty("LineAsOneDecal");
            var size = serializedObject.FindProperty("Size");

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.Slider(positionJitter, 0, 1);
            EditorGUILayout.Slider(rotationJitter, 0, 1);
            EditorGUILayout.Slider(sizeJitter, 0, 1);
            EditorGUILayout.Slider(projOffset, 0.001f, 1f);
            EditorGUILayout.Slider(projDistance, 0.001f, 3f);
            EditorGUILayout.Slider(minPaintingDistance, 0f, 5f);
            EditorGUILayout.PropertyField(order);
            EditorGUILayout.PropertyField(parentToHitted);
            EditorGUILayout.PropertyField(rotateToNext);
            EditorGUILayout.PropertyField(lineAsOneDecal);
            EditorGUILayout.PropertyField(size);


            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                DecalPlacementToolOptions options = target as DecalPlacementToolOptions;

                if(options.OnChanged != null)
                    options.OnChanged();
            }
        }
    }
}