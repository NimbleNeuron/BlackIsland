using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System;

namespace Knife.Tools
{
    public static class GPURaycast
    {
        static RenderTexture worldPosBuffer;
        static RenderTexture worldNormBuffer;
        static RenderTexture idBuffer;
        static RenderBuffer[] mrtBuffers;

        static Camera gpuRaycastCamera;
        static Plane[] cameraPlanes;
        static Renderer[] allSceneRenderers;
        static Renderer[] visibleRenderers;

        static Dictionary<Renderer, DecalsTarget> targets = new Dictionary<Renderer, DecalsTarget>(); 
        static List<Renderer> allRegisteredRenderers = new List<Renderer>();
        static List<Renderer> visibleRegisteredRenderers = new List<Renderer>();

        static MaterialPropertyBlock propertyBlock;
        static Texture2D posBufferCopy;
        static Texture2D normBufferCopy;
        static Texture2D idBufferCopy;

        static int lastPixelWidth;
        static int lastPixelHeight;
        static Camera lastCamera;

        static Shader gpuRaycastShader;
        static Shader gpuPackedRaycastShader;
        static ComputeShader gpuRaycastCopyDataShader;
        static Color positionRead;
        static Color normalRead;
        static Color idRead;

        static Vector4 _ZBufferParams = new Vector4();

        private static int LastPixelWidth
        {
            get
            {
                return lastPixelWidth;
            }

            set
            {
                lastPixelWidth = value;
            }
        }

        private static int LastPixelHeight
        {
            get
            {
                return lastPixelHeight;
            }

            set
            {
                lastPixelHeight = value;
            }
        }

        public static bool Raycast(Camera camera, Vector2 pixelPosition, bool renderersHitInfo, out GPURaycastInfo hitInfo)
        {
            InitRes();
            if (renderersHitInfo)
                return Raycast(camera, pixelPosition, out hitInfo);
            else
                return RaycastWithoutRenderer(camera, pixelPosition, out hitInfo);
        }

        private static void InitRes()
        {
            if (posBufferCopy == null)
                posBufferCopy = new Texture2D(1, 1, TextureFormat.RGBAFloat, false);
            if (normBufferCopy == null)
                normBufferCopy = new Texture2D(1, 1, TextureFormat.RGBAFloat, false);
            if (idBufferCopy == null)
                idBufferCopy = new Texture2D(1, 1, TextureFormat.RGBAFloat, false);
            if (propertyBlock == null)
                propertyBlock = new MaterialPropertyBlock();
            if (gpuRaycastShader == null)
                gpuRaycastShader = Resources.Load<Shader>("Knife/GPURaycast/GPURaycastInfoShader");
            if (gpuPackedRaycastShader == null)
                gpuPackedRaycastShader = Resources.Load<Shader>("Knife/GPURaycast/GPURaycastPackedInfoShader");
            if (gpuRaycastCopyDataShader == null)
                gpuRaycastCopyDataShader = Resources.Load<ComputeShader>("Knife/GPURaycast/RenderTextureToBuffer");
        }

        public static void AddDecalsTarget(DecalsTarget target)
        {
            if (target == null)
            {
                throw new ArgumentNullException("Decals Target cannot be null");
            }

            if (targets.ContainsValue(target))
            {
                RemoveDecalsTarget(target);
            }

            //targets.Add(target);
            for (int i = 0; i < target.Renderers.Length; i++)
            {
                targets.Add(target.Renderers[i], target);
                allRegisteredRenderers.Remove(target.Renderers[i]);
            }
            allRegisteredRenderers.AddRange(target.Renderers);
        }

        public static void RemoveDecalsTarget(DecalsTarget target)
        {
            if (target == null)
            {
                throw new ArgumentNullException("Decals Target cannot be null");
            }

            for (int i = 0; i < target.Renderers.Length; i++)
            {
                targets.Remove(target.Renderers[i]);
                allRegisteredRenderers.Remove(target.Renderers[i]);
            }
        }

        /*
            // Z buffer to linear depth
        inline float LinearEyeDepth( float z )
        {
            return 1.0 / (_ZBufferParams.z * z + _ZBufferParams.w);
        } 
        */
        static float LinearEyeDepth(float z)
        {
            return 1f / (_ZBufferParams.z * z + _ZBufferParams.w);
        }

        /*
            inline float LinearEyeDepth( float z )
            {
                return 1.0 / (_ZBufferParams.z * zOUT + _ZBufferParams.w);
            }
            // _ZBufferParams.z * zOUT + _ZBufferParams.w = 1.0 / z
            // _ZBufferParams.z * zOUT = (1.0 / z) - _ZBufferParams.w
            // zOUT = ((1.0 / z) - _ZBufferParams.w) / _ZBufferParams.z
        */
        static float Unlinear01Depth(float linear01depth)
        {
            //return 1f / (_ZBufferParams.z * z + _ZBufferParams.w);
            return ((1f / linear01depth) - _ZBufferParams.w) / _ZBufferParams.z;
        }

        static void PackZBufferParams(Camera camera)
        {
            _ZBufferParams.x = (1f - camera.farClipPlane / camera.nearClipPlane);
            _ZBufferParams.y = camera.farClipPlane / camera.nearClipPlane;
            _ZBufferParams.z = _ZBufferParams.x / camera.farClipPlane;
            _ZBufferParams.w = _ZBufferParams.y / camera.farClipPlane;
        }
        /*
		    color.r = floor(depth / 1000000);
		    color.r *= 1000000;
		    color.g = floor((depth - color.r) / 1000);
		    color.b = floor(depth - color.r - color.g * 1000) / 256;
		    color.r = color.r / 256000000;
		    color.g = color.g / 256; 
        */
        static Color UnpackData(float packedData)
        {
            Color unpackedData = Color.clear;
            unpackedData.r = Mathf.Floor(packedData / 1000000f);
            unpackedData.r *= 1000000f;
            unpackedData.g = Mathf.Floor((packedData - unpackedData.r) / 1000f);
            unpackedData.b = Mathf.Floor(packedData - unpackedData.r - unpackedData.g * 1000f) / 256f;
            unpackedData.r = unpackedData.r / 256000000f;
            unpackedData.g = unpackedData.g / 256f;

            return unpackedData;
        }

