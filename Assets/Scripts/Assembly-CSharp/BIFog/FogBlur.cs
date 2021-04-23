using System;
using System.Collections.Generic;
using UnityEngine;

namespace BIFog
{
	
	[ExecuteInEditMode]
	public class FogBlur : FogPostProcessor
	{
		
		protected void Start()
		{
			this.CheckResources();
		}

		
		private void OnEnable()
		{
			this._blurShader = Shader.Find("Hidden/BI/FogBlur");
			this.isSupported = true;
		}

		
		private void OnDestroy()
		{
			this.RemoveCreatedMaterials();
		}

		
		private void RemoveCreatedMaterials()
		{
			while (this.createdMaterials.Count > 0)
			{
				UnityEngine.Object obj = this.createdMaterials[0];
				this.createdMaterials.RemoveAt(0);
				UnityEngine.Object.Destroy(obj);
			}
		}

		
		private bool CheckResources()
		{
			this.CheckSupport(false);
			if (this.blurMaterial)
			{
				UnityEngine.Object.DestroyImmediate(this.blurMaterial);
			}
			this.blurMaterial = this.CheckShaderAndCreateMaterial(this._blurShader, this.blurMaterial);
			if (!this.isSupported)
			{
				this.ReportAutoDisable();
			}
			return this.isSupported;
		}

		
		private void OnDisable()
		{
			if (this.blurMaterial)
			{
				UnityEngine.Object.DestroyImmediate(this.blurMaterial);
			}
		}

		
		public override void Process(RenderTexture source, RenderTexture destination)
		{
			if (!this.CheckResources())
			{
				Graphics.Blit(source, destination);
				return;
			}
			float num = 1f / (1f * (float)(1 << this.downsample));
			try
			{
				this.blurMaterial.SetVector("_Parameter", new Vector4(this.blurSize * num, -this.blurSize * num, 0f, 0f));
				source.filterMode = FilterMode.Bilinear;
				int width = source.width >> this.downsample;
				int height = source.height >> this.downsample;
				RenderTexture renderTexture = RenderTexture.GetTemporary(width, height, 0, source.format);
				renderTexture.filterMode = FilterMode.Bilinear;
				Graphics.Blit(source, renderTexture, this.blurMaterial, 0);
				int num2 = (this.blurType == FogBlur.BlurType.StandardGauss) ? 0 : 2;
				for (int i = 0; i < this.blurIterations; i++)
				{
					float num3 = (float)i * 1f;
					this.blurMaterial.SetVector("_Parameter", new Vector4(this.blurSize * num + num3, -this.blurSize * num - num3, 0f, 0f));
					RenderTexture temporary = RenderTexture.GetTemporary(width, height, 0, source.format);
					temporary.filterMode = FilterMode.Bilinear;
					Graphics.Blit(renderTexture, temporary, this.blurMaterial, 1 + num2);
					RenderTexture.ReleaseTemporary(renderTexture);
					renderTexture = temporary;
					temporary = RenderTexture.GetTemporary(width, height, 0, source.format);
					temporary.filterMode = FilterMode.Bilinear;
					Graphics.Blit(renderTexture, temporary, this.blurMaterial, 2 + num2);
					RenderTexture.ReleaseTemporary(renderTexture);
					renderTexture = temporary;
				}
				Graphics.Blit(renderTexture, destination);
				RenderTexture.ReleaseTemporary(renderTexture);
			}
			catch (Exception e)
			{
				Debug.LogWarning(e);
			}
		}

		
		private bool CheckSupport(bool needDepth)
		{
			this.isSupported = true;
			this.supportHDRTextures = SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBHalf);
			this.supportDX11 = (SystemInfo.graphicsShaderLevel >= 50 && SystemInfo.supportsComputeShaders);
			// if (!SystemInfo.supportsImageEffects)
			// {
			// 	this.NotSupported();
			// 	return false;
			// }
			if (needDepth && !SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.Depth))
			{
				this.NotSupported();
				return false;
			}
			if (needDepth)
			{
				base.GetComponent<Camera>().depthTextureMode |= DepthTextureMode.Depth;
			}
			return true;
		}

		
		private Material CheckShaderAndCreateMaterial(Shader s, Material m2Create)
		{
			if (!s)
			{
				Debug.Log("Missing shader in " + this.ToString());
				base.enabled = false;
				return null;
			}
			if (s.isSupported && m2Create && m2Create.shader == s)
			{
				return m2Create;
			}
			if (!s.isSupported)
			{
				this.NotSupported();
				Debug.Log(string.Concat(new string[]
				{
					"The shader ",
					s.ToString(),
					" on effect ",
					this.ToString(),
					" is not supported on this platform!"
				}));
				return null;
			}
			m2Create = new Material(s);
			this.createdMaterials.Add(m2Create);
			m2Create.hideFlags = HideFlags.DontSave;
			return m2Create;
		}

		
		private void NotSupported()
		{
			base.enabled = false;
			this.isSupported = false;
		}

		
		private void ReportAutoDisable()
		{
			Debug.LogWarning("The image effect " + this.ToString() + " has been disabled as it's not supported on the current platform.");
		}

		
		[Range(0f, 2f)]
		public int downsample = 1;

		
		[Range(0f, 10f)]
		public float blurSize = 1f;

		
		[Range(1f, 4f)]
		public int blurIterations = 1;

		
		public FogBlur.BlurType blurType;

		
		private Shader _blurShader;

		
		private Material blurMaterial;

		
		private bool supportHDRTextures = true;

		
		private bool supportDX11;

		
		private bool isSupported = true;

		
		private List<Material> createdMaterials = new List<Material>();

		
		public enum BlurType
		{
			
			StandardGauss,
			
			SgxGauss
		}
	}
}
