using UnityEngine;
using UnityEngine.Rendering;

namespace BIFog
{
	
	[RequireComponent(typeof(Camera))]
	public class FogPostEffect : MonoBehaviour
	{
		
		
		public static FogPostEffect Instance
		{
			get
			{
				return FogPostEffect.instance;
			}
		}

		
		private void Awake()
		{
			FogPostEffect.instance = this;
			this.commandBuffer = new CommandBuffer();
			this.targetCamera = base.GetComponent<Camera>();
			this.fogMat = new Material(Shader.Find("Hidden/BIFog"));
		}

		
		private void OnEnable()
		{
			this.commandBuffer.GetTemporaryRT(FogPostEffect.fogTempRT, -1, -1, 0, FilterMode.Bilinear);
			this.commandBuffer.Blit(BuiltinRenderTextureType.CameraTarget, FogPostEffect.fogTempRT);
			this.commandBuffer.SetGlobalTexture(FogPostEffect.mainTex, FogPostEffect.fogTempRT);
			this.commandBuffer.Blit(BuiltinRenderTextureType.None, BuiltinRenderTextureType.CameraTarget, this.fogMat);
			this.commandBuffer.ReleaseTemporaryRT(FogPostEffect.fogTempRT);
			this.targetCamera.AddCommandBuffer(CameraEvent.AfterImageEffectsOpaque, this.commandBuffer);
		}

		
		private void OnDisable()
		{
			this.targetCamera.RemoveCommandBuffer(CameraEvent.AfterImageEffectsOpaque, this.commandBuffer);
		}

		
		private void OnPreRender()
		{
			this.fogMat.SetMatrix("_InverseViewMatrix", this.targetCamera.worldToCameraMatrix.inverse);
		}

		
		private void OnDestroy()
		{
			UnityEngine.Object.Destroy(this.fogMat);
		}

		
		private static readonly int fogTempRT = Shader.PropertyToID("_fogTempRT");

		
		private static readonly int mainTex = Shader.PropertyToID("_MainTex");

		
		private static FogPostEffect instance;

		
		private Camera targetCamera;

		
		private CommandBuffer commandBuffer;

		
		private Material fogMat;
	}
}
