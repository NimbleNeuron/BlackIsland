using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityStandardAssets.Water
{
	[ExecuteInEditMode]
	public class Water : MonoBehaviour
	{
		public enum WaterMode
		{
			Simple,


			Reflective,


			Refractive
		}


		private static bool s_InsideWater;


		public WaterMode waterMode = WaterMode.Refractive;


		public bool disablePixelLights = true;


		public int textureSize = 256;


		public float clipPlaneOffset = 0.07f;


		public LayerMask reflectLayers = -1;


		public LayerMask refractLayers = -1;


		private readonly Dictionary<Camera, Camera> m_ReflectionCameras = new Dictionary<Camera, Camera>();


		private readonly Dictionary<Camera, Camera> m_RefractionCameras = new Dictionary<Camera, Camera>();


		private WaterMode m_HardwareWaterSupport = WaterMode.Refractive;


		private int m_OldReflectionTextureSize;


		private int m_OldRefractionTextureSize;


		private RenderTexture m_ReflectionTexture;


		private RenderTexture m_RefractionTexture;


		private void Update()
		{
			if (!GetComponent<Renderer>())
			{
				return;
			}

			Material sharedMaterial = GetComponent<Renderer>().sharedMaterial;
			if (!sharedMaterial)
			{
				return;
			}

			Vector4 vector = sharedMaterial.GetVector("WaveSpeed");
			float @float = sharedMaterial.GetFloat("_WaveScale");
			Vector4 vector2 = new Vector4(@float, @float, @float * 0.4f, @float * 0.45f);
			double num = Time.timeSinceLevelLoad / 20.0;
			Vector4 value = new Vector4((float) Math.IEEERemainder(vector.x * vector2.x * num, 1.0),
				(float) Math.IEEERemainder(vector.y * vector2.y * num, 1.0),
				(float) Math.IEEERemainder(vector.z * vector2.z * num, 1.0),
				(float) Math.IEEERemainder(vector.w * vector2.w * num, 1.0));
			sharedMaterial.SetVector("_WaveOffset", value);
			sharedMaterial.SetVector("_WaveScale4", vector2);
		}


		private void OnDisable()
		{
			if (m_ReflectionTexture)
			{
				DestroyImmediate(m_ReflectionTexture);
				m_ReflectionTexture = null;
			}

			if (m_RefractionTexture)
			{
				DestroyImmediate(m_RefractionTexture);
				m_RefractionTexture = null;
			}

			foreach (KeyValuePair<Camera, Camera> keyValuePair in m_ReflectionCameras)
			{
				DestroyImmediate(keyValuePair.Value.gameObject);
			}

			m_ReflectionCameras.Clear();
			foreach (KeyValuePair<Camera, Camera> keyValuePair2 in m_RefractionCameras)
			{
				DestroyImmediate(keyValuePair2.Value.gameObject);
			}

			m_RefractionCameras.Clear();
		}


		public void OnWillRenderObject()
		{
			if (!enabled || !GetComponent<Renderer>() || !GetComponent<Renderer>().sharedMaterial ||
			    !GetComponent<Renderer>().enabled)
			{
				return;
			}

			Camera current = Camera.current;
			if (!current)
			{
				return;
			}

			if (s_InsideWater)
			{
				return;
			}

			s_InsideWater = true;
			m_HardwareWaterSupport = FindHardwareWaterSupport();
			WaterMode waterMode = GetWaterMode();
			Camera camera;
			Camera camera2;
			CreateWaterObjects(current, out camera, out camera2);
			Vector3 position = transform.position;
			Vector3 up = transform.up;
			int pixelLightCount = QualitySettings.pixelLightCount;
			if (disablePixelLights)
			{
				QualitySettings.pixelLightCount = 0;
			}

			UpdateCameraModes(current, camera);
			UpdateCameraModes(current, camera2);
			if (waterMode >= WaterMode.Reflective)
			{
				float w = -Vector3.Dot(up, position) - clipPlaneOffset;
				Vector4 plane = new Vector4(up.x, up.y, up.z, w);
				Matrix4x4 zero = Matrix4x4.zero;
				CalculateReflectionMatrix(ref zero, plane);
				Vector3 position2 = current.transform.position;
				Vector3 position3 = zero.MultiplyPoint(position2);
				camera.worldToCameraMatrix = current.worldToCameraMatrix * zero;
				Vector4 clipPlane = CameraSpacePlane(camera, position, up, 1f);
				camera.projectionMatrix = current.CalculateObliqueMatrix(clipPlane);
				camera.cullingMatrix = current.projectionMatrix * current.worldToCameraMatrix;
				camera.cullingMask = -17 & reflectLayers.value;
				camera.targetTexture = m_ReflectionTexture;
				bool invertCulling = GL.invertCulling;
				GL.invertCulling = !invertCulling;
				camera.transform.position = position3;
				Vector3 eulerAngles = current.transform.eulerAngles;
				camera.transform.eulerAngles = new Vector3(-eulerAngles.x, eulerAngles.y, eulerAngles.z);
				camera.Render();
				camera.transform.position = position2;
				GL.invertCulling = invertCulling;
				GetComponent<Renderer>().sharedMaterial.SetTexture("_ReflectionTex", m_ReflectionTexture);
			}

			if (waterMode >= WaterMode.Refractive)
			{
				camera2.worldToCameraMatrix = current.worldToCameraMatrix;
				Vector4 clipPlane2 = CameraSpacePlane(camera2, position, up, -1f);
				camera2.projectionMatrix = current.CalculateObliqueMatrix(clipPlane2);
				camera2.cullingMatrix = current.projectionMatrix * current.worldToCameraMatrix;
				camera2.cullingMask = -17 & refractLayers.value;
				camera2.targetTexture = m_RefractionTexture;
				camera2.transform.position = current.transform.position;
				camera2.transform.rotation = current.transform.rotation;
				camera2.Render();
				GetComponent<Renderer>().sharedMaterial.SetTexture("_RefractionTex", m_RefractionTexture);
			}

			if (disablePixelLights)
			{
				QualitySettings.pixelLightCount = pixelLightCount;
			}

			switch (waterMode)
			{
				case WaterMode.Simple:
					Shader.EnableKeyword("WATER_SIMPLE");
					Shader.DisableKeyword("WATER_REFLECTIVE");
					Shader.DisableKeyword("WATER_REFRACTIVE");
					break;
				case WaterMode.Reflective:
					Shader.DisableKeyword("WATER_SIMPLE");
					Shader.EnableKeyword("WATER_REFLECTIVE");
					Shader.DisableKeyword("WATER_REFRACTIVE");
					break;
				case WaterMode.Refractive:
					Shader.DisableKeyword("WATER_SIMPLE");
					Shader.DisableKeyword("WATER_REFLECTIVE");
					Shader.EnableKeyword("WATER_REFRACTIVE");
					break;
			}

			s_InsideWater = false;
		}


		private void UpdateCameraModes(Camera src, Camera dest)
		{
			if (dest == null)
			{
				return;
			}

			dest.clearFlags = src.clearFlags;
			dest.backgroundColor = src.backgroundColor;
			if (src.clearFlags == CameraClearFlags.Skybox)
			{
				Skybox component = src.GetComponent<Skybox>();
				Skybox component2 = dest.GetComponent<Skybox>();
				if (!component || !component.material)
				{
					component2.enabled = false;
				}
				else
				{
					component2.enabled = true;
					component2.material = component.material;
				}
			}

			dest.farClipPlane = src.farClipPlane;
			dest.nearClipPlane = src.nearClipPlane;
			dest.orthographic = src.orthographic;
			dest.fieldOfView = src.fieldOfView;
			dest.aspect = src.aspect;
			dest.orthographicSize = src.orthographicSize;
		}


		private void CreateWaterObjects(Camera currentCamera, out Camera reflectionCamera, out Camera refractionCamera)
		{
			WaterMode waterMode = GetWaterMode();
			reflectionCamera = null;
			refractionCamera = null;
			if (waterMode >= WaterMode.Reflective)
			{
				if (!m_ReflectionTexture || m_OldReflectionTextureSize != textureSize)
				{
					if (m_ReflectionTexture)
					{
						DestroyImmediate(m_ReflectionTexture);
					}

					m_ReflectionTexture = new RenderTexture(textureSize, textureSize, 16);
					m_ReflectionTexture.name = "__WaterReflection" + GetInstanceID();
					m_ReflectionTexture.isPowerOfTwo = true;
					m_ReflectionTexture.hideFlags = HideFlags.DontSave;
					m_OldReflectionTextureSize = textureSize;
				}

				m_ReflectionCameras.TryGetValue(currentCamera, out reflectionCamera);
				if (!reflectionCamera)
				{
					GameObject gameObject =
						new GameObject(
							string.Concat("Water Refl Camera id", GetInstanceID(), " for ",
								currentCamera.GetInstanceID()), typeof(Camera), typeof(Skybox));
					reflectionCamera = gameObject.GetComponent<Camera>();
					reflectionCamera.enabled = false;
					reflectionCamera.transform.position = transform.position;
					reflectionCamera.transform.rotation = transform.rotation;
					reflectionCamera.gameObject.AddComponent<FlareLayer>();
					gameObject.hideFlags = HideFlags.HideAndDontSave;
					m_ReflectionCameras[currentCamera] = reflectionCamera;
				}
			}

			if (waterMode >= WaterMode.Refractive)
			{
				if (!m_RefractionTexture || m_OldRefractionTextureSize != textureSize)
				{
					if (m_RefractionTexture)
					{
						DestroyImmediate(m_RefractionTexture);
					}

					m_RefractionTexture = new RenderTexture(textureSize, textureSize, 16);
					m_RefractionTexture.name = "__WaterRefraction" + GetInstanceID();
					m_RefractionTexture.isPowerOfTwo = true;
					m_RefractionTexture.hideFlags = HideFlags.DontSave;
					m_OldRefractionTextureSize = textureSize;
				}

				m_RefractionCameras.TryGetValue(currentCamera, out refractionCamera);
				if (!refractionCamera)
				{
					GameObject gameObject2 =
						new GameObject(
							string.Concat("Water Refr Camera id", GetInstanceID(), " for ",
								currentCamera.GetInstanceID()), typeof(Camera), typeof(Skybox));
					refractionCamera = gameObject2.GetComponent<Camera>();
					refractionCamera.enabled = false;
					refractionCamera.transform.position = transform.position;
					refractionCamera.transform.rotation = transform.rotation;
					refractionCamera.gameObject.AddComponent<FlareLayer>();
					gameObject2.hideFlags = HideFlags.HideAndDontSave;
					m_RefractionCameras[currentCamera] = refractionCamera;
				}
			}
		}


		private WaterMode GetWaterMode()
		{
			if (m_HardwareWaterSupport < waterMode)
			{
				return m_HardwareWaterSupport;
			}

			return waterMode;
		}


		private WaterMode FindHardwareWaterSupport()
		{
			if (!GetComponent<Renderer>())
			{
				return WaterMode.Simple;
			}

			Material sharedMaterial = GetComponent<Renderer>().sharedMaterial;
			if (!sharedMaterial)
			{
				return WaterMode.Simple;
			}

			string tag = sharedMaterial.GetTag("WATERMODE", false);
			if (tag == "Refractive")
			{
				return WaterMode.Refractive;
			}

			if (tag == "Reflective")
			{
				return WaterMode.Reflective;
			}

			return WaterMode.Simple;
		}


		private Vector4 CameraSpacePlane(Camera cam, Vector3 pos, Vector3 normal, float sideSign)
		{
			Vector3 point = pos + normal * clipPlaneOffset;
			Matrix4x4 worldToCameraMatrix = cam.worldToCameraMatrix;
			Vector3 lhs = worldToCameraMatrix.MultiplyPoint(point);
			Vector3 vector = worldToCameraMatrix.MultiplyVector(normal).normalized * sideSign;
			return new Vector4(vector.x, vector.y, vector.z, -Vector3.Dot(lhs, vector));
		}


		private static void CalculateReflectionMatrix(ref Matrix4x4 reflectionMat, Vector4 plane)
		{
			reflectionMat.m00 = 1f - 2f * plane[0] * plane[0];
			reflectionMat.m01 = -2f * plane[0] * plane[1];
			reflectionMat.m02 = -2f * plane[0] * plane[2];
			reflectionMat.m03 = -2f * plane[3] * plane[0];
			reflectionMat.m10 = -2f * plane[1] * plane[0];
			reflectionMat.m11 = 1f - 2f * plane[1] * plane[1];
			reflectionMat.m12 = -2f * plane[1] * plane[2];
			reflectionMat.m13 = -2f * plane[3] * plane[1];
			reflectionMat.m20 = -2f * plane[2] * plane[0];
			reflectionMat.m21 = -2f * plane[2] * plane[1];
			reflectionMat.m22 = 1f - 2f * plane[2] * plane[2];
			reflectionMat.m23 = -2f * plane[3] * plane[2];
			reflectionMat.m30 = 0f;
			reflectionMat.m31 = 0f;
			reflectionMat.m32 = 0f;
			reflectionMat.m33 = 1f;
		}
	}
}