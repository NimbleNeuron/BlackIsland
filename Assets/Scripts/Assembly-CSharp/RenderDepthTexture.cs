using UnityEngine;


[ExecuteInEditMode]
public class RenderDepthTexture : MonoBehaviour
{
	
	private void OnEnable()
	{
		base.GetComponent<Camera>().depthTextureMode = DepthTextureMode.DepthNormals;
	}
}