        static void SetupHitInfo(out GPURaycastDecalsTargetInfo hitInfo)
        {
            hitInfo.position = Vector3.zero;
            hitInfo.normal = Vector3.zero;
            hitInfo.hittedRenderer = null;
            hitInfo.IsHitted = false;
            hitInfo.hittedTarget = null;
            hitInfo.VertexIndex = -1;
        }

        public static bool RaycastToRegisteredTargets(Camera camera, float pixelX, float pixelY, out GPURaycastDecalsTargetInfo hitInfo)
        {
            Vector2 uv = Vector2.zero;

            uv.x = pixelX / camera.pixelWidth;
            uv.y = pixelY / camera.pixelHeight;

            return RaycastToRegisteredTargets(camera, uv, out hitInfo);
        }

        public static bool RaycastToRegisteredTargetsFromMousePosition(Camera camera, out GPURaycastDecalsTargetInfo hitInfo)
        {
            return RaycastToRegisteredTargets(camera, Input.mousePosition.x, Input.mousePosition.y, out hitInfo);
        }

        public static bool RaycastToRegisteredTargets(Camera camera, Vector2 uv, out GPURaycastDecalsTargetInfo hitInfo)
        {
            InitRes();

            uv.y = 1f - uv.y;
#if UNITY_EDITOR_OSX
            uv.y -= 0.5f;
            uv.y *= 2f;

            uv.x *= 2f;
#endif

            SetupHitInfo(out hitInfo);

            int cameraPixelWidth = camera.targetTexture == null ? camera.pixelWidth : camera.targetTexture.width;
            int cameraPixelHeight = camera.targetTexture == null ? camera.pixelHeight : camera.targetTexture.height;

            float scaleFactor = 0.2f;
            cameraPixelWidth = (int)(cameraPixelWidth * scaleFactor);
            cameraPixelHeight = (int)(cameraPixelHeight * scaleFactor);

            if (lastCamera != camera || gpuRaycastCamera == null)
            {
                gpuRaycastCamera = new GameObject("GPURaycastCamera", typeof(Camera)).GetComponent<Camera>();
                gpuRaycastCamera.gameObject.SetActive(false);
                gpuRaycastCamera.enabled = false;
                gpuRaycastCamera.gameObject.hideFlags = HideFlags.HideAndDontSave;
            }

            gpuRaycastCamera.cameraType = CameraType.Game;
            gpuRaycastCamera.targetTexture = null;
            gpuRaycastCamera.transform.position = camera.transform.position;
            gpuRaycastCamera.transform.rotation = camera.transform.rotation;
            gpuRaycastCamera.clearFlags = CameraClearFlags.Color;
            gpuRaycastCamera.backgroundColor = Color.clear;
            gpuRaycastCamera.renderingPath = RenderingPath.Forward;
            gpuRaycastCamera.rect = camera.rect;

            lastCamera = camera;

            cameraPlanes = GeometryUtility.CalculateFrustumPlanes(gpuRaycastCamera);
            visibleRegisteredRenderers.Clear();
            visibleRegisteredRenderers.AddRange(allRegisteredRenderers.FindAll((r) =>
            {
                return GeometryUtility.TestPlanesAABB(cameraPlanes, r.bounds);
            }));

            for (int i = 0; i < visibleRegisteredRenderers.Count; i++)
            {
                visibleRegisteredRenderers[i].GetPropertyBlock(propertyBlock);
                propertyBlock.SetInt("ObjectID", i + 1);
                visibleRegisteredRenderers[i].SetPropertyBlock(propertyBlock);
            }

            if (LastPixelWidth != cameraPixelWidth || LastPixelHeight != cameraPixelHeight || worldPosBuffer == null)
            {
                if (worldPosBuffer != null)
                {
                    worldPosBuffer.Release();
                    GameObject.DestroyImmediate(worldPosBuffer, true);
                }
                worldPosBuffer = new RenderTexture(cameraPixelWidth, cameraPixelHeight, 16, RenderTextureFormat.ARGBFloat);
                worldPosBuffer.Create();

                LastPixelWidth = cameraPixelWidth;
                LastPixelHeight = cameraPixelHeight;
            }

            gpuRaycastCamera.targetTexture = worldPosBuffer;
            gpuRaycastCamera.RenderWithShader(gpuPackedRaycastShader, "");

            if (uv.x < 0 || uv.x > 1 || uv.y < 0 || uv.y > 1)
            {
                return false;
            }

            int idx = (int)(uv.x * cameraPixelWidth);
            int idy = (int)(uv.y * cameraPixelHeight);

            var lastActive = RenderTexture.active;

            RenderTexture.active = worldPosBuffer;
            posBufferCopy.ReadPixels(new Rect(idx, idy, 1, 1), 0, 0);
            RenderTexture.active = lastActive;

            posBufferCopy.Apply();

            Color packedData = posBufferCopy.GetPixel(0, 0);
            // r - bone index
            // g - lineareyedepth(uv.z)
            // b - packed world normal
            // a - object id + 1

            Color spDepth = Color.clear;
            spDepth.r = uv.x;
            spDepth.g = 1f - uv.y;
            spDepth.b = packedData.g;
            Vector2 screenPos = new Vector2(spDepth.r, spDepth.g);

            float depth = spDepth.b;
            Matrix4x4 proj = camera.projectionMatrix;
            Matrix4x4 cameraToWorld = camera.cameraToWorldMatrix;
            Vector2 p11_22 = new Vector2(proj.m00, proj.m11);
            Vector2 p13_31 = new Vector2(proj.m02, proj.m12);
            Vector2 xy = (screenPos * 2 - Vector2.one - p13_31);
            xy.x /= p11_22.x;
            xy.y /= p11_22.y;

            Vector3 pos_o = new Vector3(xy.x, xy.y, 1) * depth;
            pos_o.z *= -1;
            Vector3 wpos = cameraToWorld.MultiplyVector(pos_o) + camera.transform.position;

            normalRead = UnpackData(packedData.b) * 2 - Color.white;
            Vector3 normVector = new Vector3(normalRead.r, normalRead.g, normalRead.b);
            normVector.Normalize();

            int objectID = Mathf.RoundToInt(packedData.a) - 1 - 1;

            hitInfo.position = wpos;
            hitInfo.normal = normVector;
            if (objectID >= 0 && objectID < visibleRegisteredRenderers.Count)
            {
                hitInfo.hittedRenderer = visibleRegisteredRenderers[objectID];
                hitInfo.hittedTarget = targets[hitInfo.hittedRenderer];
            }
            hitInfo.VertexIndex = (int)packedData.r;

            hitInfo.IsHitted = packedData.a > 1;
            return hitInfo.IsHitted;
        }


