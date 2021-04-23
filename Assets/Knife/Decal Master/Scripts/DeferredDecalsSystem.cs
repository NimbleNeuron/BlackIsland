using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using System.Linq;

namespace Knife.DeferredDecals
{
    [ExecuteInEditMode]
    public class DeferredDecalsSystem : MonoBehaviour
    {
        private Dictionary<Camera, CommandBuffer> m_Cameras = new Dictionary<Camera, CommandBuffer>();
        private Dictionary<Camera, CommandBuffer> m_CamerasSceneview = new Dictionary<Camera, CommandBuffer>();
        private Dictionary<Decal, bool> currentDecalsVisibility = new Dictionary<Decal, bool>();
        private Dictionary<Camera, RenderTexture> m_ExclusionMasks = new Dictionary<Camera, RenderTexture>();
        private CommandBuffer customRenderBuffer;
        Plane[] cameraPlanes;

        public bool LockRebuild;
        public TerrainDecalsType TerrainDecals;
        public int TerrainHeightMapSize = 1024;
        public bool UseExclusionMask;
        public LayerMask ExclusionMask;
        public bool FrustumCulling = true;
        public bool DistanceCulling = true;
        public float StartFadeDistance = 50;
        public float FadeLength = 2;
        public static bool DrawDecalGizmos = true;

        public enum TerrainDecalsType
        {
            None,
            OneTerrain,
            MultiTerrain
        }

        [SerializeField] private Mesh cubeMesh = default;
        Mesh cubeMeshCopy;

        public Texture2D[] terrainsTextures;
        TerrainData[] terrainsData;

        ComputeBuffer terrainsDataBuffer;
        HeightmapsCollector heightmapsCollector;
        CommandBuffer copyHeightmapsBuffer;

        Dictionary<Decal, FloatValue> lastSelectedDecals = new Dictionary<Decal, FloatValue>();
        const float selectionDuration = 1f;
        readonly AnimationCurve selectionFadeCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);
        List<Decal> decalsToRemove = new List<Decal>(); // editor only for selection
        double lastRenderTime;

        [SerializeField]
        Shader specularSmoothnessBlitterShader;
        Material specularSmoothnessBlitterMaterial;
        static Material defaultDecalMaterial;

        MaterialPropertyBlock instancedBlock;
        Dictionary<DecalsGroup, List<Decal>> decalsGrouped = new Dictionary<DecalsGroup, List<Decal>>();
        Matrix4x4[] matrices = new Matrix4x4[1023];
        Vector4[] tintArray = new Vector4[1023];
        Vector4[] uvArray = new Vector4[1023];

        public struct DecalsGroup
        {
            public Material Material;
            public int SortingOrder;
            public bool Instancing;
        }

        public class FloatValue
        {
            public float Value;
        }

        public Mesh CubeMesh
        {
            get
            {
                if (cubeMeshCopy == null)
                    CreateCubeMeshCopy(); 

                return cubeMeshCopy;
            }
        }

        public static Material DefaultDecalMaterial
        {
            get
            {
                if (defaultDecalMaterial == null)
                {
                    defaultDecalMaterial = Resources.Load<Material>("Knife/Deferred Decals/DefaultDecalMaterial");
                }

                return defaultDecalMaterial;
            }
        }

        public CommandBuffer CustomRenderBuffer
        {
            get
            {
                if (customRenderBuffer == null)
                {
                    customRenderBuffer = new CommandBuffer();
                    customRenderBuffer.name = cmdCustomRenderBufferName;
                }

                return customRenderBuffer;
            }

            set
            {
                customRenderBuffer = value;
            }
        }
        readonly string cmdCustomRenderBufferName = "[Decal Master] Custom Render Buffer";

        Shader depthShader;
        RenderTexture exclusionMaskRenderTarget;
        Camera exclusionCamera;

        private void Start()
        {
            DeferredDecalsSystem[] systems = GameObject.FindObjectsOfType<DeferredDecalsSystem>();

            if(systems.Length > 1)
            {
                Debug.LogError("Must be only one deferred decals system");
                Disable();
            }
        }

