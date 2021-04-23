using UnityEngine;

public class TCP2_ShaderUpdateUnityTime : MonoBehaviour
{
	private void LateUpdate()
	{
		Shader.SetGlobalFloat("unityTime", Time.time);
	}
}