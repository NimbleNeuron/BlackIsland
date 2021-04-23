using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;
using Random = UnityEngine.Random;
using Knife.DeferredDecals;
using Object = UnityEngine.Object;

namespace Knife.Tools
{
    public class DecalPlacementToolOptions : ScriptableObject
    {
        public float PositionJitter = 0;
        public float RotationJitter = 0;
        public float SizeJitter = 0;
        public bool ParentToHittedRenderer;
        public float ProjectionOffset = 0.01f;
        public float ProjectionDistance = 1f;
        public float MinPaintingDistance = 1f;
        public int SortingOrder = 0;
        public bool RotateToNext;
        public bool LineAsOneDecal;
        public Vector3 Size = new Vector3(1,1,1);

        public Action OnChanged;
    }

    public enum PlacingType
    {
        None,
        OnePerClick,
        Painting,
        Burst
    }

    public class DecalPlacementTool : EditorWindow
    {
        [HideInInspector]
        private PlacingType Placing = PlacingType.None;
        DecalPlacementToolOptions options;

        bool isMouseHold0 = false;
        bool isMouseHold1 = false;
        int burstCount = 5;
        float burstSize = 1.5f;

        const double decalDoubleClickTimestep = 0.5f;
        Vector3 lastDecalPlacePositions;
        double lastDecalTemplateClickTime;

        const int burstPixelsPerSize = 100;

        GameObject lastDecalPrefab;
        GameObject decalProjector;
        DecalTemplate selectedTemplate;

        float randomAngle;
        Vector2 randomOffset;
        float randomScale;
        float scale = 1;
        float rotation = 0;

        bool RendererHitInfo
        {
            get
            {
                return Options.ParentToHittedRenderer;
            }
        }

        public DecalPlacementToolOptionsEditor OptionsEditor
        {
            get
            {
                if(optionsEditor == null || optionsEditor.target == null)
                {
                    optionsEditor = Editor.CreateEditor(Options, typeof(DecalPlacementToolOptionsEditor)) as DecalPlacementToolOptionsEditor;
                }

                return optionsEditor;
            }

            set
            {
                optionsEditor = value;
            }
        }

        public DecalPlacementToolOptions Options
        {
            get
            {
                if(options == null)
                {
                    options = ScriptableObject.CreateInstance<DecalPlacementToolOptions>();
                }

                return options;
            }

            set
            {
                options = value;
            }
        }

        List<DecalTemplate> decalsTemplates
        {
            get
            {
                return DPTResourcesLoader.EditorKit.Templates;
            }
        }

        DecalTemplatesKit kit
        {
            get
            {
                return DPTResourcesLoader.EditorKit;
            }
        }

        Texture2D burstCircle;

        List<int> selectedTemplates;
        int previousClickedIndex;

        bool isMouseOverWindow;

        DecalPlacementToolOptionsEditor optionsEditor;
        float decalTemplateElementSize = 100;
        // float decalTemplateOptionsHeight = 100;

        Vector2 decalsScroll;

        GameObject lastSpawnedDecal;

        List<GameObject> decalsLineList = new List<GameObject>();
        Vector2 startLinePos;
        bool hasStartLinePos;

        CameraData lastCameraData;
        GPURaycastInfo[] fullScreenHits;
        int fullScreenWidth;
        int fullScreenHeight;
        bool oldShift;

        struct CameraData
        {
            public int Width;
            public int Height;
            public float Fov;
            public Vector3 position;
            public Quaternion rotation;

            public CameraData(Camera cam)
            {
                Width = cam.pixelWidth;
                Height = cam.pixelHeight;
                Fov = cam.fieldOfView;
                position = cam.transform.position;
                rotation = cam.transform.rotation;
            }

            public static bool Equals(CameraData first, CameraData second)
            {
                return first.Width == second.Width
                    && first.Height == second.Height
                    && first.Fov == second.Fov
                    && first.position == second.position
                    && first.rotation == second.rotation;
            }
        }

        bool isCameraDirty(Camera cam)
        {
            bool isEquals = CameraData.Equals(lastCameraData, new CameraData(cam));
            return !isEquals;
        }

        Color defaultFrameColor = new Color(0f, 0.7137255f, 0.9803922f, 1f);

        int sortingOrder
        {
            get
            {
                var decals = GameObject.FindObjectsOfType<Decal>().ToList();
                decals.Remove(decalProjector.GetComponent<Decal>());
                if (decals.Count == 0)
                    return 0;

                return decals.ToList().Max((d) => d.SortingOrder) + 1;
            }
        }

        [MenuItem("Window/Knife/Decal Placement Tool")]
        public static void OpenPlacementTool()
        {
            DecalPlacementTool window = GetWindow<DecalPlacementTool>("Decal placement tool");
            window.Show();
        }

        private void OnEnable()
        {
            Awake();
        }

        private void Awake()
        {
            DPTResourcesLoader.Init();
            randomOffset = Random.insideUnitCircle;
            randomAngle = Random.Range(-1f, 1f);
            randomScale = Random.Range(0f, 1f);

            if(burstCircle == null)
                burstCircle = Resources.Load<Texture2D>("Knife.BurstCircle");

            SceneView.duringSceneGui -= OnSceneGUI;
            // SceneView.onSceneGUIDelegate -= OnSceneGUI;
            SceneView.duringSceneGui += OnSceneGUI;
            // SceneView.onSceneGUIDelegate += OnSceneGUI;

            selectedTemplates = new List<int>();

            Undo.undoRedoPerformed -= undoRedoPerformed;
            Undo.undoRedoPerformed += undoRedoPerformed;

        }