        private void OnEnable()
        {
#if UNITY_EDITOR
            DrawDecalGizmos = UnityEditor.EditorPrefs.GetBool("DM_DrawDecalGizmos");
#endif

            Enable();
        }


        private void OnDisable()
        {
            Disable();
        }

        public void Enable()
        {
            //specularSmoothnessBlitterShader = Shader.Find("Hidden/Knife/SpecSmoothnessBlitter");
            specularSmoothnessBlitterShader = Resources.Load<Shader>("Knife/Deferred Decals/SpecSmoothnessBlitter");
            specularSmoothnessBlitterMaterial = new Material(specularSmoothnessBlitterShader);
            Camera.onPreRender += Render;
#if UNITY_EDITOR
            UnityEditor.Selection.selectionChanged += OnSelectionChanged;
#endif
            instancedBlock = new MaterialPropertyBlock();

            //depthShader = Shader.Find("Hidden/DepthToTarget");
            depthShader = Resources.Load<Shader>("Knife/Deferred Decals/DepthToTarget");

            if (exclusionCamera != null)
            {
                DestroyImmediate(exclusionCamera.gameObject);
            }

            exclusionCamera = new GameObject("Exclusion camera", typeof(Camera)).GetComponent<Camera>();
            exclusionCamera.gameObject.hideFlags = HideFlags.HideAndDontSave;
            exclusionCamera.enabled = false;
            exclusionCamera.AddCommandBuffer(CameraEvent.AfterEverything, CustomRenderBuffer);
            CustomRendererManager.instance.SetRenderBuffer(CustomRenderBuffer, new Material(depthShader));
        }

#if UNITY_EDITOR
        void OnSelectionChanged()
        {
            var selectedObjects = UnityEditor.Selection.GetFiltered<Decal>(UnityEditor.SelectionMode.Unfiltered).ToList();

            List<Decal> decalsToRemove = new List<Decal>();

            foreach (var selectedDecal in lastSelectedDecals.Keys)
            {
                if(!selectedObjects.Contains(selectedDecal))
                {
                    decalsToRemove.Add(selectedDecal);
                }
            }

            foreach(var decal in decalsToRemove)
            {
                lastSelectedDecals.Remove(decal);
            }

            foreach(var selectedDecal in selectedObjects)
            {
                if(!lastSelectedDecals.ContainsKey(selectedDecal))
                {
                    lastSelectedDecals.Add(selectedDecal, new FloatValue());
                }
            }
        }
#endif