        private static bool Raycast(Camera camera, Vector2 pixelPosition, out GPURaycastInfo hitInfo)
        {
            hitInfo.position = Vector3.zero;
            hitInfo.normal = Vector3.zero;
            hitInfo.hittedRenderer = null;
            hitInfo.IsHitted = false;

            int cameraPixelWidth = camera.targetTexture == null ? camera.pixelWidth : camera.targetTexture.width;
            int cameraPixelHeight = camera.targetTexture == null ? camera.pixelHeight : camera.targetTexture.height;

            if (lastCamera != camera || gpuRaycastCamera == null)
            {
                gpuRaycastCamera = new GameObject("GPURaycastCamera", typeof(Camera)).GetComponent<Camera>();
                gpuRaycastCamera.gameObject.SetActive(false);
                gpuRaycastCamera.enabled = false;
                gpuRaycastCamera.gameObject.hideFlags = HideFlags.HideAndDontSave;
            }

            gpuRaycastCamera.cameraType = CameraType.Game;
            gpuRaycastCamera.targetTexture = null;
            gpuRaycastCamera.transform.position = camera.transform.position;
            gpuRaycastCamera.transform.rotation = camera.transform.rotation;
            gpuRaycastCamera.clearFlags = CameraClearFlags.Color;
            gpuRaycastCamera.backgroundColor = Color.clear;
            gpuRaycastCamera.renderingPath = RenderingPath.Forward;
            gpuRaycastCamera.rect = camera.rect;

            lastCamera = camera;

            cameraPlanes = GeometryUtility.CalculateFrustumPlanes(gpuRaycastCamera);

            allSceneRenderers = GameObject.FindObjectsOfType<Renderer>();
            visibleRenderers = allSceneRenderers.ToList().FindAll((r) =>
            {
                return GeometryUtility.TestPlanesAABB(cameraPlanes, r.bounds);
            }).ToArray();

            for (int i = 0; i < visibleRenderers.Length; i++)
            {
                visibleRenderers[i].GetPropertyBlock(propertyBlock);
                propertyBlock.SetInt("ObjectID", i);
                visibleRenderers[i].SetPropertyBlock(propertyBlock);
            }

            if (LastPixelWidth != cameraPixelWidth || LastPixelHeight != cameraPixelHeight || worldPosBuffer == null || worldNormBuffer == null || idBuffer == null)
            {
                if (worldPosBuffer != null)
                {
                    worldPosBuffer.Release();
                    GameObject.DestroyImmediate(worldPosBuffer, true);
                }
                if (worldNormBuffer != null)
                {
                    worldNormBuffer.Release();
                    GameObject.DestroyImmediate(worldNormBuffer, true);
                }
                if (idBuffer != null)
                {
                    idBuffer.Release();
                    GameObject.DestroyImmediate(idBuffer, true);
                }

                worldPosBuffer = new RenderTexture(cameraPixelWidth, cameraPixelHeight, 16, RenderTextureFormat.ARGBFloat);
                worldNormBuffer = new RenderTexture(cameraPixelWidth, cameraPixelHeight, 0, RenderTextureFormat.ARGBFloat);
                idBuffer = new RenderTexture(cameraPixelWidth, cameraPixelHeight, 0, RenderTextureFormat.ARGBFloat);
                worldPosBuffer.Create();
                worldNormBuffer.Create();
                idBuffer.Create();
                mrtBuffers = new RenderBuffer[3];
                mrtBuffers[0] = worldPosBuffer.colorBuffer;
                mrtBuffers[1] = worldNormBuffer.colorBuffer;
                mrtBuffers[2] = idBuffer.colorBuffer;

                LastPixelWidth = cameraPixelWidth;
                LastPixelHeight = cameraPixelHeight;
                gpuRaycastCamera.SetTargetBuffers(mrtBuffers, worldPosBuffer.depthBuffer);
            }

            gpuRaycastCamera.SetTargetBuffers(mrtBuffers, worldPosBuffer.depthBuffer);
            gpuRaycastCamera.RenderWithShader(gpuRaycastShader, "");

            Vector2 uv = new Vector2(pixelPosition.x / camera.pixelWidth, pixelPosition.y / camera.pixelHeight);

            if (uv.x < 0 || uv.x > 1 || uv.y < 0 || uv.y > 1)
            {
                return false;
            }

            int idx = (int)(uv.x * cameraPixelWidth);
            int idy = (int)(uv.y * cameraPixelHeight);
            
            var lastActive = RenderTexture.active;

            RenderTexture.active = worldPosBuffer;
            posBufferCopy.ReadPixels(new Rect(idx, idy, 1, 1), 0, 0);
            RenderTexture.active = worldNormBuffer;
            normBufferCopy.ReadPixels(new Rect(idx, idy, 1, 1), 0, 0);
            RenderTexture.active = idBuffer;
            idBufferCopy.ReadPixels(new Rect(idx, idy, 1, 1), 0, 0);
            RenderTexture.active = lastActive;

            posBufferCopy.Apply();
            normBufferCopy.Apply();
            idBufferCopy.Apply();

            positionRead = posBufferCopy.GetPixel(0, 0);
            normalRead = normBufferCopy.GetPixel(0, 0);
            idRead = idBufferCopy.GetPixel(0, 0);

            int objectID = Mathf.RoundToInt(idRead.r) - 1;

            hitInfo.position = new Vector3(positionRead.r, positionRead.g, positionRead.b);
            hitInfo.normal = new Vector3(normalRead.r, normalRead.g, normalRead.b);
            if (objectID >= 0 && objectID < visibleRenderers.Length)
                hitInfo.hittedRenderer = visibleRenderers[objectID];

            hitInfo.IsHitted = positionRead.a > 0;
            return hitInfo.IsHitted;
        }