        private void undoRedoPerformed()
        {
            foreach(var dt in decalsTemplates)
            {
                dt.RecreatePreview();
            }
        }

        public void AddMaterialToTemplates(Material mat)
        {
            Undo.RecordObject(kit, "Current kit changed");
            var decalTemplate = new DecalTemplate(mat, (int)decalTemplateElementSize);
            decalsTemplates.Add(decalTemplate);
            Repaint();
            
            AssetDatabase.SaveAssets();
            RecreatePreviews();
        }

        public void AddTemplatesKit(DecalTemplatesKit kit)
        {
            Undo.RecordObject(kit, "Current kit changed");
            decalsTemplates.AddRange(kit.Templates);
            AssetDatabase.SaveAssets();
            RecreatePreviews();
        }

        public void AddTemplates(DecalTemplate[] templates)
        {
            Undo.RecordObject(kit, "Current kit changed");
            decalsTemplates.AddRange(templates);
            AssetDatabase.SaveAssets();
            RecreatePreviews();
        }

        public GPURaycastInfo GetFullscreenRaycastHitInfo(int x, int y)
        {
#if UNITY_EDITOR_OSX
            y -= (int)((float)fullScreenHeight / 2f);
            y *= 2;
            x *= 2;
#endif

            return fullScreenHits[y * fullScreenWidth + x];
        }

        public GPURaycastInfo GetFullscreenRaycastHitInfo(Vector2 pos)
        {
            return GetFullscreenRaycastHitInfo((int)pos.x, (int)pos.y);
        }

        public List<GPURaycastInfo> GetFullscreenRaycastHitInfoPixels(Vector2[] pixels)
        {
            List<GPURaycastInfo> hits = new List<GPURaycastInfo>();
            for (int i = 0; i < pixels.Length; i++)
            {
                hits.Add(GetFullscreenRaycastHitInfo(pixels[i]));
            }
            return hits;
        }

        private void OnFocus()
        {
            RecreatePreviews();
        }

        private void CheckPreviews()
        {
            foreach (var dt in decalsTemplates)
            {
                dt.CheckPreview();
            }
        }

        private void RecreatePreviews()
        {
            foreach (var dt in decalsTemplates)
            {
                dt.RecreatePreview();
            }
        }

