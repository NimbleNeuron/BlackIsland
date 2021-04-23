using UnityEditor;
using UnityEngine;

namespace Knife.DeferredDecals
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Decal))]
    public class DecalEditor : Editor
    {
        Decal decal;
        MaterialEditor decalMaterialEditor;
        Material lastMaterial;

        MaterialEditor DecalMaterialEditor
        {
            get
            {
                if (decalMaterialEditor == null || decal.DecalMaterial != lastMaterial)
                {
                    decalMaterialEditor = CreateEditor(decal.DecalMaterial) as MaterialEditor;
                }

                return decalMaterialEditor;
            }
        }


        static Texture2D header;
        public static Texture2D Header
        {
            get
            {
                if (header == null)
                    header = Resources.Load<Texture2D>("Knife.DecalLogo");

                return header;
            }
        }

        public override void OnInspectorGUI()
        {
            GUILayout.Label(Header, GUILayout.MaxWidth(400));
            base.OnInspectorGUI();

            if (decal.DecalMaterial == null)
                return;

            try
            {
                DecalMaterialEditor.DrawHeader();
                EditorGUI.BeginDisabledGroup(decal.DecalMaterial == DeferredDecalsSystem.DefaultDecalMaterial);
                DecalMaterialEditor.OnInspectorGUI();
                EditorGUI.EndDisabledGroup();
            } catch
            {
                decalMaterialEditor = null;
            }
            lastMaterial = decal.DecalMaterial;
        }

        private void OnEnable()
        {
            decal = target as Decal;
        }

        private void OnDestroy()
        {
            if (decalMaterialEditor != null)
            {
                DestroyImmediate(decalMaterialEditor, true);
            }
        }

        [MenuItem("GameObject/3D Object/Decal")]
        public static void CreateDecal()
        {
            var scene = SceneView.sceneViews[0] as SceneView;

            GameObject decalInstance = new GameObject("Decal", typeof(Decal));

            decalInstance.transform.position = scene.pivot;
            Selection.activeObject = decalInstance;
            Undo.RegisterCreatedObjectUndo(decalInstance, "Create Decal");
        }
    }
}