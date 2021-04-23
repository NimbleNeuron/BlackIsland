using System;
using UnityEngine;

namespace Coffee.UIExtensions
{
	
	[ExecuteInEditMode]
	public class UIParticleOverlayCamera : MonoBehaviour
	{
		
		
		public static UIParticleOverlayCamera instance {
			get
			{
				if (s_Instance == null)
				{
					UIParticleOverlayCamera uiparticleOverlayCamera;
					if ((uiparticleOverlayCamera = FindObjectOfType<UIParticleOverlayCamera>()) ==
					    null)
					{
						uiparticleOverlayCamera = new GameObject(typeof(UIParticleOverlayCamera).Name, new Type[]
						{
							typeof(UIParticleOverlayCamera)
						}).GetComponent<UIParticleOverlayCamera>();
					}

					s_Instance = uiparticleOverlayCamera;
					s_Instance.gameObject.SetActive(true);
					s_Instance.enabled = true;
				}

				return s_Instance;
			}
		}

		
		public static Camera GetCameraForOvrelay(Canvas canvas)
		{
			UIParticleOverlayCamera instance = UIParticleOverlayCamera.instance;
			RectTransform rectTransform = canvas.rootCanvas.transform as RectTransform;
			Camera cameraForOvrelay = instance.cameraForOvrelay;
			Transform transform = instance.transform;
			cameraForOvrelay.enabled = false;
			Vector3 localPosition = rectTransform.localPosition;
			cameraForOvrelay.orthographic = true;
			cameraForOvrelay.orthographicSize = Mathf.Max(localPosition.x, localPosition.y);
			cameraForOvrelay.nearClipPlane = 0.3f;
			cameraForOvrelay.farClipPlane = 1000f;
			localPosition.z -= 100f;
			transform.localPosition = localPosition;
			return cameraForOvrelay;
		}

		
		
		private Camera cameraForOvrelay {
			get
			{
				if (m_Camera)
				{
					return m_Camera;
				}

				if (!(m_Camera = GetComponent<Camera>()))
				{
					return m_Camera = gameObject.AddComponent<Camera>();
				}

				return m_Camera;
			}
		}

		
		private void Awake()
		{
			if (s_Instance == null)
			{
				s_Instance = GetComponent<UIParticleOverlayCamera>();
			}
			else if (s_Instance != this)
			{
				Debug.LogWarning("Multiple " + typeof(UIParticleOverlayCamera).Name + " in scene.", gameObject);
				enabled = false;
				Destroy(gameObject);
				return;
			}

			cameraForOvrelay.enabled = false;
			if (Application.isPlaying)
			{
				DontDestroyOnLoad(gameObject);
			}
		}

		
		private void OnDestroy()
		{
			if (s_Instance == this)
			{
				s_Instance = null;
			}
		}

		
		private Camera m_Camera;

		
		private static UIParticleOverlayCamera s_Instance;
	}
}