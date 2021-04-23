using UnityEngine;
using UnityEngine.Rendering;

namespace VolumetricFogAndMist
{
	[ExecuteInEditMode]
	public class ShadowMapCopy : MonoBehaviour
	{
		private CommandBuffer cb;

		private Light m_Light;

		private void OnEnable()
		{
			m_Light = GetComponent<Light>();
			if (m_Light == null)
			{
				Debug.LogError(
					"Light component not found on this gameobject. Make sure you have assigned a valid directional light to the Sun property of Volumetric Fog & Mist.");
				return;
			}

			cb = new CommandBuffer();
			cb.name = "Volumetric Fog ShadowMap Copy";

			cb.SetGlobalTexture("_VolumetricFogShadowMapCopy",
				new RenderTargetIdentifier(BuiltinRenderTextureType.CurrentActive));

			// Execute after the shadowmap has been filled.
			m_Light.AddCommandBuffer(LightEvent.AfterShadowMap, cb);
		}

		private void OnDisable()
		{
			if (m_Light != null)
			{
				m_Light.RemoveCommandBuffer(LightEvent.AfterShadowMap, cb);
			}
		}
	}
}