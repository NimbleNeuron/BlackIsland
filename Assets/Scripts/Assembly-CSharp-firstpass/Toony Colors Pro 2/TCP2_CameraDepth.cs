using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class TCP2_CameraDepth : MonoBehaviour
{
	public bool RenderDepth = true;


	private void OnEnable()
	{
		SetCameraDepth();
	}


	private void OnValidate()
	{
		SetCameraDepth();
	}


	private void SetCameraDepth()
	{
		Camera component = GetComponent<Camera>();
		if (RenderDepth)
		{
			component.depthTextureMode |= DepthTextureMode.Depth;
			return;
		}

		component.depthTextureMode &= ~DepthTextureMode.Depth;
	}
}