//------------------------------------------------------------------------------------------------------------------
// Volumetric Fog & Mist
// Created by Ramiro Oliva (Kronnect)
//------------------------------------------------------------------------------------------------------------------

using UnityEngine;

namespace VolumetricFogAndMist
{
	[ExecuteInEditMode]
	[AddComponentMenu("")]
	[RequireComponent(typeof(Camera))]
	[ImageEffectAllowedInSceneView]
	public class VolumetricFogPosT : MonoBehaviour, IVolumetricFogRenderComponent
	{
		private Material copyOpaqueMat;

		public void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
#if UNITY_EDITOR
			if (Camera.current.cameraType == CameraType.SceneView)
			{
				if (fog == null)
				{
					fog = VolumetricFog.instance;
				}

				if (fog != null && !fog.showInSceneView)
				{
					fog = null;
				}
			}
#endif

			if (fog == null || !fog.enabled)
			{
				Graphics.Blit(source, destination);
				return;
			}

			if (fog.transparencyBlendMode == TRANSPARENT_MODE.None)
			{
				fog.DoOnRenderImage(source, destination);
			}
			else
			{
				RenderTextureDescriptor desc = source.descriptor;
				RenderTexture opaqueFrame = RenderTexture.GetTemporary(desc);
				if (copyOpaqueMat == null)
				{
					copyOpaqueMat = new Material(Shader.Find("VolumetricFogAndMist/CopyOpaque"));
				}

				copyOpaqueMat.SetFloat("_BlendPower", fog.transparencyBlendPower);
				Graphics.Blit(source, destination, copyOpaqueMat, fog.computeDepth && fog.downsampling == 1 ? 1 : 0);
				RenderTexture.ReleaseTemporary(opaqueFrame);
			}
		}

		public VolumetricFog fog { get; set; }

		public void DestroySelf()
		{
			DestroyImmediate(this);
		}
	}
}