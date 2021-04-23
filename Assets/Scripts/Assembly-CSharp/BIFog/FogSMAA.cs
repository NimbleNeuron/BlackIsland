using UnityEngine;

namespace BIFog
{
	
	public class FogSMAA : FogPostProcessor
	{
		
		private Material CreateSMAA()
		{
			return new Material(Shader.Find("Hidden/SMAA"));
		}

		
		protected void Awake()
		{
			if (this._smaaMat != null)
			{
				UnityEngine.Object.Destroy(this._smaaMat);
			}
			this._smaaMat = this.CreateSMAA();
		}

		
		public override void Process(RenderTexture source, RenderTexture dest)
		{
			this._smaaMat.SetVector("_Metrics", new Vector4(1f / (float)source.width, 1f / (float)source.height, (float)source.width, (float)source.height));
			this._smaaMat.SetVector("_Params1", new Vector4(this._threshold, (float)this._maxSearchSteps, (float)this._maxSearchStepsDiag));
			this._smaaMat.SetVector("_Params2", new Vector2((float)this._cornerRounding, this._localContrastAdaptationFactor));
			RenderTexture temporary = RenderTexture.GetTemporary(source.width, source.height, source.depth, source.format);
			RenderTexture temporary2 = RenderTexture.GetTemporary(source.width, source.height, source.depth, source.format);
			Graphics.SetRenderTarget(temporary);
			GL.Clear(false, false, Color.clear);
			Graphics.Blit(source, temporary, this._smaaMat, (int)this._smaaType);
			this._smaaMat.SetTexture("_BlendTex", temporary);
			Graphics.Blit(source, temporary2, this._smaaMat, 0);
			Graphics.CopyTexture(temporary2, dest);
			RenderTexture.ReleaseTemporary(temporary);
			RenderTexture.ReleaseTemporary(temporary2);
		}

		
		private void OnDestroy()
		{
			UnityEngine.Object.Destroy(this._smaaMat);
		}

		
		[SerializeField]
		private FogSMAA.SMAAType _smaaType = FogSMAA.SMAAType.LUMA;

		
		[SerializeField]
		[Range(0f, 0.5f)]
		private float _threshold = 0.1f;

		
		[SerializeField]
		[Range(0f, 112f)]
		private int _maxSearchSteps = 16;

		
		[SerializeField]
		[Range(0f, 20f)]
		private int _maxSearchStepsDiag = 8;

		
		[SerializeField]
		[Range(0f, 100f)]
		private int _cornerRounding = 25;

		
		[SerializeField]
		[Min(0f)]
		private float _localContrastAdaptationFactor = 2f;

		
		private Material _smaaMat;

		
		public enum SMAAType
		{
			
			LUMA = 1,
			
			COLOR,
			
			Depth
		}
	}
}
