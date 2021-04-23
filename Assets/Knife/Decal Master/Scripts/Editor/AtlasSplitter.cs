using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Knife.Tools
{
    public class AtlasSplitter : EditorWindow
    {
        VisualRectsObject rectsContainer;
        DecalPlacementTool.DecalTemplate template;
        float zoom = 1f;

        Vector2 offset;
        Vector2 lastZoomPosition;

        Material BackgroundMaterial;
        Material TextureMaterial;

        float targetZoom = 1;
        float zoomVelocity;
        const float zoomSmoothTime = 4f;

        const float offsetSmoothTime = 4f;
        Vector2 targetOffset;
        Vector2 offsetVelocity;

        bool mouseHold;
        Vector2 startMouseHoldPosition;
        Vector2Int tiles; // for autosplit

        Color defaultFrameColor = new Color(0f, 0.7137255f, 0.9803922f, 1f);
        Color defaultOptionsRectColor = new Color(0.5f, 0.5f, 0.5f, 0.9f);

        List<VisualRect> visualRects
        {
            get
            {
                return rectsContainer.Rects;
            }
            set
            {
                rectsContainer.Rects = value;
            }
        }
        Vector2 _zoomCoordsOrigin;

        const float tabHeight = 23f;

        Matrix4x4 guiMatrix;
        Matrix4x4 zoomedMatrix;
        Rect backRect;

        VisualRect selectedVisualRect;
        Vector2 startDragDelta;

        Action<DecalPlacementTool.DecalTemplate[]> splittedCallback;

        public static void Open(DecalPlacementTool.DecalTemplate template, Action<DecalPlacementTool.DecalTemplate[]> splittedCallback)
        {
            AtlasSplitter window = GetWindow<AtlasSplitter>();
            window.template = template;
            window.Show();

            window.splittedCallback = splittedCallback;
            window.titleContent = new GUIContent("Split");
        }

        private void OnEnable()
        {
            BackgroundMaterial = new Material(DPTResourcesLoader.GUIShader);
            TextureMaterial = new Material(DPTResourcesLoader.GUIShader);
            offset = Vector2.zero;
            zoom = 1f;
            targetOffset = Vector2.zero;
            targetZoom = 1f;

            if (rectsContainer == null)
                rectsContainer = VisualRectsObject.CreateInstance<VisualRectsObject>();

            visualRects.Clear();
        }

        private void OnDestroy()
        {
            DestroyImmediate(BackgroundMaterial, true);
            DestroyImmediate(TextureMaterial, true);
        }

        bool CheckContent()
        {
            if (template == null)
            {
                EditorGUILayout.HelpBox("No template opened", MessageType.Error);
                return false;
            }

            if (template.DecalMaterial == null)
            {
                EditorGUILayout.HelpBox("No material on template asigned", MessageType.Error);
                return false;
            }

            if (template.DecalMaterial.mainTexture == null)
            {
                EditorGUILayout.HelpBox("No main texture asigned", MessageType.Error);
                return false;
            }

            return true;
        }

        void SetupMatrix()
        {
            guiMatrix = GUI.matrix;
            Vector3 position = guiMatrix.GetColumn(3);

            Quaternion rotation = Quaternion.LookRotation(
                guiMatrix.GetColumn(2),
                guiMatrix.GetColumn(1)
            );

            Vector3 scale = new Vector3(
                guiMatrix.GetColumn(0).magnitude,
                guiMatrix.GetColumn(1).magnitude,
                guiMatrix.GetColumn(2).magnitude
            );

            Vector3 newScale = Vector3.one * zoom;
            scale.x = newScale.x;
            scale.y = newScale.y;

            zoomedMatrix = Matrix4x4.TRS(position, rotation, scale);
        }

        void HandleEvents()
        {
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                mouseHold = true;
                startMouseHoldPosition = Event.current.mousePosition;
            }
            if (Event.current.type == EventType.MouseUp && Event.current.button == 0)
            {
                mouseHold = false;
            }

            if (Event.current.isScrollWheel)
            {
                float oldZoom = targetZoom;
                targetZoom += -Event.current.delta.y * 0.03f;
                targetZoom = Mathf.Clamp(targetZoom, 0.3f, 5f);

                lastZoomPosition = Event.current.mousePosition;

                Vector2 screenCoordsMousePos = Event.current.mousePosition;
                Vector2 zoomCoordsMousePos = (screenCoordsMousePos - new Vector2(backRect.xMin, backRect.yMin)) / targetZoom - targetOffset;

                targetOffset -= (zoomCoordsMousePos + targetOffset) - (oldZoom / targetZoom) * (zoomCoordsMousePos + targetOffset);
            }

            if (Event.current.type == EventType.MouseDrag && Event.current.button == 2)
            {
                targetOffset.x += Event.current.delta.x;
                targetOffset.y += Event.current.delta.y;

                targetOffset.x = Mathf.Clamp(targetOffset.x, -backRect.width * targetZoom, backRect.width * targetZoom);
                targetOffset.y = Mathf.Clamp(targetOffset.y, -backRect.height * targetZoom, backRect.height * targetZoom);
            }

            zoom = Mathf.SmoothDamp(zoom, targetZoom, ref zoomVelocity, zoomSmoothTime);

            offset = Vector2.SmoothDamp(offset, targetOffset, ref offsetVelocity, offsetSmoothTime);
        }

        void DrawCanvas(Rect fullRect)
        {
            EditorGUI.DrawRect(backRect, Color.black);

            GUI.EndGroup();

            Rect groupRect = backRect;
            groupRect.yMin += tabHeight / zoom;
            groupRect.yMax += tabHeight / zoom;
            groupRect.width /= zoom;
            groupRect.height /= zoom;
            backRect.center += offset;

            GUI.BeginGroup(groupRect);

            GUI.matrix = zoomedMatrix;

            GUI.DrawTextureWithTexCoords(backRect, DPTResourcesLoader.AlphaGrid, new Rect(0, 0, backRect.width / DPTResourcesLoader.AlphaGrid.width, backRect.height / DPTResourcesLoader.AlphaGrid.height));
            GUI.color = Color.black;
            GUI.DrawTextureWithTexCoords(backRect, template.DecalMaterial.mainTexture, new Rect(-0.005f, 0.005f, 1, 1));
            GUI.color = Color.white;
            GUI.DrawTextureWithTexCoords(backRect, template.DecalMaterial.mainTexture, new Rect(0, 0, 1, 1));

            GUI.matrix = guiMatrix;

            GUI.EndGroup();
            GUI.BeginGroup(fullRect);

            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                selectedVisualRect = null;
            }
            if(Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Delete)
            {
                Undo.RecordObject(rectsContainer, "Delete rect");
                if(selectedVisualRect != null)
                    visualRects.Remove(selectedVisualRect);
            }
            for (int i = 0; i < visualRects.Count; i++)
            {

                Vector2 tabOffset = new Vector4(0, tabHeight);
                Rect r = visualRects[i].Rect;
                r.min += offset - tabOffset;
                r.max += offset - tabOffset;

                r.min += tabOffset;
                r.max += tabOffset;
                r.min *= zoom;
                r.max *= zoom;

                //if (r.height < 0)
                //{
                //    r.center += r.height * Vector2.up;
                //    r.height = Mathf.Abs(r.height);
                //}

                //if (r.width < 0)
                //{
                //    r.center += r.width * Vector2.right;
                //    r.width = Mathf.Abs(r.width);
                //}

                if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
                {
                    var mouseIN = r.Contains(Event.current.mousePosition);

                    if (mouseIN)
                    {
                        selectedVisualRect = visualRects[i];
                    }
                }

                Color outlineColor = defaultFrameColor;
                if (selectedVisualRect == visualRects[i])
                {
                    outlineColor.a = 1f;
                }
                else
                {
                    outlineColor.a = 1f;
                }
                Color faceColor = defaultFrameColor;
                if (selectedVisualRect == visualRects[i])
                {
                    faceColor.a *= 0.1f;
                }
                else
                {
                    faceColor.a *= 0.05f;
                }

                Handles.DrawSolidRectangleWithOutline(r, faceColor, Color.clear);
                Vector3[] points = new Vector3[5];
                points[0] = r.min;
                points[1] = new Vector3(r.min.x, r.max.y);
                points[2] = r.max;
                points[3] = new Vector3(r.max.x, r.min.y);
                points[4] = r.min;
                Handles.color = outlineColor;
                Handles.DrawAAPolyLine(5f, 5, points);
            }
        }

        void DrawSelectionRectangle(Rect backRect)
        {
            if (mouseHold)
            {
                Vector2 rectScale = Event.current.mousePosition - startMouseHoldPosition;
                Rect selectionRect = new Rect(startMouseHoldPosition, rectScale);

                var min = selectionRect.min;
                var max = selectionRect.max;

                min.x = Mathf.Clamp(min.x, 0 + offset.x * zoom, (backRect.width + offset.x) * zoom);
                min.y = Mathf.Clamp(min.y, 0 + offset.y * zoom, (backRect.height + offset.y) * zoom);

                max.x = Mathf.Clamp(max.x, 0 + offset.x * zoom, (backRect.width + offset.x) * zoom);
                max.y = Mathf.Clamp(max.y, 0 + offset.y * zoom, (backRect.height + offset.y) * zoom);

                selectionRect.min = min;
                selectionRect.max = max;


                Color faceColor = defaultFrameColor;
                faceColor.a *= 0.2f;

                Handles.DrawSolidRectangleWithOutline(selectionRect, faceColor, defaultFrameColor);
            }
        }
        
        void CreateRectangle(Rect backRect)
        {
            if (Event.current.type == EventType.MouseUp && Event.current.button == 0)
            {
                Vector2 rectScale = Event.current.mousePosition - startMouseHoldPosition;

                if (Mathf.Abs(rectScale.y) > 5 && Mathf.Abs(rectScale.x) > 5)
                {
                    Rect selectionRect = new Rect(startMouseHoldPosition, rectScale);

                    var min = selectionRect.min;
                    var max = selectionRect.max;

                    min.x = Mathf.Clamp(min.x, 0 + offset.x * zoom, (backRect.width + offset.x) * zoom);
                    min.y = Mathf.Clamp(min.y, 0 + offset.y * zoom, (backRect.height + offset.y) * zoom);

                    max.x = Mathf.Clamp(max.x, 0 + offset.x * zoom, (backRect.width + offset.x) * zoom);
                    max.y = Mathf.Clamp(max.y, 0 + offset.y * zoom, (backRect.height + offset.y) * zoom);

                    selectionRect.min = min / zoom;
                    selectionRect.max = max / zoom;

                    selectionRect.center -= offset;

                    Rect r = selectionRect;

                    if (r.height < 0)
                    {
                        r.center += r.height * Vector2.up;
                        r.height = Mathf.Abs(r.height);
                    }

                    if (r.width < 0)
                    {
                        r.center += r.width * Vector2.right;
                        r.width = Mathf.Abs(r.width);
                    }
                    selectionRect = r;

                    var visualRect = new VisualRect();
                    visualRect.Rect = selectionRect;

                    Undo.RecordObject(rectsContainer, "Create rect");
                    visualRects.Add(visualRect);
                }
            }
        }

        private void OnGUI()
        {
            if (!CheckContent())
                return;

            Rect pos = this.position;
            backRect = new Rect(0, 0, pos.width, pos.height);
            Rect originalRect = new Rect(backRect);

            HandleEvents();
            SetupMatrix();
            originalRect.y += tabHeight;
            DrawCanvas(originalRect);
            if (selectedVisualRect == null)
            {
                DrawSelectionRectangle(backRect);
                CreateRectangle(backRect);
            }
            else
            {
                MoveSelected();
            }

            originalRect.y -= tabHeight;
            DrawRectOptions(originalRect);
            DrawOptions(originalRect);

            Repaint();
        }

        private void DrawRectOptions(Rect fullRect)
        {
            Rect optionsAreaRect = fullRect;
            optionsAreaRect.min = optionsAreaRect.max - Vector2.one * 200f;

            if (selectedVisualRect != null)
            {
                EditorGUI.DrawRect(optionsAreaRect, defaultOptionsRectColor);
                GUILayout.BeginArea(optionsAreaRect);
                EditorGUILayout.BeginVertical();

                selectedVisualRect.Rect.x = EditorGUILayout.IntField("X", (int)selectedVisualRect.Rect.x);
                selectedVisualRect.Rect.y = EditorGUILayout.IntField("Y", (int)selectedVisualRect.Rect.y);

                selectedVisualRect.Rect.width = EditorGUILayout.IntField("W", (int)selectedVisualRect.Rect.width);
                selectedVisualRect.Rect.height = EditorGUILayout.IntField("H", (int)selectedVisualRect.Rect.height);

                EditorGUILayout.EndVertical();
                GUILayout.EndArea();
            }
        }

        private void DrawOptions(Rect fullRect)
        {
            Rect optionsAreaRect = fullRect;
            optionsAreaRect.max = optionsAreaRect.min + Vector2.one * 200f;

            EditorGUI.DrawRect(optionsAreaRect, defaultOptionsRectColor);
            GUILayout.BeginArea(optionsAreaRect);
            EditorGUILayout.BeginVertical();

            if (GUILayout.Button("SPLIT"))
            {
                Split();
            }

            tiles = EditorGUILayout.Vector2IntField("Tiles", tiles);

            if (GUILayout.Button("AUTO SPLIT"))
            {
                AutoSplit();
            }

            EditorGUILayout.EndVertical();
            GUILayout.EndArea();
        }

        private void AutoSplit()
        {
            Undo.RecordObject(rectsContainer, "Auto split");
            visualRects.Clear();
            Vector2 tileSize = new Vector2((float)backRect.width / tiles.x, (float)backRect.height / tiles.y);
            for (int x = 0; x < tiles.x; x++)
            {
                for (int y = 0; y < tiles.y; y++)
                {
                    Rect tileRect = new Rect(Vector2.Scale(tileSize, new Vector2(x, y)), tileSize);
                    var vr = new VisualRect();
                    vr.Rect = tileRect;
                    visualRects.Add(vr);
                }
            }
        }

        private void Split()
        {
            List<DecalPlacementTool.DecalTemplate> templates = new List<DecalPlacementTool.DecalTemplate>();
            foreach(var rect in visualRects)
            {
                Vector2 tiling = Vector2.zero;
                Vector2 offset = Vector2.zero;
                tiling.x = rect.Rect.width / backRect.width;
                tiling.y = rect.Rect.height / backRect.height;
                offset.x = rect.Rect.x / backRect.width;
                offset.y = rect.Rect.y / backRect.height;

                var templateInstance = new DecalPlacementTool.DecalTemplate(template.DecalMaterial, 100);
                templateInstance.InstancedUV = new Vector4(tiling.x, tiling.y, offset.x, offset.y);
                templateInstance.RecreatePreview();

                templates.Add(templateInstance);
            }

            splittedCallback(templates.ToArray());
            Close();
        }

        private void MoveSelected()
        {
            if (mouseHold)
            {
                if(Event.current.type == EventType.MouseDown)
                {
                    Vector2 rectWorldCenter = selectedVisualRect.Rect.center * zoom + offset;

                    startDragDelta = Event.current.mousePosition - rectWorldCenter;
                    Undo.RecordObject(rectsContainer, "Drag rect");
                }
                selectedVisualRect.Rect.center = (Event.current.mousePosition - startDragDelta - offset) / zoom;
            }
        }

        [System.Serializable]
        public class VisualRect
        {
            public Rect Rect;
        }

        public class VisualRectsObject : ScriptableObject
        {
            public List<VisualRect> Rects = new List<VisualRect>();
        }
    }
}