        private static bool RaycastWithoutRenderer(Camera camera, Vector2 pixelPosition, out GPURaycastInfo hitInfo)
        {
            hitInfo.position = Vector3.zero;
            hitInfo.normal = Vector3.zero;
            hitInfo.hittedRenderer = null;
            hitInfo.IsHitted = false;

            int cameraPixelWidth = camera.targetTexture == null ? camera.pixelWidth : camera.targetTexture.width;
            int cameraPixelHeight = camera.targetTexture == null ? camera.pixelHeight : camera.targetTexture.height;

            if (lastCamera != camera || gpuRaycastCamera == null)
            {
                gpuRaycastCamera = new GameObject("GPURaycastCamera", typeof(Camera)).GetComponent<Camera>();
                gpuRaycastCamera.gameObject.SetActive(false);
                gpuRaycastCamera.enabled = false;
                gpuRaycastCamera.gameObject.hideFlags = HideFlags.HideAndDontSave;
            }

            gpuRaycastCamera.cameraType = CameraType.Game;
            gpuRaycastCamera.targetTexture = null;
            gpuRaycastCamera.transform.position = camera.transform.position;
            gpuRaycastCamera.transform.rotation = camera.transform.rotation;
            gpuRaycastCamera.clearFlags = CameraClearFlags.Color;
            gpuRaycastCamera.backgroundColor = Color.clear;
            gpuRaycastCamera.renderingPath = RenderingPath.Forward;
            gpuRaycastCamera.rect = camera.rect;
            gpuRaycastCamera.fieldOfView = camera.fieldOfView;
            gpuRaycastCamera.aspect = camera.aspect;

            lastCamera = camera;

            Vector2 uv = new Vector2(pixelPosition.x / camera.pixelWidth, pixelPosition.y / camera.pixelHeight);
#if UNITY_EDITOR_OSX
            uv.y -= 0.5f;
            uv.y *= 2f;

            uv.x *= 2f;
#endif

            if (uv.x < 0 || uv.x > 1 || uv.y < 0 || uv.y > 1)
            {
                return false;
            }

            if (LastPixelWidth != cameraPixelWidth || LastPixelHeight != cameraPixelHeight || worldPosBuffer == null || worldNormBuffer == null || idBuffer == null)
            {
                if (worldPosBuffer != null)
                {
                    worldPosBuffer.Release();
                    GameObject.DestroyImmediate(worldPosBuffer, true);
                }
                if (worldNormBuffer != null)
                {
                    worldNormBuffer.Release();
                    GameObject.DestroyImmediate(worldNormBuffer, true);
                }
                if (idBuffer != null)
                {
                    idBuffer.Release();
                    GameObject.DestroyImmediate(idBuffer, true);
                }

                worldPosBuffer = new RenderTexture(cameraPixelWidth, cameraPixelHeight, 16, RenderTextureFormat.ARGBFloat);
                worldNormBuffer = new RenderTexture(cameraPixelWidth, cameraPixelHeight, 0, RenderTextureFormat.ARGBFloat);
                idBuffer = new RenderTexture(cameraPixelWidth, cameraPixelHeight, 0, RenderTextureFormat.ARGBFloat);
                worldPosBuffer.Create();
                worldNormBuffer.Create();
                idBuffer.Create();
                mrtBuffers = new RenderBuffer[3];
                mrtBuffers[0] = worldPosBuffer.colorBuffer;
                mrtBuffers[1] = worldNormBuffer.colorBuffer;
                mrtBuffers[2] = idBuffer.colorBuffer;

                LastPixelWidth = cameraPixelWidth;
                LastPixelHeight = cameraPixelHeight;
                gpuRaycastCamera.SetTargetBuffers(mrtBuffers, worldPosBuffer.depthBuffer);
            }

            gpuRaycastCamera.SetTargetBuffers(mrtBuffers, worldPosBuffer.depthBuffer);
            gpuRaycastCamera.RenderWithShader(gpuRaycastShader, "");

            int idx = (int)(uv.x * cameraPixelWidth);
            int idy = (int)(uv.y * cameraPixelHeight);

            var lastActive = RenderTexture.active;
            RenderTexture.active = worldPosBuffer;
            posBufferCopy.ReadPixels(new Rect(idx, idy, 1, 1), 0, 0);
            RenderTexture.active = worldNormBuffer;
            normBufferCopy.ReadPixels(new Rect(idx, idy, 1, 1), 0, 0);
            RenderTexture.active = idBuffer;
            idBufferCopy.ReadPixels(new Rect(idx, idy, 1, 1), 0, 0);
            RenderTexture.active = lastActive;

            posBufferCopy.Apply();
            normBufferCopy.Apply();
            idBufferCopy.Apply();

            positionRead = posBufferCopy.GetPixel(0, 0);
            normalRead = normBufferCopy.GetPixel(0, 0);
            idRead = idBufferCopy.GetPixel(0, 0);

            hitInfo.position = new Vector3(positionRead.r, positionRead.g, positionRead.b);
            hitInfo.normal = new Vector3(normalRead.r, normalRead.g, normalRead.b);
            hitInfo.IsHitted = positionRead.a > 0;

            return hitInfo.IsHitted;
        }