        private void CreateCubeMeshCopy()
        {
            cubeMeshCopy = Instantiate(cubeMesh);

            Vector3[] vertices = cubeMeshCopy.vertices;
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] += Vector3.up * -0.5f;
            }
            cubeMeshCopy.vertices = vertices;
            cubeMeshCopy.UploadMeshData(true);
        }

        public void Disable()
        {
            if (specularSmoothnessBlitterMaterial != null)
                DestroyImmediate(specularSmoothnessBlitterMaterial, true);
            foreach (var camBuffer in m_Cameras)
            {
                // key - camera
                // value - buffer

                if (camBuffer.Key == null)
                    continue;
                
                camBuffer.Key.RemoveCommandBuffer(CameraEvent.BeforeReflections, camBuffer.Value);
            }
            foreach(var camBuffer in m_CamerasSceneview)
            {
                if (camBuffer.Key == null)
                    continue;

                camBuffer.Key.RemoveCommandBuffer(CameraEvent.AfterLighting, camBuffer.Value);
            }
            foreach(var kv in m_ExclusionMasks)
            {
                DestroyImmediate(kv.Value, true);
            }
            m_ExclusionMasks.Clear();
            m_CamerasSceneview.Clear();
            m_Cameras.Clear();
            Camera.onPreRender -= Render;
        }

        public void Render(Camera camera)
        {
            var cam = camera;
            if (!cam)
            {
                return;
            }

            // before lighting buffer
            CommandBuffer bufferBeforeLighting = null;
            if (m_Cameras.ContainsKey(cam))
            {
                bufferBeforeLighting = m_Cameras[cam];
                if (LockRebuild)
                    return;
                bufferBeforeLighting.Clear();
            }
            else
            {
                bufferBeforeLighting = new CommandBuffer();
                bufferBeforeLighting.name = "Deferred decals";
                m_Cameras[cam] = bufferBeforeLighting;

                // set this command buffer to be executed just before deferred lighting pass
                // in the camera
                var buffers = cam.GetCommandBuffers(CameraEvent.BeforeReflections);

                for (int i = 0; i < buffers.Length; i++)
                {
                    if (buffers[i].name.Equals("Deferred decals"))
                    {
                        cam.RemoveCommandBuffer(CameraEvent.BeforeReflections, buffers[i]);
                    }
                }

                cam.AddCommandBuffer(CameraEvent.BeforeReflections, bufferBeforeLighting);
            }

            if (UseExclusionMask)
            {
                if (!m_ExclusionMasks.TryGetValue(cam, out exclusionMaskRenderTarget))
                {
                    exclusionMaskRenderTarget = new RenderTexture(cam.pixelWidth, cam.pixelHeight, 16, RenderTextureFormat.RFloat, RenderTextureReadWrite.Linear);
                    exclusionMaskRenderTarget.filterMode = FilterMode.Point;
                    exclusionMaskRenderTarget.Create();
                    exclusionMaskRenderTarget.name = "Exclusion Mask for camera " + cam.name;
                    m_ExclusionMasks.Add(cam, exclusionMaskRenderTarget);
                }
                if (exclusionMaskRenderTarget != null && (cam.pixelWidth != exclusionMaskRenderTarget.width || cam.pixelHeight != exclusionMaskRenderTarget.height))
                {
                    m_ExclusionMasks.Remove(cam);
                    DestroyImmediate(exclusionMaskRenderTarget, true);

                    exclusionMaskRenderTarget = new RenderTexture(cam.pixelWidth, cam.pixelHeight, 16, RenderTextureFormat.RFloat, RenderTextureReadWrite.Linear);
                    exclusionMaskRenderTarget.filterMode = FilterMode.Point;
                    exclusionMaskRenderTarget.Create();
                    exclusionMaskRenderTarget.name = "Exclusion Mask for camera " + cam.name;
                    m_ExclusionMasks.Add(cam, exclusionMaskRenderTarget);
                }

                exclusionCamera.CopyFrom(cam);
                exclusionCamera.depth = cam.depth - 0.01f;
                exclusionCamera.transform.position = cam.transform.position;
                exclusionCamera.transform.rotation = cam.transform.rotation;
                exclusionCamera.cullingMask = ExclusionMask;
                exclusionCamera.targetTexture = exclusionMaskRenderTarget;
                exclusionCamera.renderingPath = RenderingPath.Forward;
                exclusionCamera.clearFlags = CameraClearFlags.Color;
                exclusionCamera.backgroundColor = Color.clear;
                exclusionCamera.RenderWithShader(depthShader, "RenderType");

                bufferBeforeLighting.EnableShaderKeyword("EXCLUSIONMASK");

                bufferBeforeLighting.SetGlobalTexture("_ExclusionMask", exclusionMaskRenderTarget);
            }
            else
            {
                bufferBeforeLighting.DisableShaderKeyword("EXCLUSIONMASK");
            }

            bool isSceneViewCamera = cam.cameraType == CameraType.SceneView;
            CommandBuffer selectionBuffer = null;
            if(m_CamerasSceneview.ContainsKey(cam))
            {
                selectionBuffer = m_CamerasSceneview[cam];
                selectionBuffer.Clear();
            }
            else
            {
                if(isSceneViewCamera)
                {
                    selectionBuffer = new CommandBuffer();
                    selectionBuffer.name = "Deferred decals selection";
                    m_CamerasSceneview[cam] = selectionBuffer;

                    // set this command buffer to be executed just before deferred lighting pass
                    // in the camera
                    var buffers = cam.GetCommandBuffers(CameraEvent.AfterLighting);

                    for (int i = 0; i < buffers.Length; i++)
                    {
                        if (buffers[i].name.Equals("Deferred decals selection"))
                        {
                            cam.RemoveCommandBuffer(CameraEvent.AfterLighting, buffers[i]);
                        }
                    }

                    cam.AddCommandBuffer(CameraEvent.AfterLighting, selectionBuffer);
                }
            }

            // frustum culling
            cameraPlanes = GeometryUtility.CalculateFrustumPlanes(camera);
            currentDecalsVisibility.Clear();
            var system = DeferredDecalsManager.instance;

            // cache visibility for each decal
            if(FrustumCulling)
            {
                foreach (var decal in system.m_Decals)
                {
                    if (decal == null)
                        continue;

                    SetupBounds(decal);
                    bool isDecalVisible = IsBoundsVisible(cameraPlanes, decal.Bounds);
                    currentDecalsVisibility.Add(decal, isDecalVisible);
                }
            } else
            {
                foreach (var decal in system.m_Decals)
                {
                    if (decal == null)
                        continue;

                    currentDecalsVisibility.Add(decal, true);
                }
            }

            BuiltinRenderTextureType emissionTexture = cam.allowHDR ? BuiltinRenderTextureType.CameraTarget : BuiltinRenderTextureType.GBuffer3;

            // copy g-buffer normals into a temporary RT
            var normalsID = Shader.PropertyToID("_NormalsCopy");
            bufferBeforeLighting.GetTemporaryRT(normalsID, -1, -1, 0, FilterMode.Bilinear, RenderTextureFormat.ARGB32);
            bufferBeforeLighting.Blit(BuiltinRenderTextureType.GBuffer2, normalsID);

            // copy g-buffer specrough into a temporary RT
            var specularID = Shader.PropertyToID("_SpecularTarget");
            bufferBeforeLighting.GetTemporaryRT(specularID, -1, -1, 0, FilterMode.Bilinear, RenderTextureFormat.ARGB32);
            bufferBeforeLighting.Blit(BuiltinRenderTextureType.GBuffer1, specularID);

            // copy g-buffer specrough into a temporary RT
            var smoothnessID = Shader.PropertyToID("_SmoothnessTarget");
            bufferBeforeLighting.GetTemporaryRT(smoothnessID, -1, -1, 0, FilterMode.Bilinear, RenderTextureFormat.ARGB32);
            bufferBeforeLighting.Blit(BuiltinRenderTextureType.GBuffer1, smoothnessID, specularSmoothnessBlitterMaterial, 0);

            // copy g-buffer camera target into a temporary RT
            var emissionID = Shader.PropertyToID("_CameraTargetCopy");
            bufferBeforeLighting.GetTemporaryRT(emissionID, -1, -1, 0, FilterMode.Bilinear, RenderTextureFormat.ARGB32);
            bufferBeforeLighting.Blit(emissionTexture, emissionID);

            instancedBlock.Clear();

            var sortedDecalsBySortingOrder = system.m_Decals;

            decalsGrouped.Clear();

            DecalsGroup groupKey;
            Vector3 camPos = cam.transform.position;
            float currentDistanceToCamera;
            float fadeFactor;
            foreach (var decal in sortedDecalsBySortingOrder)
            {
                if (!currentDecalsVisibility[decal])
                    continue;
                if (DistanceCulling)
                {
                    currentDistanceToCamera = Vector3.Distance(camPos, decal.transform.position);

                    if(currentDistanceToCamera >= StartFadeDistance + FadeLength)
                    {
                        continue;
                    }

                    fadeFactor = Mathf.InverseLerp(StartFadeDistance, StartFadeDistance + FadeLength, currentDistanceToCamera);
                    decal.DistanceFade = 1 - fadeFactor;
                }
                else
                {
                    decal.DistanceFade = 1f;
                }

                groupKey.Material = decal.DecalMaterial;
                groupKey.SortingOrder = decal.SortingOrder;
                groupKey.Instancing = decal.DecalMaterial.enableInstancing;
                List<Decal> decalsByMaterials;
                if(!decalsGrouped.TryGetValue(groupKey, out decalsByMaterials))
                {
                    decalsByMaterials = new List<Decal>();
                    decalsGrouped.Add(groupKey, decalsByMaterials);
                }
                decalsByMaterials.Add(decal);
            }

            RenderTargetIdentifier[] mrtDifNormSpecRough = { BuiltinRenderTextureType.GBuffer0, BuiltinRenderTextureType.GBuffer2, emissionTexture };
            bufferBeforeLighting.SetRenderTarget(mrtDifNormSpecRough, BuiltinRenderTextureType.CameraTarget);
            Color tint;
            Vector4 uv;
            Vector4 uvOne = new Vector4(1, 1, 0, 0);
            foreach (var decalGroup in decalsGrouped)
            {
                var decalsList = decalGroup.Value;
                if (decalGroup.Key.Instancing)
                {
                    bufferBeforeLighting.SetGlobalColor("_NotInstancedColor", Color.white);
                    bufferBeforeLighting.SetGlobalVector("_NotInstancedUV", uvOne);
                    int batchCount = 1 + Mathf.FloorToInt(decalsList.Count / 1024);
                    for (int i = 0; i < batchCount; i++)
                    {
                        int drawCount = Mathf.Min(1023, decalsList.Count);
                        for (int j = 0; j < drawCount; j++)
                        {
                            tintArray[j] = decalsList[j].InstancedColor;
                            tintArray[j].w *= decalsList[j].Fade * decalsList[j].DistanceFade;
                            uvArray[j] = decalsList[j].UV;
                            matrices[j] = decalsList[j].transform.localToWorldMatrix;
                        }
                        if (drawCount > 0)
                        {
                            instancedBlock.SetVectorArray("_Tint", tintArray);
                            instancedBlock.SetVectorArray("_UV", uvArray);
                            bufferBeforeLighting.DrawMeshInstanced(CubeMesh, 0, decalGroup.Key.Material, 0, matrices, drawCount, instancedBlock);
                        }
                    }
                }
                else
                {
                    foreach (var decal in decalsList)
                    {
                        tint = decal.InstancedColor;
                        tint.a *= decal.Fade * decal.DistanceFade;
                        bufferBeforeLighting.SetGlobalColor("_NotInstancedColor", tint);
                        uv = decal.UV;
                        bufferBeforeLighting.SetGlobalVector("_NotInstancedUV", uv);
                        bufferBeforeLighting.DrawMesh(CubeMesh, decal.transform.localToWorldMatrix, decal.DecalMaterial, 0, 0);
                    }
                }
            }

            mrtDifNormSpecRough = new RenderTargetIdentifier[] { specularID, smoothnessID };
            bufferBeforeLighting.SetRenderTarget(mrtDifNormSpecRough, BuiltinRenderTextureType.CameraTarget);

            foreach (var decalGroup in decalsGrouped)
            {
                var decalsList = decalGroup.Value;
                if (decalGroup.Key.Instancing)
                {
                    bufferBeforeLighting.SetGlobalColor("_NotInstancedColor", Color.white);
                    bufferBeforeLighting.SetGlobalVector("_NotInstancedUV", uvOne);
                    int batchCount = 1 + Mathf.FloorToInt(decalsList.Count / 1024);
                    for (int i = 0; i < batchCount; i++)
                    {
                        int drawCount = Mathf.Min(1023, decalsList.Count);
                        for (int j = 0; j < drawCount; j++)
                        {
                            tintArray[j] = decalsList[j].InstancedColor;
                            tintArray[j].w *= decalsList[j].Fade;
                            uvArray[j] = decalsList[j].UV;
                            matrices[j] = decalsList[j].transform.localToWorldMatrix;
                        }
                        if (drawCount > 0)
                        {
                            instancedBlock.SetVectorArray("_Tint", tintArray);
                            instancedBlock.SetVectorArray("_UV", uvArray);
                            bufferBeforeLighting.DrawMeshInstanced(CubeMesh, 0, decalGroup.Key.Material, 2, matrices, drawCount, instancedBlock);
                        }
                    }
                }
                else
                {
                    foreach (var decal in decalsList)
                    {
                        tint = decal.InstancedColor;
                        tint.a *= decal.Fade;
                        bufferBeforeLighting.SetGlobalColor("_NotInstancedColor", tint);
                        uv = decal.UV;
                        bufferBeforeLighting.SetGlobalVector("_NotInstancedUV", uv);
                        bufferBeforeLighting.DrawMesh(CubeMesh, decal.transform.localToWorldMatrix, decal.DecalMaterial, 0, 2);
                    }
                }
            }

            bufferBeforeLighting.SetGlobalTexture("_Alpha", smoothnessID);
            bufferBeforeLighting.Blit(specularID, BuiltinRenderTextureType.GBuffer1, specularSmoothnessBlitterMaterial, 1);

            if (TerrainDecals == TerrainDecalsType.MultiTerrain)
            {
                foreach(var d in system.m_Decals)
                {
                    d.DecalMaterial.EnableKeyword("MULTI_TERRAIN_DECAL");
                }
            }
            else
            {
                foreach (var d in system.m_Decals)
                {
                    d.DecalMaterial.DisableKeyword("MULTI_TERRAIN_DECAL");
                }
            }
            if (Terrain.activeTerrains == null || Terrain.activeTerrains.Length == 0)
            {
                foreach (var d in system.m_Decals)
                {
                    d.DecalMaterial.EnableKeyword("NO_TERRAIN");
                }
            }
            else
            {
                foreach (var d in system.m_Decals)
                {
                    d.DecalMaterial.DisableKeyword("NO_TERRAIN");
                }
            }

            if (TerrainDecals != TerrainDecalsType.None)
            {
                if (TerrainDecals == TerrainDecalsType.OneTerrain)
                {
                    Terrain terrain = Terrain.activeTerrain;
                    terrain.drawHeightmap = true;
                    Shader.SetGlobalMatrix("_World2Terrain", terrain.transform.worldToLocalMatrix);
                    Shader.SetGlobalTexture("_TerrainHeightMap", terrain.terrainData.heightmapTexture);
                    Shader.SetGlobalVector("_TerrainSize", terrain.terrainData.size);
                } else if(TerrainDecals == TerrainDecalsType.MultiTerrain)
                {
                    Terrain[] terrains = Terrain.activeTerrains;

                    if(copyHeightmapsBuffer == null)
                        CopyHeightmaps();

                    terrainsDataBuffer.SetData(terrainsData);

                    SetupToShaders();
                }
            }

#if UNITY_EDITOR
            if(isSceneViewCamera)
            {
                float deltaTime = (float)(UnityEditor.EditorApplication.timeSinceStartup - lastRenderTime);
                lastRenderTime = UnityEditor.EditorApplication.timeSinceStartup;
                decalsToRemove.Clear();

                foreach (var decalTime in lastSelectedDecals)
                {
                    decalTime.Value.Value += deltaTime;
                    if (decalTime.Value.Value >= selectionDuration)
                    {
                        decalsToRemove.Add(decalTime.Key);
                    }
                }

                foreach (var decal in decalsToRemove)
                {
                    lastSelectedDecals.Remove(decal);
                }

                var selectedObjects = UnityEditor.Selection.objects;

                if (selectedObjects != null)
                {
                    selectionBuffer.SetRenderTarget(BuiltinRenderTextureType.CameraTarget, BuiltinRenderTextureType.CameraTarget);
                    foreach (var o in selectedObjects)
                    {
                        GameObject go = o as GameObject;

                        if (go != null)
                        {
                            Decal decal = go.GetComponent<Decal>();
                            if (decal != null)
                            {
                                FloatValue time;
                                if (lastSelectedDecals.TryGetValue(decal, out time))
                                {
                                    tint = decal.InstancedColor;
                                    tint.a *= decal.Fade;
                                    selectionBuffer.SetGlobalColor("_NotInstancedColor", tint);
                                    uv = decal.UV;
                                    selectionBuffer.SetGlobalVector("_NotInstancedUV", uv);

                                    selectionBuffer.SetGlobalFloat("SelectionTime", selectionFadeCurve.Evaluate(time.Value / selectionDuration));
                                    selectionBuffer.DrawMesh(CubeMesh, decal.transform.localToWorldMatrix, decal.DecalMaterial, 0, 1); // selection pass - 1
                                }
                                
                            }
                        }
                    }
                }
            }
#endif
            // release temporary RT
            bufferBeforeLighting.ReleaseTemporaryRT(normalsID);
            bufferBeforeLighting.ReleaseTemporaryRT(specularID);

            if (selectionBuffer != null)
            {
                selectionBuffer.ReleaseTemporaryRT(normalsID);
            }
        }


        [ContextMenu("Copy terrains heightmaps")]
        public void CopyHeightmaps()
        {
            Terrain[] terrains = Terrain.activeTerrains;
            if (terrainsTextures != null)
                DestroyTextures();

            if (terrainsTextures == null)
            {
                terrainsTextures = new Texture2D[terrains.Length];
            }

            for (int i = 0; i < terrains.Length; i++)
            {
                terrains[i].drawHeightmap = true;
                if (terrainsTextures[i] == null)
                    terrainsTextures[i] = new Texture2D(TerrainHeightMapSize, TerrainHeightMapSize, TextureFormat.RFloat, false);
            }

            if (terrainsData == null || terrainsData.Length != terrains.Length)
                terrainsData = new TerrainData[terrains.Length];

            if (terrainsDataBuffer != null && terrainsDataBuffer.count != terrains.Length)
            {
                terrainsDataBuffer.Dispose();
                terrainsDataBuffer = null;
            }
            if (terrainsDataBuffer == null)
                terrainsDataBuffer = new ComputeBuffer(terrains.Length, TerrainData.Size);

            if (heightmapsCollector != null && heightmapsCollector.RequiredSizeX != TerrainHeightMapSize)
            {
                heightmapsCollector.Resize(TerrainHeightMapSize, TerrainHeightMapSize);
            }
            else if (heightmapsCollector == null)
            {
                heightmapsCollector = new HeightmapsCollector(TerrainHeightMapSize, TerrainHeightMapSize);
            }
            if (copyHeightmapsBuffer == null)
            {
                copyHeightmapsBuffer = new CommandBuffer();
            }
            copyHeightmapsBuffer.Clear();
            heightmapsCollector.ClearTexturesList();
            for (int i = 0; i < terrains.Length; i++)
            {
                CopyTerrainHeightToTexture(terrains[i], terrainsTextures[i]);
                //copyHeightmapsBuffer.Blit(terrains[i].terrainData.heightmapTexture, terrainsTextures[i]);
                terrainsData[i].terrainSize = terrains[i].terrainData.size;
                terrainsData[i].heightmapIndex = i;
                terrainsData[i].world2Terrain = terrains[i].transform.worldToLocalMatrix;
                heightmapsCollector.AddTexture(terrainsTextures[i]);
            }
            Graphics.ExecuteCommandBuffer(copyHeightmapsBuffer);
            heightmapsCollector.Generate();
            terrainsDataBuffer.SetData(terrainsData);
            SetupToShaders();
        }

        private void CopyTerrainHeightToTexture(Terrain terrain, Texture2D targetTexture)
        {
            float dx = terrain.terrainData.size.x / targetTexture.width;
            float dz = terrain.terrainData.size.z / targetTexture.height;
            for (int x = 0; x < targetTexture.width; x++)
            {
                for (int y = 0; y < targetTexture.height; y++)
                {
                    Vector3 position = new Vector3(x * dx, 0, y * dz);
                    position = terrain.transform.TransformPoint(position);
                    float height = terrain.SampleHeight(position);
                    targetTexture.SetPixel(x, y, new Color(height, 0, 0, 0));
                }
            }

            targetTexture.Apply();
        }

        public void SetupToShaders()
        {
            Shader.SetGlobalBuffer("TerrainsDatas", terrainsDataBuffer);
            Shader.SetGlobalTexture("_TerrainsHeightMaps", heightmapsCollector.Texture);
            Shader.SetGlobalInt("TerrainsCount", terrainsDataBuffer.count);
        }

        void DestroyTextures()
        {
            if (terrainsTextures != null)
            {
                for (int i = 0; i < terrainsTextures.Length; i++)
                {
                    DestroyImmediate(terrainsTextures[i], true);
                }
                terrainsTextures = null;
            }
        }

        public void SetupBounds(Decal decal)
        {
            if (decal == null)
                return;

            decal.SetupBounds();
        }

        public bool IsBoundsVisible(Plane[] planes, Bounds bounds)
        {
            return GeometryUtility.TestPlanesAABB(planes, bounds);
        }
    }

    public struct TerrainData
    {
        public int heightmapIndex;
        public Matrix4x4 world2Terrain;
        public Vector3 terrainSize;

        public static int Size
        {
            get
            {
                int size = 0;
                size += sizeof(int); // heightmapIndex
                size += sizeof(float) * 4 * 4; // world2Terrain
                size += sizeof(float) * 3; // terrainSize

                return size;
            }
        }
    }

    public class DeferredDecalsManager
    {
        static DeferredDecalsManager m_Instance;
        static public DeferredDecalsManager instance
        {
            get
            {
                if (m_Instance == null)
                    m_Instance = new DeferredDecalsManager();

                return m_Instance;
            }
        }

        internal List<Decal> m_Decals = new List<Decal>();

        public void AddOrUpdateDecal(Decal d)
        {
            RemoveDecal(d);

            if (d.DecalMaterial == null)
                return;

            if (m_Decals.Count == 0)
                m_Decals.Add(d);
            else
            {
                for (int i = 0; i <= m_Decals.Count; i++)
                {
                    if (i == m_Decals.Count)
                    {
                        m_Decals.Add(d);
                        break;
                    }

                    if (m_Decals[i].SortingOrder > d.SortingOrder)
                    {
                        m_Decals.Insert(i, d);
                        break;
                    }
                }
            }

            /*for (int i = 0; i < m_Decals.Count; i++)
            {
                Debug.LogWarning(m_Decals[i].name + " " + m_Decals[i].SortingOrder);
            }*/
        }

        public void RemoveDecal(Decal d)
        {
            m_Decals.Remove(d);
        }
    }

    public class CustomRendererManager
    {
        static CustomRendererManager m_Instance;
        static public CustomRendererManager instance
        {
            get
            {
                if (m_Instance == null)
                    m_Instance = new CustomRendererManager();

                return m_Instance;
            }
        }

        public int GlobalTexID
        {
            get
            {
                if (globalTexID == -1)
                    globalTexID = Shader.PropertyToID("_GlobalTex");

                return globalTexID;
            }
        }

        internal Dictionary<Renderer, int[]> m_CustomRenderers = new Dictionary<Renderer, int[]>();

        CommandBuffer renderBuffer;
        Material renderingMaterial;

        int globalTexID = -1;

        public void SetRenderBuffer(CommandBuffer renderBuffer, Material renderingMaterial)
        {
            this.renderBuffer = renderBuffer;
            this.renderingMaterial = renderingMaterial;
            UpdateBuffer();
        }

        public void AddCustomRendererID(Renderer targetRenderer, int[] submeshes)
        {
            if (m_CustomRenderers.ContainsKey(targetRenderer))
                return;

            m_CustomRenderers.Add(targetRenderer, submeshes);

            if (renderBuffer != null)
            {
                renderBuffer.Clear();
                UpdateBuffer();
            }
        }

        void UpdateBuffer()
        {
            renderBuffer.EnableShaderKeyword("GLOBAL");
            foreach (var kv in m_CustomRenderers)
            {
                for (int sm = 0; sm < kv.Value.Length; sm++)
                {
                    renderBuffer.SetGlobalTexture(GlobalTexID, kv.Key.sharedMaterials[sm].mainTexture);
                    renderBuffer.DrawRenderer(kv.Key, renderingMaterial, kv.Value[sm]);
                }
            }
            renderBuffer.SetGlobalTexture(GlobalTexID, Texture2D.whiteTexture);
        }

        public void RemoveCustomRendererID(Renderer id)
        {
            if (!m_CustomRenderers.ContainsKey(id))
                return;

            m_CustomRenderers.Remove(id);

            if (renderBuffer != null)
            {
                renderBuffer.Clear();
                UpdateBuffer();
            }
        }
    }
}