using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace Blis.Client
{
	public class PostProcessVolumeControl : MonoBehaviour
	{
		[SerializeField] private Camera mainCamera;


		[SerializeField] private CameraControl mainCameraControl;


		[SerializeField] private PostProcessVolume postProcessVolume;


		public Vector2 focalLengthBounds = new Vector2(80f, 120f);


		private DepthOfField depthOfField;

		public void Awake()
		{
			GameObject gameObject = GameObject.FindGameObjectWithTag("MainCamera");
			mainCamera = gameObject.GetComponent<Camera>();
			mainCameraControl = gameObject.GetComponent<CameraControl>();
			postProcessVolume = GetComponent<PostProcessVolume>();
			depthOfField = postProcessVolume.profile.GetSetting<DepthOfField>();
		}


		public void LateUpdate()
		{
			float num = (mainCameraControl.currentZoom - mainCameraControl.minZoom) /
			            (mainCameraControl.maxZoom - mainCameraControl.minZoom);
			depthOfField.focalLength.value = num * (focalLengthBounds.x - focalLengthBounds.y) + focalLengthBounds.y;
		}
	}
}