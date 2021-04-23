using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor;
using System.Collections.Generic;

namespace Knife.Tools
{
    public static class DPTResourcesLoader
    {
        public static Texture2D CommonButton;
        public static Texture2D SimplePlacementIcon;
        public static Texture2D BurstPlacementIcon;
        public static Texture2D PaintingPlacementIcon;
        public static Texture2D AddTemplateIcon;
        public static Texture2D EmptyImage;
        public static Texture2D Header1;
        public static Texture2D Header2;
        public static Texture2D Header3;
        public static Texture2D AlphaGrid;
        public static GUISkin Skin;
        public static GUISkin SelectedSkin;
        public static GUISkin GridSkin;
        public static GUISkin SelectedGridSkin;
        public static GUISkin AddTemplateGridSkin;
        public static GUISkin StretchBoxSkin;
        public static Shader GUIShader;
        private static Material shadowMaterial;

        public static GUIStyle ButtonStyle
        {
            get
            {
                return Skin.GetStyle("Button");
            }
        }

        public static GUIStyle SelectedButtonStyle
        {
            get
            {
                return SelectedSkin.GetStyle("Button");
            }
        }

        public static GUIStyle DefaultGridButtonStyle
        {
            get
            {
                return GridSkin.GetStyle("Button");
            }
        }

        public static GUIStyle SelectedGridButtonStyle
        {
            get
            {
                return SelectedGridSkin.GetStyle("Button");
            }
        }

        public static GUIStyle EmptyButtonGridStyle
        {
            get
            {
                return GridSkin.GetStyle("Box");
            }
        }

        public static GUIStyle AddTemplateButtonGridStyle
        {
            get
            {
                return AddTemplateGridSkin.GetStyle("Button");
            }
        }

        public static Material ShadowMaterial
        {
            get
            {
                if (shadowMaterial == null)
                    shadowMaterial = new Material(Shader.Find("Hidden/Knife/ShadowCreate"));

                return shadowMaterial;
            }

            private set
            {
                shadowMaterial = value;
            }
        }

        public static DecalTemplatesKit EditorKit
        {
            get
            {
                if(editorKit == null)
                {
                    var kitsGUIDS = AssetDatabase.FindAssets("t:DecalTemplatesKit");
                    for (int i = 0; i < kitsGUIDS.Length; i++)
                    {
                        var kit = AssetDatabase.LoadAssetAtPath<DecalTemplatesKit>(AssetDatabase.GUIDToAssetPath(kitsGUIDS[i]));
                        if(kit != null && kit.name.Equals("EditorLastUsedTemplates Kit"))
                        {
                            editorKit = kit;
                            break;
                        }
                    }

                    if(editorKit == null)
                    {
                        Debug.Log("Not found kit");
                        string path = "Assets/Knife/Decal Master/EditorLastUsedTemplates Kit.asset";

                        editorKit = ScriptableObject.CreateInstance<DecalTemplatesKit>();
                        editorKit.Templates = new List<DecalPlacementTool.DecalTemplate>();
                        AssetDatabase.CreateAsset(editorKit, path);
                        AssetDatabase.Refresh();
                        AssetDatabase.SaveAssets();
                    }
                }

                return editorKit;
            }

            private set
            {
                editorKit = value;
            }
        }

        static DecalTemplatesKit editorKit;

        static CommandBuffer blitBuffer;

        const string prefix = "Style/Knife.DPT.";

        static DPTResourcesLoader()
        {
            Init();
        }

        [InitializeOnLoadMethod]
        public static void Init()
        {
            Resources.UnloadUnusedAssets();
            CommonButton = LoadTexture("Button");
            SimplePlacementIcon = LoadTexture("Simple");
            BurstPlacementIcon = LoadTexture("Burst");
            PaintingPlacementIcon = LoadTexture("Painting");
            Skin = LoadSkin("Skin");
            SelectedSkin = LoadSkin("SelectedSkin");
            GridSkin = LoadSkin("GridSkin");
            AddTemplateIcon = LoadTexture("AddTemplate");
            EmptyImage = LoadTexture("Empty");
            SelectedGridSkin = LoadSkin("SelectedGridSkin");
            AddTemplateGridSkin = LoadSkin("AddTemplateGridSkin");

            Header1 = LoadTexture("HEADER_1");
            Header2 = LoadTexture("HEADER_2");
            Header3 = LoadTexture("HEADER_3");
            AlphaGrid = LoadTexture("AlphaGrid");
            StretchBoxSkin = LoadSkin("StretchBoxSkin");

            GUIShader = LoadShader("GUIShader");

            blitBuffer = new CommandBuffer();
        }

        public static void BlitTexture(Texture src, Texture dst, Material mat = null, int pass = 0)
        {
            blitBuffer.Clear();
            blitBuffer.Blit(src, dst, mat, pass);
            Graphics.ExecuteCommandBuffer(blitBuffer);
        }

        static Texture2D LoadTexture(string resourceName)
        {
            return Load<Texture2D>(resourceName);
        }

        static GUISkin LoadSkin(string resourceName)
        {
            return Load<GUISkin>(resourceName);
        }

        static Shader LoadShader(string resourceName)
        {
            return Load<Shader>(resourceName);
        }

        static T Load<T>(string resourceName) where T : Object
        {
            return Resources.Load<T>(prefix + resourceName);
        }
    }
}