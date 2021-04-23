using System.Collections;
using UnityEngine;

[ExecuteInEditMode]
public class TCP2_PlanarReflection : MonoBehaviour
{
	private static bool s_InsideRendering;


	public bool m_DisablePixelLights;


	public int m_TextureSize = 1024;


	public float m_ClipPlaneOffset = 0.07f;


	public LayerMask m_ReflectLayers = -1;


	private readonly Hashtable m_ReflectionCameras = new Hashtable();


	private int m_OldReflectionTextureSize;


	private RenderTexture m_ReflectionTexture;


	private void OnDisable()
	{
		if (m_ReflectionTexture)
		{
			DestroyImmediate(m_ReflectionTexture);
			m_ReflectionTexture = null;
		}

		foreach (object obj in m_ReflectionCameras)
		{
			DestroyImmediate(((Camera) ((DictionaryEntry) obj).Value).gameObject);
		}

		m_ReflectionCameras.Clear();
	}


	public void OnWillRenderObject()
	{
		Renderer component = GetComponent<Renderer>();
		if (!enabled || !component || !component.sharedMaterial || !component.enabled)
		{
			return;
		}

		Camera current = Camera.current;
		if (!current)
		{
			return;
		}

		if (s_InsideRendering)
		{
			return;
		}

		s_InsideRendering = true;
		Camera camera;
		CreateMirrorObjects(current, out camera);
		Vector3 position = transform.position;
		Vector3 up = transform.up;
		int pixelLightCount = QualitySettings.pixelLightCount;
		if (m_DisablePixelLights)
		{
			QualitySettings.pixelLightCount = 0;
		}

		UpdateCameraModes(current, camera);
		float w = -Vector3.Dot(up, position) - m_ClipPlaneOffset;
		Vector4 plane = new Vector4(up.x, up.y, up.z, w);
		Matrix4x4 zero = Matrix4x4.zero;
		CalculateReflectionMatrix(ref zero, plane);
		Vector3 position2 = current.transform.position;
		Vector3 position3 = zero.MultiplyPoint(position2);
		camera.worldToCameraMatrix = current.worldToCameraMatrix * zero;
		Vector4 clipPlane = CameraSpacePlane(camera, position, up, 1f);
		Matrix4x4 projectionMatrix = current.CalculateObliqueMatrix(clipPlane);
		camera.projectionMatrix = projectionMatrix;
		camera.cullingMask = -17 & m_ReflectLayers.value;
		camera.targetTexture = m_ReflectionTexture;
		GL.invertCulling = true;
		camera.transform.position = position3;
		Vector3 eulerAngles = current.transform.eulerAngles;
		camera.transform.eulerAngles = new Vector3(0f, eulerAngles.y, eulerAngles.z);
		camera.Render();
		camera.transform.position = position2;
		GL.invertCulling = false;
		foreach (Material material in component.sharedMaterials)
		{
			if (material.HasProperty("_ReflectionTex"))
			{
				material.SetTexture("_ReflectionTex", m_ReflectionTexture);
			}
		}

		if (m_DisablePixelLights)
		{
			QualitySettings.pixelLightCount = pixelLightCount;
		}

		s_InsideRendering = false;
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
			Skybox skybox = src.GetComponent(typeof(Skybox)) as Skybox;
			Skybox skybox2 = dest.GetComponent(typeof(Skybox)) as Skybox;
			if (!skybox || !skybox.material)
			{
				skybox2.enabled = false;
			}
			else
			{
				skybox2.enabled = true;
				skybox2.material = skybox.material;
			}
		}

		dest.farClipPlane = src.farClipPlane;
		dest.nearClipPlane = src.nearClipPlane;
		dest.orthographic = src.orthographic;
		dest.fieldOfView = src.fieldOfView;
		dest.aspect = src.aspect;
		dest.orthographicSize = src.orthographicSize;
	}


	private void CreateMirrorObjects(Camera currentCamera, out Camera reflectionCamera)
	{
		reflectionCamera = null;
		if (!m_ReflectionTexture || m_OldReflectionTextureSize != m_TextureSize)
		{
			if (m_ReflectionTexture)
			{
				DestroyImmediate(m_ReflectionTexture);
			}

			m_ReflectionTexture = new RenderTexture(m_TextureSize, m_TextureSize, 16);
			m_ReflectionTexture.name = "__MirrorReflection" + GetInstanceID();
			m_ReflectionTexture.isPowerOfTwo = true;
			m_ReflectionTexture.hideFlags = HideFlags.DontSave;
			m_OldReflectionTextureSize = m_TextureSize;
		}

		reflectionCamera = m_ReflectionCameras[currentCamera] as Camera;
		if (!reflectionCamera)
		{
			GameObject gameObject =
				new GameObject(
					string.Concat("Mirror Refl Camera id", GetInstanceID(), " for ", currentCamera.GetInstanceID()),
					typeof(Camera), typeof(Skybox));
			reflectionCamera = gameObject.GetComponent<Camera>();
			reflectionCamera.enabled = false;
			reflectionCamera.transform.position = transform.position;
			reflectionCamera.transform.rotation = transform.rotation;
			reflectionCamera.gameObject.AddComponent<FlareLayer>();
			gameObject.hideFlags = HideFlags.HideAndDontSave;
			m_ReflectionCameras[currentCamera] = reflectionCamera;
		}
	}


	private static float sgn(float a)
	{
		if (a > 0f)
		{
			return 1f;
		}

		if (a < 0f)
		{
			return -1f;
		}

		return 0f;
	}


	private Vector4 CameraSpacePlane(Camera cam, Vector3 pos, Vector3 normal, float sideSign)
	{
		Vector3 point = pos + normal * m_ClipPlaneOffset;
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