        public static void FullscreenRaycastAsync(Camera camera, float resolutionFactor, out int width, out int height, Action<GPURaycastInfo[]> callback)
        {
            InitRes();
            float resolution = resolutionFactor;

            int cameraPixelWidth = camera.targetTexture == null ? camera.pixelWidth : camera.targetTexture.width;
            int cameraPixelHeight = camera.targetTexture == null ? camera.pixelHeight : camera.targetTexture.height;

            cameraPixelWidth = (int)(cameraPixelWidth * resolution);
            cameraPixelHeight = (int)(cameraPixelHeight * resolution);

            width = cameraPixelWidth;
            height = cameraPixelHeight;

            if (lastCamera != camera || gpuRaycastCamera == null)
            {
                gpuRaycastCamera = new GameObject("GPURaycastCamera", typeof(Camera)).GetComponent<Camera>();
                gpuRaycastCamera.gameObject.SetActive(false);
                gpuRaycastCamera.enabled = false;
                gpuRaycastCamera.gameObject.hideFlags = HideFlags.HideAndDontSave;
            }

            gpuRaycastCamera.cameraType = CameraType.Game;
            gpuRaycastCamera.targetTexture = null;
            gpuRaycastCamera.transform.position = camera.transform.position;
            gpuRaycastCamera.transform.rotation = camera.transform.rotation;
            gpuRaycastCamera.clearFlags = CameraClearFlags.Color;
            gpuRaycastCamera.backgroundColor = Color.clear;
            gpuRaycastCamera.renderingPath = RenderingPath.Forward;
            gpuRaycastCamera.rect = camera.rect;
            gpuRaycastCamera.fieldOfView = camera.fieldOfView;
            gpuRaycastCamera.aspect = camera.aspect;

            lastCamera = camera;

            if (LastPixelWidth != width || LastPixelHeight != height || worldPosBuffer == null || worldNormBuffer == null || idBuffer == null)
            {
                if (worldPosBuffer != null)
                {
                    worldPosBuffer.Release();
                    GameObject.DestroyImmediate(worldPosBuffer, true);
                }
                if (worldNormBuffer != null)
                {
                    worldNormBuffer.Release();
                    GameObject.DestroyImmediate(worldNormBuffer, true);
                }
                if (idBuffer != null)
                {
                    idBuffer.Release();
                    GameObject.DestroyImmediate(idBuffer, true);
                }

                worldPosBuffer = new RenderTexture(width, height, 16, RenderTextureFormat.ARGBFloat);
                worldNormBuffer = new RenderTexture(width, height, 0, RenderTextureFormat.ARGBFloat);
                idBuffer = new RenderTexture(width, height, 0, RenderTextureFormat.ARGBFloat);
                worldPosBuffer.Create();
                worldNormBuffer.Create();
                idBuffer.Create();
                mrtBuffers = new RenderBuffer[3];
                mrtBuffers[0] = worldPosBuffer.colorBuffer;
                mrtBuffers[1] = worldNormBuffer.colorBuffer;
                mrtBuffers[2] = idBuffer.colorBuffer;
                LastPixelWidth = width;
                LastPixelHeight = height;
                gpuRaycastCamera.SetTargetBuffers(mrtBuffers, worldPosBuffer.depthBuffer);
            }

            gpuRaycastCamera.SetTargetBuffers(mrtBuffers, worldPosBuffer.depthBuffer);
            gpuRaycastCamera.RenderWithShader(gpuRaycastShader, "");

            ComputeBuffer bufferPositions = new ComputeBuffer(width * height, sizeof(float) * 4);
            ComputeBuffer bufferNormals = new ComputeBuffer(width * height, sizeof(float) * 4);

            uint x, y, z;
            gpuRaycastCopyDataShader.GetKernelThreadGroupSizes(0, out x, out y, out z);
            gpuRaycastCopyDataShader.SetInt("buffersWidth", width);
            gpuRaycastCopyDataShader.SetTexture(0, "posTex", worldPosBuffer);
            gpuRaycastCopyDataShader.SetTexture(0, "normalTex", worldNormBuffer);
            gpuRaycastCopyDataShader.SetBuffer(0, "positions", bufferPositions);
            gpuRaycastCopyDataShader.SetBuffer(0, "normals", bufferNormals);
            gpuRaycastCopyDataShader.Dispatch(0, (int)(width / x), (int)(height / y), 1);

            Vector4[] positions = new Vector4[width * height];
            Vector4[] normals = new Vector4[width * height];

            /*bufferPositions.GetData(positions);
            bufferNormals.GetData(normals);

            GPURaycastInfo[] hits = new GPURaycastInfo[width * height];
            GPURaycastInfo hitInfo = new GPURaycastInfo();
            for (int i = 0; i < hits.Length; i++)
            {
                hitInfo.position = positions[i];
                hitInfo.normal = normals[i];
                hitInfo.IsHitted = positions[i].w > 0;
                hits[i] = hitInfo;
            }
            bufferPositions.Dispose();
            bufferNormals.Dispose();*/

            //var request = AsyncGPUReadback.Request(bufferPositions, (r));

            /*
            Texture2D posBufferCopy = posBufferCopy = new Texture2D(width, height, TextureFormat.RGBAFloat, false);
            Texture2D normBufferCopy = normBufferCopy = new Texture2D(width, height, TextureFormat.RGBAFloat, false);
            //Texture2D idBufferCopy = idBufferCopy = new Texture2D(cameraPixelWidth, cameraPixelHeight, TextureFormat.RGBAFloat, false);

            var lastActive = RenderTexture.active;
            RenderTexture.active = worldPosBuffer;
            posBufferCopy.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            RenderTexture.active = worldNormBuffer;
            normBufferCopy.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            //RenderTexture.active = idBuffer;
            //idBufferCopy.ReadPixels(new Rect(idx, idy, 1, 1), 0, 0);
            RenderTexture.active = lastActive;
            posBufferCopy.Apply();
            normBufferCopy.Apply();
            //idBufferCopy.Apply();

            Color[] pixelsPositions = posBufferCopy.GetPixels();
            Color[] pixelsNormals = normBufferCopy.GetPixels();

            GPURaycastInfo[] hits = new GPURaycastInfo[width * height];
            GPURaycastInfo hitInfo = new GPURaycastInfo();

            for (int i = 0; i < hits.Length; i++)
            {
                positionRead = pixelsPositions[i];
                normalRead = pixelsNormals[i];

                hitInfo.position = new Vector3(positionRead.r, positionRead.g, positionRead.b);
                hitInfo.normal = new Vector3(normalRead.r, normalRead.g, normalRead.b);
                hitInfo.IsHitted = positionRead.a > 0;
                hits[i] = hitInfo;
            }

            GameObject.DestroyImmediate(posBufferCopy, true);
            GameObject.DestroyImmediate(normBufferCopy, true);
            */
        }
        public static GPURaycastInfo[] FullscreenRaycast(Camera camera, float resolutionFactor, out int width, out int height)
        {
            InitRes();
            float resolution = resolutionFactor;

            int cameraPixelWidth = camera.targetTexture == null ? camera.pixelWidth : camera.targetTexture.width;
            int cameraPixelHeight = camera.targetTexture == null ? camera.pixelHeight : camera.targetTexture.height;

            cameraPixelWidth = (int)(cameraPixelWidth * resolution);
            cameraPixelHeight = (int)(cameraPixelHeight * resolution);

            width = cameraPixelWidth;
            height = cameraPixelHeight;

            if (lastCamera != camera || gpuRaycastCamera == null)
            {
                gpuRaycastCamera = new GameObject("GPURaycastCamera", typeof(Camera)).GetComponent<Camera>();
                gpuRaycastCamera.gameObject.SetActive(false);
                gpuRaycastCamera.enabled = false;
                gpuRaycastCamera.gameObject.hideFlags = HideFlags.HideAndDontSave;
            }

            gpuRaycastCamera.cameraType = CameraType.Game;
            gpuRaycastCamera.targetTexture = null;
            gpuRaycastCamera.transform.position = camera.transform.position;
            gpuRaycastCamera.transform.rotation = camera.transform.rotation;
            gpuRaycastCamera.clearFlags = CameraClearFlags.Color;
            gpuRaycastCamera.backgroundColor = Color.clear;
            gpuRaycastCamera.renderingPath = RenderingPath.Forward;
            gpuRaycastCamera.rect = camera.rect;
            gpuRaycastCamera.fieldOfView = camera.fieldOfView;
            gpuRaycastCamera.aspect = camera.aspect;

            lastCamera = camera;

            if (LastPixelWidth != width || LastPixelHeight != height || worldPosBuffer == null || worldNormBuffer == null || idBuffer == null)
            {
                if (worldPosBuffer != null)
                {
                    worldPosBuffer.Release();
                    GameObject.DestroyImmediate(worldPosBuffer, true);
                }
                if (worldNormBuffer != null)
                {
                    worldNormBuffer.Release();
                    GameObject.DestroyImmediate(worldNormBuffer, true);
                }
                if (idBuffer != null)
                {
                    idBuffer.Release();
                    GameObject.DestroyImmediate(idBuffer, true);
                }

                worldPosBuffer = new RenderTexture(width, height, 16, RenderTextureFormat.ARGBFloat);
                worldNormBuffer = new RenderTexture(width, height, 0, RenderTextureFormat.ARGBFloat);
                idBuffer = new RenderTexture(width, height, 0, RenderTextureFormat.ARGBFloat);
                worldPosBuffer.Create();
                worldNormBuffer.Create();
                idBuffer.Create();
                mrtBuffers = new RenderBuffer[3];
                mrtBuffers[0] = worldPosBuffer.colorBuffer;
                mrtBuffers[1] = worldNormBuffer.colorBuffer;
                mrtBuffers[2] = idBuffer.colorBuffer;
                LastPixelWidth = width;
                LastPixelHeight = height;
                gpuRaycastCamera.SetTargetBuffers(mrtBuffers, worldPosBuffer.depthBuffer);
            }

            gpuRaycastCamera.SetTargetBuffers(mrtBuffers, worldPosBuffer.depthBuffer);
            gpuRaycastCamera.RenderWithShader(gpuRaycastShader, "");

            ComputeBuffer bufferPositions = new ComputeBuffer(width * height, sizeof(float) * 4);
            ComputeBuffer bufferNormals = new ComputeBuffer(width * height, sizeof(float) * 4);

            uint x, y, z;
            gpuRaycastCopyDataShader.GetKernelThreadGroupSizes(0, out x, out y, out z);
            gpuRaycastCopyDataShader.SetInt("buffersWidth", width);
            gpuRaycastCopyDataShader.SetTexture(0, "posTex", worldPosBuffer);
            gpuRaycastCopyDataShader.SetTexture(0, "normalTex", worldNormBuffer);
            gpuRaycastCopyDataShader.SetBuffer(0, "positions", bufferPositions);
            gpuRaycastCopyDataShader.SetBuffer(0, "normals", bufferNormals);
            gpuRaycastCopyDataShader.Dispatch(0, (int)(width / x), (int)(height / y), 1);

            Vector4[] positions = new Vector4[width * height];
            Vector4[] normals = new Vector4[width * height];

            bufferPositions.GetData(positions);
            bufferNormals.GetData(normals);

            GPURaycastInfo[] hits = new GPURaycastInfo[width * height];
            GPURaycastInfo hitInfo = new GPURaycastInfo();
            for (int i = 0; i < hits.Length; i++)
            {
                hitInfo.position = positions[i];
                hitInfo.normal = normals[i];
                hitInfo.IsHitted = positions[i].w > 0;
                hits[i] = hitInfo;
            }
            bufferPositions.Dispose();
            bufferNormals.Dispose();

            /*
            Texture2D posBufferCopy = posBufferCopy = new Texture2D(width, height, TextureFormat.RGBAFloat, false);
            Texture2D normBufferCopy = normBufferCopy = new Texture2D(width, height, TextureFormat.RGBAFloat, false);
            //Texture2D idBufferCopy = idBufferCopy = new Texture2D(cameraPixelWidth, cameraPixelHeight, TextureFormat.RGBAFloat, false);

            var lastActive = RenderTexture.active;
            RenderTexture.active = worldPosBuffer;
            posBufferCopy.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            RenderTexture.active = worldNormBuffer;
            normBufferCopy.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            //RenderTexture.active = idBuffer;
            //idBufferCopy.ReadPixels(new Rect(idx, idy, 1, 1), 0, 0);
            RenderTexture.active = lastActive;
            posBufferCopy.Apply();
            normBufferCopy.Apply();
            //idBufferCopy.Apply();

            Color[] pixelsPositions = posBufferCopy.GetPixels();
            Color[] pixelsNormals = normBufferCopy.GetPixels();

            GPURaycastInfo[] hits = new GPURaycastInfo[width * height];
            GPURaycastInfo hitInfo = new GPURaycastInfo();

            for (int i = 0; i < hits.Length; i++)
            {
                positionRead = pixelsPositions[i];
                normalRead = pixelsNormals[i];

                hitInfo.position = new Vector3(positionRead.r, positionRead.g, positionRead.b);
                hitInfo.normal = new Vector3(normalRead.r, normalRead.g, normalRead.b);
                hitInfo.IsHitted = positionRead.a > 0;
                hits[i] = hitInfo;
            }

            GameObject.DestroyImmediate(posBufferCopy, true);
            GameObject.DestroyImmediate(normBufferCopy, true);
            */
            return hits;
        }