        protected void OnGUI()
        {
            if (Event.current.type == EventType.ExecuteCommand)
            {
                if (Event.current.commandName.Equals("ObjectSelectorClosed"))
                {
                    Material selectedDecalMaterial = EditorGUIUtility.GetObjectPickerObject() as Material;
                    if (selectedDecalMaterial != null)
                    {
                        AddMaterialToTemplates(selectedDecalMaterial);
                    }
                    DecalTemplatesKit decalTemplateContainer = EditorGUIUtility.GetObjectPickerObject() as DecalTemplatesKit;
                    if(decalTemplateContainer != null)
                    {
                        AddTemplatesKit(decalTemplateContainer);
                    }
                }
            }

            if(selectedTemplates.Count > 0)
            {
                if(Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Delete)
                {
                    List<DecalTemplate> templatesToRemove = new List<DecalTemplate>();
                    foreach(var index in selectedTemplates)
                    {
                        templatesToRemove.Add(decalsTemplates[index]);
                    }
                    foreach(var decal in templatesToRemove)
                    {
                        decal.Destroy();

                        Undo.RecordObject(kit, "Current kit changed");
                        decalsTemplates.Remove(decal);
                        //DPTResourcesLoader.EditorKit.Templates = decalsTemplates.ToArray();
                        AssetDatabase.SaveAssets();
                    }
                    selectedTemplates.Clear();
                }
            }

            float linesXPaddingLeft = 5f;
            float linesXPaddingRight = 10f;
            float linesXPaddingTop = 5f;
            float linesXPaddingBottom = 5f;
            float height = 45;

            Rect logoRectLeftIcon = new Rect(linesXPaddingLeft, linesXPaddingTop, height - linesXPaddingBottom, height - linesXPaddingBottom);
            Rect lineRect = new Rect(linesXPaddingLeft, linesXPaddingTop, position.width - linesXPaddingRight, height - linesXPaddingBottom);

            Vector2 headerSize = new Vector2(DPTResourcesLoader.Header1.width, DPTResourcesLoader.Header1.height);
            headerSize /= 1.8f;

            Rect titleRect = new Rect(
                linesXPaddingLeft + position.width / 2 - headerSize.x / 2,
                linesXPaddingTop + height / 2 - 3 - headerSize.y / 2,
                headerSize.x,
                headerSize.y
                );

            GUI.DrawTexture(lineRect, DPTResourcesLoader.Header3);
            GUI.DrawTexture(logoRectLeftIcon, DPTResourcesLoader.Header2);
            GUI.DrawTexture(titleRect, DPTResourcesLoader.Header1);

            GUILayout.Space(height + linesXPaddingTop + linesXPaddingBottom);

            GUILayout.BeginHorizontal();
            GUIContent simpleContent = new GUIContent(DPTResourcesLoader.SimplePlacementIcon, "Simple");
            GUIContent burstContent = new GUIContent(DPTResourcesLoader.BurstPlacementIcon, "Burst");
            GUIContent paintingContent = new GUIContent(DPTResourcesLoader.PaintingPlacementIcon, "Painting");

            var skinSimple = Placing == PlacingType.OnePerClick ? DPTResourcesLoader.SelectedButtonStyle : DPTResourcesLoader.ButtonStyle;
            var skinBurst = Placing == PlacingType.Burst ? DPTResourcesLoader.SelectedButtonStyle : DPTResourcesLoader.ButtonStyle;
            var skinPainting = Placing == PlacingType.Painting ? DPTResourcesLoader.SelectedButtonStyle : DPTResourcesLoader.ButtonStyle;

            var buttonsMaxHeight = GUILayout.MaxHeight(34);

            if (GUILayout.Button(simpleContent, skinSimple, buttonsMaxHeight))
            {
                if (Placing == PlacingType.OnePerClick)
                {
                    Placing = PlacingType.None;
                }
                else
                {
                    Placing = PlacingType.OnePerClick;
                }
            }
            if (GUILayout.Button(burstContent, skinBurst, buttonsMaxHeight))
            {
                if (Placing == PlacingType.Burst)
                {
                    Placing = PlacingType.None;
                }
                else
                {
                    Placing = PlacingType.Burst;
                }
            }
            if (GUILayout.Button(paintingContent, skinPainting, buttonsMaxHeight))
            {
                if (Placing == PlacingType.Painting)
                {
                    Placing = PlacingType.None;
                }
                else
                {
                    Placing = PlacingType.Painting;
                }
            }
            GUILayout.EndHorizontal();

            OptionsEditor.OnInspectorGUI();

            if (Placing == PlacingType.Burst)
            {
                burstCount = EditorGUILayout.IntSlider("Burst count", burstCount, 1, 100);
                burstSize = EditorGUILayout.Slider("Burst Size", burstSize, 0, 3f);
            }

            rotation = EditorGUILayout.Slider("Rotation", rotation, 0, 360f);
            scale = EditorGUILayout.Slider("Scale", scale, 0.01f, 4f);

            GUIContent[] gridContent = decalsTemplates.ConvertAll((dt) => dt.ResultContent).ToArray();
            Color[] gridContentColors = decalsTemplates.ConvertAll((dt) => dt.InstancedColor).ToArray();

            decalsScroll = GUILayout.BeginScrollView(decalsScroll, true, true);
            int[] selectedMaterials = selectedTemplates.ToArray();

            GUILayout.Space(10);

            bool addButtonPressed;
            bool loadPressed;
            bool savePressed;
            bool clearPressed;

            int templatesXCount = Mathf.FloorToInt(((position.width - decalTemplateElementSize / 2) / decalTemplateElementSize));
            EditorGUI.BeginChangeCheck();
            int selectedID = MultiSelectionGrid.Draw(
                selectedMaterials,
                decalTemplateElementSize,
                templatesXCount,
                gridContent,
                gridContentColors,
                out addButtonPressed,
                "Load",
                "Save",
                "Clear",
                out loadPressed,
                out savePressed,
                out clearPressed
                );

            if(EditorGUI.EndChangeCheck())
            {
                RecreatePreviews();
                if(decalProjector != null)
                    decalProjector.GetComponent<Decal>().InstancedColor = selectedTemplate.InstancedColor;
            }

            if (loadPressed)
            {
                EditorGUIUtility.ShowObjectPicker<DecalTemplatesKit>(null, false, "", 0);
            }
            if (savePressed)
            {
                string path = EditorUtility.SaveFilePanelInProject("Select save path", "Decal Template Container.asset", "asset", "Please enter a file name to save the decals kit to");

                if (!string.IsNullOrEmpty(path))
                {
                    DecalTemplatesKit instance = ScriptableObject.CreateInstance<DecalTemplatesKit>();

                    List<DecalTemplate> savingTemplates = new List<DecalTemplate>();
                    for (int i = 0; i < selectedTemplates.Count; i++)
                    {
                        savingTemplates.Add(decalsTemplates[selectedTemplates[i]]);
                    }
                    instance.Templates = savingTemplates;

                    AssetDatabase.CreateAsset(instance, path);
                    AssetDatabase.Refresh();
                    AssetDatabase.SaveAssets();
                }
            }
            if (clearPressed)
            {
                decalsTemplates.Clear();
            }

            for (int i = 0; i < decalsTemplates.Count; i++)
            {
                decalsTemplates[i].InstancedColor = gridContentColors[i];
            }

            if(addButtonPressed)
            {
                EditorGUIUtility.ShowObjectPicker<Material>(null, false, "", 0);
            }
            GUILayout.Space(50);
            GUILayout.EndScrollView();

            if (selectedID != -1)
            {
                if (Event.current.button == 0)
                {
                    // left mouse click
                    if (Event.current.shift && selectedID != previousClickedIndex)
                    {
                        if (selectedTemplates.Count == 0)
                        {
                            if (selectedTemplates.Contains(selectedID))
                            {
                                selectedTemplates.Remove(selectedID);
                            }
                            else
                            {
                                selectedTemplates.Add(selectedID);
                            }
                        }
                        else
                        {
                            if (selectedID < previousClickedIndex)
                            {
                                for (int i = previousClickedIndex; i >= selectedID; i--)
                                {
                                    if (!selectedTemplates.Contains(i))
                                    {
                                        selectedTemplates.Add(i);
                                    }
                                }
                            }
                            else if (selectedID > previousClickedIndex)
                            {
                                for (int i = previousClickedIndex; i <= selectedID; i++)
                                {
                                    if (!selectedTemplates.Contains(i))
                                    {
                                        selectedTemplates.Add(i);
                                    }
                                }
                            }
                        }
                    }
                    else if (Event.current.control || Event.current.command)
                    {
                        if (selectedTemplates.Contains(selectedID))
                        {
                            selectedTemplates.Remove(selectedID);
                            previousClickedIndex = -1;
                        }
                        else
                        {
                            selectedTemplates.Add(selectedID);
                        }
                    }
                    else
                    {
                        if (selectedID == previousClickedIndex && EditorApplication.timeSinceStartup < lastDecalTemplateClickTime + decalDoubleClickTimestep)
                        {
                            Selection.activeObject = decalsTemplates[selectedID].DecalMaterial;
                        }

                        selectedTemplates.Clear();
                        if (!selectedTemplates.Contains(selectedID))
                        {
                            selectedTemplates.Add(selectedID);
                        }
                    }
                    lastDecalTemplateClickTime = EditorApplication.timeSinceStartup;

                    previousClickedIndex = selectedID;
                    RespawnDecalProjector();
                } else if(Event.current.button == 1)
                {
                    // right mouse click

                    GenericMenu menu = new GenericMenu();
                    var clickedDecal = decalsTemplates[selectedID];
                    menu.AddItem(new GUIContent("Atlas Editor"), false, () => SplitAtlas(clickedDecal));
                    menu.AddItem(new GUIContent("Select templates kit"), false, () => SelectKit(clickedDecal));
                    menu.ShowAsContext();
                }
            }

            // materials drag and drop
            if (Event.current.type == EventType.DragUpdated)
            {
                Object[] draggingObjects = DragAndDrop.objectReferences;
                bool hasMaterials = false;
                List<Material> draggingMaterials = new List<Material>();
                foreach (var dragObj in draggingObjects)
                {
                    Material mat = dragObj as Material;
                    if (mat != null)
                    {
                        hasMaterials = true;
                        draggingMaterials.Add(mat);
                    }
                }
                if (hasMaterials)
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
                    DragAndDrop.AcceptDrag();
                }
            }
            if (Event.current.type == EventType.DragPerform)
            {
                Object[] draggingObjects = DragAndDrop.objectReferences;
                List<Material> draggingMaterials = new List<Material>();
                foreach (var dragObj in draggingObjects)
                {
                    Material mat = dragObj as Material;
                    if(mat != null)
                        draggingMaterials.Add(mat);
                }
                foreach (var material in draggingMaterials)
                {
                    AddMaterialToTemplates(material);
                }
            }