        public static int MultiRaycastWithoutRenderer(Camera camera, Vector2[] pixelPositions, List<GPURaycastInfo> hitsInfo)
        {
            hitsInfo.Clear();
            if (pixelPositions.Length == 0)
                return 0;

            float resolution = 0.4f;

            int cameraPixelWidth = camera.targetTexture == null ? camera.pixelWidth : camera.targetTexture.width;
            int cameraPixelHeight = camera.targetTexture == null ? camera.pixelHeight : camera.targetTexture.height;

            cameraPixelWidth = (int)(cameraPixelWidth * resolution);
            cameraPixelHeight = (int)(cameraPixelHeight * resolution);

            for (int i = 0; i < pixelPositions.Length; i++)
            {
                pixelPositions[i] *= resolution;
            }

            if (lastCamera != camera || gpuRaycastCamera == null)
            {
                gpuRaycastCamera = new GameObject("GPURaycastCamera", typeof(Camera)).GetComponent<Camera>();
                gpuRaycastCamera.gameObject.SetActive(false);
                gpuRaycastCamera.enabled = false;
                gpuRaycastCamera.gameObject.hideFlags = HideFlags.HideAndDontSave;
            }

            gpuRaycastCamera.cameraType = CameraType.Game;
            gpuRaycastCamera.targetTexture = null;
            gpuRaycastCamera.transform.position = camera.transform.position;
            gpuRaycastCamera.transform.rotation = camera.transform.rotation;
            gpuRaycastCamera.clearFlags = CameraClearFlags.Color;
            gpuRaycastCamera.backgroundColor = Color.clear;
            gpuRaycastCamera.renderingPath = RenderingPath.Forward;
            gpuRaycastCamera.rect = camera.rect;
            gpuRaycastCamera.fieldOfView = camera.fieldOfView;
            gpuRaycastCamera.aspect = camera.aspect;

            lastCamera = camera;

            if (LastPixelWidth != cameraPixelWidth || LastPixelHeight != cameraPixelHeight || worldPosBuffer == null || worldNormBuffer == null || idBuffer == null)
            {
                if (worldPosBuffer != null)
                {
                    worldPosBuffer.Release();
                    GameObject.DestroyImmediate(worldPosBuffer, true);
                }
                if (worldNormBuffer != null)
                {
                    worldNormBuffer.Release();
                    GameObject.DestroyImmediate(worldNormBuffer, true);
                }
                if (idBuffer != null)
                {
                    idBuffer.Release();
                    GameObject.DestroyImmediate(idBuffer, true);
                }

                worldPosBuffer = new RenderTexture(cameraPixelWidth, cameraPixelHeight, 16, RenderTextureFormat.ARGBFloat);
                worldNormBuffer = new RenderTexture(cameraPixelWidth, cameraPixelHeight, 0, RenderTextureFormat.ARGBFloat);
                idBuffer = new RenderTexture(cameraPixelWidth, cameraPixelHeight, 0, RenderTextureFormat.ARGBFloat);
                worldPosBuffer.Create();
                worldNormBuffer.Create();
                idBuffer.Create();
                mrtBuffers = new RenderBuffer[3];
                mrtBuffers[0] = worldPosBuffer.colorBuffer;
                mrtBuffers[1] = worldNormBuffer.colorBuffer;
                mrtBuffers[2] = idBuffer.colorBuffer;
                LastPixelWidth = cameraPixelWidth;
                LastPixelHeight = cameraPixelHeight;
                gpuRaycastCamera.SetTargetBuffers(mrtBuffers, worldPosBuffer.depthBuffer);
            }

            gpuRaycastCamera.SetTargetBuffers(mrtBuffers, worldPosBuffer.depthBuffer);
            gpuRaycastCamera.RenderWithShader(gpuRaycastShader, "");
            /*
            int minX = (int)pixelPositions.Min((v) => v.x);
            int minY = (int)pixelPositions.Min((v) => v.y);
            int maxX = (int)pixelPositions.Max((v) => v.x);
            int maxY = (int)pixelPositions.Max((v) => v.y);
            */
            int minX = cameraPixelWidth;
            int minY = cameraPixelHeight;
            int maxX = 0;
            int maxY = 0;

            for (int i = 0; i < pixelPositions.Length; i++)
            {
                if (pixelPositions[i].x < minX)
                {
                    minX = (int)pixelPositions[i].x;
                }
                if (pixelPositions[i].x > maxX)
                {
                    maxX = (int)pixelPositions[i].x;
                }

                if (pixelPositions[i].y < minY)
                {
                    minY = (int)pixelPositions[i].y;
                }
                if (pixelPositions[i].y > maxY)
                {
                    maxY = (int)pixelPositions[i].y;
                }
            }
            int width = Mathf.Clamp(maxX - minX, 1, cameraPixelWidth);
            int height = Mathf.Clamp(maxY - minY, 1, cameraPixelHeight);
            //Debug.Log(minX + " " + minY + " " + width + " " + height);

            Texture2D posBufferCopy = posBufferCopy = new Texture2D(width, height, TextureFormat.RGBAFloat, false);
            Texture2D normBufferCopy = normBufferCopy = new Texture2D(width, height, TextureFormat.RGBAFloat, false);
            //Texture2D idBufferCopy = idBufferCopy = new Texture2D(cameraPixelWidth, cameraPixelHeight, TextureFormat.RGBAFloat, false);

            var lastActive = RenderTexture.active;
            RenderTexture.active = worldPosBuffer;
            posBufferCopy.ReadPixels(new Rect(minX, minY, width, height), 0, 0);
            RenderTexture.active = worldNormBuffer;
            normBufferCopy.ReadPixels(new Rect(minX, minY, width, height), 0, 0);
            //RenderTexture.active = idBuffer;
            //idBufferCopy.ReadPixels(new Rect(idx, idy, 1, 1), 0, 0);
            RenderTexture.active = lastActive;
            posBufferCopy.Apply();
            normBufferCopy.Apply();
            //idBufferCopy.Apply();

            Color[] pixelsPositions = posBufferCopy.GetPixels();
            Color[] pixelsNormals = normBufferCopy.GetPixels();

            for (int i = 0; i < pixelPositions.Length; i++)
            {
                var pixelPosition = pixelPositions[i] - new Vector2(minX, minY) - Vector2.one;
                pixelPosition.y = height - pixelPosition.y;
                pixelPosition.x = Mathf.Clamp(pixelPosition.x, 0, width - 1);
                pixelPosition.y = Mathf.Clamp(pixelPosition.y, 0, height - 1);

                int idx = (int)pixelPosition.x;
                int idy = (int)pixelPosition.y;

                positionRead = pixelsPositions[idy * width + idx];
                normalRead = pixelsNormals[idy * width + idx];

                if (positionRead.a > 0)
                {
                    GPURaycastInfo hitInfo = new GPURaycastInfo();
                    hitInfo.position = new Vector3(positionRead.r, positionRead.g, positionRead.b);
                    hitInfo.normal = new Vector3(normalRead.r, normalRead.g, normalRead.b);
                    hitsInfo.Add(hitInfo);
                }
            }

            GameObject.DestroyImmediate(posBufferCopy, true);
            GameObject.DestroyImmediate(normBufferCopy, true);
            return hitsInfo.Count;
        }
    }

    public struct GPURaycastInfo
    {
        public Vector3 position;
        public Vector3 normal;
        public Renderer hittedRenderer;
        public bool IsHitted;
    }

    public struct GPURaycastDecalsTargetInfo
    {
        public Vector3 position;
        public Vector3 normal;
        public Renderer hittedRenderer;
        public int VertexIndex;
        public DecalsTarget hittedTarget;
        public bool IsHitted;
    }
}