            // kits drag and drop
            if (Event.current.type == EventType.DragUpdated)
            {
                Object[] draggingObjects = DragAndDrop.objectReferences;
                bool hasKits = false;
                List<DecalTemplatesKit> draggingKits = new List<DecalTemplatesKit>();
                foreach (var dragObj in draggingObjects)
                {
                    DecalTemplatesKit kit = dragObj as DecalTemplatesKit;
                    if (kit != null)
                    {
                        hasKits = true;
                        draggingKits.Add(kit);
                    }
                }
                if (hasKits)
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
                    DragAndDrop.AcceptDrag();
                }
            }
            if (Event.current.type == EventType.DragPerform)
            {
                Object[] draggingObjects = DragAndDrop.objectReferences;
                List<DecalTemplatesKit> draggingKits = new List<DecalTemplatesKit>();
                foreach (var dragObj in draggingObjects)
                {
                    DecalTemplatesKit kit = dragObj as DecalTemplatesKit;
                    if(kit != null)
                        draggingKits.Add(kit);
                }
                foreach (var kit in draggingKits)
                {
                    AddTemplatesKit(kit);
                }
            }
            Repaint();
        }

        private void SelectKit(DecalTemplate clickedDecal)
        {
            var kitsGUIDS = AssetDatabase.FindAssets("t:DecalTemplatesKit");
            for (int i = 0; i < kitsGUIDS.Length; i++)
            {
                var kit = AssetDatabase.LoadAssetAtPath<DecalTemplatesKit>(AssetDatabase.GUIDToAssetPath(kitsGUIDS[i]));
                if (kit != null)
                {
                    if(kit.Templates.Contains(clickedDecal))
                    {
                        Selection.activeObject = kit;
                        EditorGUIUtility.PingObject(kit);
                        break;
                    }
                }
            }
        }

        private void SplitAtlas(DecalTemplate template)
        {
            AtlasSplitter.Open(template, Splitted);
        }

        private void Splitted(DecalTemplate[] splittedTemplates)
        {
            AddTemplates(splittedTemplates);
        }

        private void OnSceneGUI(SceneView sceneView)
        {
            var mousePos = Event.current.mousePosition;
            var windowRect = new Rect(0, 0, sceneView.position.width, sceneView.position.height);

            if(Event.current.type == EventType.MouseEnterWindow)
            {
                isMouseOverWindow = true;
            }

            if(Event.current.type == EventType.MouseLeaveWindow)
            {
                isMouseOverWindow = false;
            }

            if (Placing != PlacingType.None)
            {
                Handles.BeginGUI();

                float rectWidth = 2;

                Rect rLeft = new Rect(0, 0, rectWidth, sceneView.position.height);
                Rect rRight = new Rect(sceneView.position.width - rectWidth, 0, rectWidth, sceneView.position.height);
                Rect rBottom = new Rect(0, sceneView.position.height - 18 - rectWidth, sceneView.position.width, rectWidth);
                Rect rTop = new Rect(0, 0, sceneView.position.width - rectWidth, rectWidth);

                GUI.color = defaultFrameColor;
                GUI.DrawTexture(rLeft, Texture2D.whiteTexture);
                GUI.DrawTexture(rRight, Texture2D.whiteTexture);
                GUI.DrawTexture(rBottom, Texture2D.whiteTexture);
                GUI.DrawTexture(rTop, Texture2D.whiteTexture);

                Handles.EndGUI();
            }

            mousePos = isMouseOverWindow ? mousePos : new Vector2(sceneView.camera.pixelWidth / 2, sceneView.camera.pixelHeight / 2);

            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                isMouseHold0 = true;
            }
            
            if (Placing == PlacingType.Painting)
            {
                // shift key down
                if (Event.current.shift)
                {
                    if (!oldShift)
                    {
                        if (isCameraDirty(sceneView.camera))
                        {
                            lastCameraData = new CameraData(sceneView.camera);
                            fullScreenHits = GPURaycast.FullscreenRaycast(sceneView.camera, 1f, out fullScreenWidth, out fullScreenHeight);
                        }
                    }
                }
            }
            oldShift = Event.current.shift;

            if (Event.current.type == EventType.MouseUp && Event.current.button == 0 || Event.current.type == EventType.MouseLeaveWindow)
            {
                isMouseHold0 = false;
                lastSpawnedDecal = null;
            }
            if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
            {
                isMouseHold1 = true;
            }

            if (Event.current.type == EventType.MouseUp && Event.current.button == 1 || Event.current.type == EventType.MouseLeaveWindow)
            {
                isMouseHold1 = false;
            }
            if (Event.current.type == EventType.MouseUp && Event.current.button == 0)
            {
                decalsLineList.Clear();
            }

            bool useEvent = false;
            if (Placing != PlacingType.None)
            {
                if (Event.current.control && isMouseHold0)
                {
                    scale -= Event.current.delta.y * 0.0035f;
                    scale = Mathf.Clamp(scale, 0.01f, 15f);
                    useEvent = true;
                }
                if (Event.current.control && isMouseHold1)
                {
                    rotation += Event.current.delta.y * 0.2f;
                    rotation = Mathf.Repeat(rotation, 360f);
                    useEvent = true;
                }
                if (useEvent)
                {
                    int controlId = GUIUtility.GetControlID(FocusType.Passive);
                    GUIUtility.hotControl = controlId;
                    Event.current.Use();
                }
                if (Event.current.type == EventType.ScrollWheel)
                {
                    useEvent = false;
                    if (Event.current.control)
                    {
                        scale -= Event.current.delta.y * 0.035f;
                        scale = Mathf.Clamp(scale, 0.01f, 15f);
                        useEvent = true;
                    }
                    if (Event.current.shift)
                    {
                        rotation += Event.current.delta.y * 2f;
                        rotation = Mathf.Repeat(rotation, 360f);
                        useEvent = true;
                    }
                    if (useEvent)
                    {
                        int controlId = GUIUtility.GetControlID(FocusType.Passive);
                        GUIUtility.hotControl = controlId;
                        Event.current.Use();
                    }
                }
            }

            Camera sceneCamera = sceneView.camera;

            GPURaycastInfo hitInfo;
            if (Placing != PlacingType.None && selectedTemplates.Count > 0)
            {
                Vector2 randomMouseOffset = randomOffset * Options.PositionJitter;
                bool isHitted = GPURaycast.Raycast(sceneCamera, mousePos + randomMouseOffset * burstPixelsPerSize, false, out hitInfo);
                if (decalProjector != null)
                {
                    if (isHitted)
                    {
                        decalProjector.SetActive(true);
                        decalProjector.GetComponent<Decal>().SortingOrder = Mathf.Max(sortingOrder, Options.SortingOrder);
                        if(useEvent)
                        {
                            ApplyTransformRotScale(decalProjector.transform, hitInfo.normal);
                        } else
                        {
                            ApplyTransform(decalProjector.transform, hitInfo.position, hitInfo.normal);
                        }
                    }
                    else
                    {
                        decalProjector.SetActive(false);
                    }
                }

                if (Placing == PlacingType.Burst)
                {
                    Handles.BeginGUI();

                    float burstImageSize = burstSize * burstPixelsPerSize;

                    Rect rect = new Rect(mousePos.x - burstImageSize, mousePos.y - burstImageSize, burstImageSize * 2, burstImageSize * 2);
                    GUI.DrawTexture(rect, burstCircle);

                    Handles.EndGUI();
                }

                if (Options.RotateToNext && Placing == PlacingType.Painting && !Options.LineAsOneDecal)
                {
                    if (lastSpawnedDecal != null)
                    {
                        Vector3 normal = lastSpawnedDecal.transform.up;
                        Vector3 direction = (hitInfo.position - lastSpawnedDecal.transform.position).normalized;
                        Vector3 localDirection = lastSpawnedDecal.transform.InverseTransformDirection(direction);
                        localDirection.y = 0;
                        localDirection.Normalize();
                        direction = lastSpawnedDecal.transform.TransformDirection(localDirection);
                        float angle = Vector3.SignedAngle(lastSpawnedDecal.transform.forward, direction, normal);

                        lastSpawnedDecal.transform.rotation *= Quaternion.AngleAxis(angle + rotation, Vector3.up);
                    }
                }


                bool shouldSpawn;

                float distance = Vector3.Distance(lastDecalPlacePositions, hitInfo.position);

                shouldSpawn = Event.current.type == EventType.MouseDown && (Placing == PlacingType.OnePerClick || Placing == PlacingType.Burst);
                shouldSpawn = shouldSpawn || (isMouseHold0 && Placing == PlacingType.Painting && distance > Options.MinPaintingDistance);
                shouldSpawn = shouldSpawn && decalProjector != null && isMouseOverWindow && !useEvent;

                if(Event.current.shift && Placing == PlacingType.Painting && decalProjector != null)
                {
                    if (isMouseHold0)
                    {
                        var hitCurrent = GetFullscreenRaycastHitInfo(mousePos);
                        isHitted = hitCurrent.IsHitted;

                        if (isHitted)
                        {
                            if (!hasStartLinePos)
                            {
                                startLinePos = mousePos;
                                hasStartLinePos = true;
                                lastSpawnedDecal = SpawnDecal();
                                ApplyTransformScale(lastSpawnedDecal.transform);
                                Undo.RegisterCreatedObjectUndo(lastSpawnedDecal, "Create decal");
                            }
                            else
                            {
                                isHitted = GetFullscreenRaycastHitInfo(mousePos).IsHitted;

                                if (isHitted)
                                {
                                    decalProjector.SetActive(false);
                                    if (Options.LineAsOneDecal)
                                    {
                                        Vector2 pixelStart = startLinePos;
                                        pixelStart.y = fullScreenHeight - pixelStart.y;

                                        Vector2 pixelEnd = mousePos;
                                        pixelEnd.y = fullScreenHeight - pixelEnd.y;

                                        if(Vector2.Distance(pixelStart, pixelEnd) < 5)
                                        {
                                            return;
                                        }

                                        var hitStart = GetFullscreenRaycastHitInfo(pixelStart);
                                        var hitEnd = GetFullscreenRaycastHitInfo(pixelEnd);

                                        Vector3 startLineWorldPos = hitStart.position;
                                        Vector3 endLineWorldPos = hitEnd.position;

                                        Vector3 normalAverage = ((hitStart.normal + hitEnd.normal) / 2).normalized;

                                        Vector3 direction = endLineWorldPos - startLineWorldPos;

                                        Transform decalTransform = lastSpawnedDecal.transform;
                                        decalTransform.position = startLineWorldPos + direction / 2 + normalAverage * Options.ProjectionOffset;
                                        decalTransform.rotation = Quaternion.LookRotation(direction.normalized, normalAverage);

                                        ApplyTransformScale(decalTransform);

                                        float scaleX = decalTransform.localScale.x;

                                        Vector3 currentDecalScale = decalTransform.localScale;
                                        currentDecalScale.z = direction.magnitude;
                                        decalTransform.localScale = currentDecalScale;

                                        var decalComponent = lastSpawnedDecal.GetComponent<Decal>();
                                        var uv = decalComponent.UV;
                                        uv.y = direction.magnitude / scaleX * Options.MinPaintingDistance;
                                        decalComponent.UV = uv;
                                    }
                                    else
                                    {
                                        float resolution = 1f;

                                        Vector2 direction = mousePos - startLinePos;

                                        float stepsCountToTargetPixel = direction.magnitude * resolution;

                                        List<Vector2> pixels = new List<Vector2>();
                                        for (int i = 0; i < stepsCountToTargetPixel; i++)
                                        {
                                            float fI = (float)i;
                                            float fraction = fI / stepsCountToTargetPixel;
                                            Vector2 pixel = Vector2.Lerp(startLinePos, mousePos, fraction);
                                            pixel.y = fullScreenHeight - pixel.y;
                                            pixels.Add(pixel);
                                        }

                                        List<GPURaycastInfo> hits = GetFullscreenRaycastHitInfoPixels(pixels.ToArray());

                                        List<GPURaycastInfo> actualHits = new List<GPURaycastInfo>();
                                        int lastUsedHit = 0;
                                        for (int i = 0; i < hits.Count; i++)
                                        {
                                            if (i > 0)
                                            {
                                                distance = Vector3.Distance(hits[lastUsedHit].position, hits[i].position);

                                                if (distance > Options.MinPaintingDistance)
                                                {
                                                    lastUsedHit = i;
                                                    actualHits.Add(hits[lastUsedHit]);
                                                }
                                            }
                                            else
                                            {
                                                actualHits.Add(hits[i]);
                                            }
                                        }

                                        hits = actualHits;

                                        while (decalsLineList.Count > hits.Count)
                                        {
                                            int lastIndex = decalsLineList.Count - 1;
                                            DestroyImmediate(decalsLineList[lastIndex]);
                                            decalsLineList.RemoveAt(lastIndex);
                                        }

                                        for (int i = 0; i < hits.Count; i++)
                                        {
                                            if (i >= decalsLineList.Count)
                                            {
                                                var instanceDecal = SpawnDecal();
                                                Undo.RegisterCreatedObjectUndo(instanceDecal, "Decals line");
                                                var decalTransform = instanceDecal.transform;
                                                ApplyTransform(decalTransform, hits[i].position, hits[i].normal);
                                                if (decalsLineList.Count >= 1)
                                                {
                                                    decalTransform.rotation = decalsLineList[decalsLineList.Count - 1].transform.rotation;
                                                }
                                                decalsLineList.Add(instanceDecal);
                                            }
                                            else
                                            {
                                                var decalTransform = decalsLineList[i].transform;
                                                ApplyTransform(decalTransform, hits[i].position, hits[i].normal);

                                                if (Options.RotateToNext)
                                                {
                                                    if (i < decalsLineList.Count - 1)
                                                    {
                                                        Vector3 normal = decalTransform.up;
                                                        Vector3 directionToNext = (hits[i + 1].position - decalTransform.position).normalized;
                                                        Vector3 localDirection = decalTransform.InverseTransformDirection(directionToNext);
                                                        localDirection.y = 0;
                                                        localDirection.Normalize();
                                                        directionToNext = decalTransform.TransformDirection(localDirection);
                                                        float angle = Vector3.SignedAngle(decalTransform.forward, directionToNext, normal);

                                                        decalTransform.rotation *= Quaternion.AngleAxis(angle + rotation, Vector3.up);
                                                    }
                                                    else if (i == decalsLineList.Count - 1)
                                                    {
                                                        if (decalsLineList.Count >= 2)
                                                        {
                                                            decalTransform.rotation = decalsLineList[decalsLineList.Count - 2].transform.rotation;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        hasStartLinePos = false;
                    }
                }
                else
                {
                    if (shouldSpawn && Event.current.button == 0)
                    {
                        int spawnCount = 1;
                        if (Placing == PlacingType.Burst)
                            spawnCount = burstCount;

                        for (int i = 0; i < spawnCount; i++)
                        {
                            Vector2 mouseOffsetRandom = Vector2.zero;

                            if (Placing == PlacingType.Burst)
                            {
                                mouseOffsetRandom = Random.insideUnitCircle * burstSize * burstPixelsPerSize;
                            }
                            isHitted = GPURaycast.Raycast(sceneCamera, mousePos + mouseOffsetRandom + randomMouseOffset * burstPixelsPerSize, RendererHitInfo, out hitInfo);
                            if (isHitted)
                            {
                                GameObject instance;
                                if (spawnCount == 1)
                                {
                                    instance = Instantiate(decalProjector);
                                    instance.transform.position = decalProjector.transform.position;
                                    instance.transform.rotation = decalProjector.transform.rotation;
                                    instance.transform.localScale = decalProjector.transform.localScale;
                                    instance.name = "Decal " + instance.GetComponent<Decal>().DecalMaterial.name;
                                    RespawnDecalProjector();
                                }
                                else
                                {
                                    instance = SpawnDecal();
                                    instance.name = "Decal " + instance.GetComponent<Decal>().DecalMaterial.name;
                                    ApplyTransform(instance.transform, hitInfo.position, hitInfo.normal);
                                }
                                Undo.RegisterCreatedObjectUndo(instance, "Decal placed");
                                if (Options.ParentToHittedRenderer)
                                {
                                    instance.transform.SetParent(hitInfo.hittedRenderer.transform);
                                }
                                randomOffset = Random.insideUnitCircle;
                                randomAngle = Random.Range(-1f, 1f);
                                randomScale = Random.Range(0f, 1f);
                                lastSpawnedDecal = instance;
                            }
                            lastDecalPlacePositions = hitInfo.position;
                        }
                    }
                    hasStartLinePos = false;
                    decalsLineList.Clear();
                }
                
                if (Event.current.button == 0 && !Event.current.isKey)
                {
                    if (Event.current.type == EventType.MouseDown || (isMouseHold0 && Event.current.type == EventType.MouseDrag))
                    {
                        int controlId = GUIUtility.GetControlID(FocusType.Passive);
                        GUIUtility.hotControl = controlId;
                        Event.current.Use();
                    }
                }
            }
            else
            {
                decalsLineList.Clear();
                if (decalProjector != null)
                    decalProjector.SetActive(false);
            }
        }

        void RespawnDecalProjector()
        {
            if (decalProjector != null)
                DestroyImmediate(decalProjector);

            int templateIndex;
            decalProjector = SpawnDecal(out templateIndex);
            selectedTemplate = decalsTemplates[templateIndex];
        }

        GameObject SpawnDecal()
        {
            if (selectedTemplates.Count == 0)
                return null;

            var template = decalsTemplates[selectedTemplates[Random.Range(0, selectedTemplates.Count)]];
            Material selectedMaterial = template.DecalMaterial;

            GameObject decal = new GameObject("Decal", typeof(Decal));
            var decalComponent = decal.GetComponent<Decal>();
            decalComponent.DecalMaterial = selectedMaterial;
            decalComponent.InstancedColor = template.InstancedColor;
            if (!decalComponent.DecalMaterial.enableInstancing)
                decalComponent.SortingOrder = sortingOrder;
            else
                decalComponent.SortingOrder = Options.SortingOrder;

            return decal;
        }

        GameObject SpawnDecal(out int templateIndex)
        {
            templateIndex = -1;
            if (selectedTemplates.Count == 0)
                return null;

            templateIndex = selectedTemplates[Random.Range(0, selectedTemplates.Count)];

            var template = decalsTemplates[templateIndex];
            Material selectedMaterial = template.DecalMaterial;

            GameObject decal = new GameObject("Decal", typeof(Decal));
            var decalComponent = decal.GetComponent<Decal>();
            decalComponent.DecalMaterial = selectedMaterial;
            decalComponent.InstancedColor = template.InstancedColor;
            decalComponent.UV = template.InstancedUV;
            if (!decalComponent.DecalMaterial.enableInstancing)
                decalComponent.SortingOrder = sortingOrder;
            else
                decalComponent.SortingOrder = Options.SortingOrder;

            return decal;
        }

        void ApplyTransform(Transform decalTransform, Vector3 worldPos, Vector3 worldNormal)
        {
            decalTransform.position = worldPos + worldNormal * Options.ProjectionOffset;

            decalTransform.localScale = Options.Size * (scale + randomScale * Options.SizeJitter);
            Vector3 currentScale = decalTransform.localScale;
            currentScale.y = Options.ProjectionDistance;
            decalTransform.localScale = currentScale;

            decalTransform.up = worldNormal;
            decalTransform.rotation *= Quaternion.AngleAxis(randomAngle * 180f * Options.RotationJitter + rotation, Vector3.up);
        }

        void ApplyTransformRotScale(Transform decalTransform, Vector3 worldNormal)
        {
            decalTransform.localScale = Options.Size * (scale + randomScale * Options.SizeJitter);
            Vector3 currentScale = decalTransform.localScale;
            currentScale.y = Options.ProjectionDistance;
            decalTransform.localScale = currentScale;

            decalTransform.up = worldNormal;
            decalTransform.rotation *= Quaternion.AngleAxis(randomAngle * 180f * Options.RotationJitter + rotation, Vector3.up);
        }

        void ApplyTransformScale(Transform decalTransform)
        {
            decalTransform.localScale = Options.Size * (scale + randomScale * Options.SizeJitter);
            Vector3 currentScale = decalTransform.localScale;
            currentScale.y = Options.ProjectionDistance;
            decalTransform.localScale = currentScale;
        }

        void ApplyTransformRaw(Transform decalTransform, Vector3 worldPos, Vector3 worldNormal)
        {
            decalTransform.position = worldPos + worldNormal * Options.ProjectionOffset;

            decalTransform.localScale = Options.Size * (scale);
            Vector3 currentScale = decalTransform.localScale;
            currentScale.y = Options.ProjectionDistance;
            decalTransform.localScale = currentScale;

            decalTransform.up = worldNormal;
            decalTransform.rotation *= Quaternion.AngleAxis(rotation, Vector3.up);
        }

        private void OnDestroy()
        {
            if(Options != null)
                DestroyImmediate(Options, true);

            // SceneView.onSceneGUIDelegate -= OnSceneGUI;
            SceneView.duringSceneGui -= OnSceneGUI;

            if (decalProjector != null)
                DestroyImmediate(decalProjector);
        }

        [System.Serializable]
        public class DecalTemplate
        {
            public Material DecalMaterial;
            [HideInInspector]
            public GUIContent ResultContent;
            [HideInInspector]
            [NonSerialized]
            public RenderTexture Preview;
            public Color InstancedColor = new Color(1,1,1,1);
            public Vector4 InstancedUV = new Vector4(1, 1, 0, 0);

            readonly Vector2 previewShadowOffset = new Vector2(1.2f, -1.2f);
            readonly float shadowOpacity = 2f;
            public readonly int PreviewSize;

            public DecalTemplate(Material mat, int previewSize)
            {
                this.PreviewSize = previewSize;
                DecalMaterial = mat;

                CreatePreview(DecalMaterial, previewSize);
            }

            public DecalTemplate()
            {
                PreviewSize = 100;
            }

            public void RecreatePreview()
            {
                if (Preview != null)
                    DestroyImmediate(Preview, true);

                CreatePreview(DecalMaterial, PreviewSize);
            }

            public void CheckPreview()
            {
                if (Preview == null)
                    CreatePreview(DecalMaterial, PreviewSize);
            }

            public void CreatePreview(Material mat, int previewSize)
            {
                DecalMaterial = mat;
                Preview = CreatePreview();

                ResultContent = new GUIContent(Preview);
            }

            public RenderTexture CreatePreview()
            {
                RenderTexture preview = new RenderTexture(PreviewSize, PreviewSize, 0, RenderTextureFormat.ARGB32);

                if (DecalMaterial == null)
                {
                    DPTResourcesLoader.BlitTexture(null, preview, DPTResourcesLoader.ShadowMaterial, 1);
                    return preview;
                }

                DPTResourcesLoader.ShadowMaterial.SetInt("_TextureSize", PreviewSize);
                DPTResourcesLoader.ShadowMaterial.SetVector("_ShadowOffset", previewShadowOffset);
                DPTResourcesLoader.ShadowMaterial.SetFloat("_ShadowOpacity", shadowOpacity);

                Vector2 scale = DecalMaterial.GetTextureScale("_MainTex") * new Vector2(InstancedUV.x, InstancedUV.y);
                Vector2 offset = DecalMaterial.GetTextureOffset("_MainTex") + new Vector2(InstancedUV.z, InstancedUV.w);

                Color mainColor = DecalMaterial.GetColor("_Color") * InstancedColor;

                DPTResourcesLoader.ShadowMaterial.SetVector("_Tiling", new Vector4(scale.x, scale.y, offset.x, offset.y));
                DPTResourcesLoader.ShadowMaterial.SetColor("_Color", mainColor);
                DPTResourcesLoader.ShadowMaterial.SetTexture("_NormalMap", DecalMaterial.GetTexture("_BumpMap"));
                DPTResourcesLoader.ShadowMaterial.SetTexture("_Emission", DecalMaterial.GetTexture("_EmissionMap"));
                DPTResourcesLoader.ShadowMaterial.SetVector("_EmissionColor", DecalMaterial.GetVector("_EmissionColor"));
                DPTResourcesLoader.ShadowMaterial.SetFloat("_NormalScale", DecalMaterial.GetFloat("_NormalScale"));

                if(mainColor.a == 0)
                {
                    DPTResourcesLoader.ShadowMaterial.SetInt("_NormalsOnly", 1);
                }
                else
                {
                    DPTResourcesLoader.ShadowMaterial.SetInt("_NormalsOnly", 0);
                }

                DPTResourcesLoader.BlitTexture(DecalMaterial.GetTexture("_MainTex"), preview, DPTResourcesLoader.ShadowMaterial, 0);
                return preview;
            }

            public void Destroy()
            {
                if (Preview != null)
                    DestroyImmediate(Preview, true);
            }
        }
    }
}