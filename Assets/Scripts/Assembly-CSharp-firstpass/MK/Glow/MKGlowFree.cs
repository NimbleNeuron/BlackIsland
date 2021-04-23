using UnityEngine;

namespace MK.Glow
{
	[ExecuteInEditMode]
	[ImageEffectAllowedInSceneView]
	[RequireComponent(typeof(Camera))]
	public class MKGlowFree : MonoBehaviour
	{
		private const float GLOW_INTENSITY_MULT = 12.5f;


		private const float BLUR_SPREAD_INNTER_MULT = 10f;


		private const float BLUR_SPREAD_OUTER_MULT = 50f;


		private static readonly float[] gaussFilter =
		{
			0.5f,
			0.5398f,
			0.5793f,
			0.6179f,
			0.6554f,
			0.6915f,
			0.7257f,
			0.758f,
			0.7881f,
			0.8159f,
			0.8413f
		};


		[SerializeField] private RenderTextureFormat rtFormat = RenderTextureFormat.Default;


		[SerializeField] private Shader blurShader;


		[SerializeField] private Shader compositeShader;


		[SerializeField] private Shader selectiveRenderShader;


		[SerializeField] [Tooltip("recommend: -1")]
		private LayerMask glowLayer = -1;


		[SerializeField]
		[Tooltip("Selective = to specifically bring objects to glow, Fullscreen = complete screen glows")]
		private GlowType glowType;


		[SerializeField] [Tooltip("The glows coloration")]
		private Color glowTint = new Color(1f, 1f, 1f, 0f);


		[SerializeField] [Tooltip("Inner width of the glow effect")]
		private float blurSpreadInner = 0.6f;


		[SerializeField] [Tooltip("Number of used blurs. Lower iterations = better performance")]
		private int blurIterations = 5;


		[SerializeField] [Tooltip("The global inner luminous intensity")]
		private float glowIntensityInner = 0.4f;


		[SerializeField]
		[Tooltip("Downsampling steps of the blur. Higher samples = better performance, but gains more flickering")]
		private int samples = 2;


		private Material blurMaterial;


		private Material compositeMaterial;


		private RenderTexture glowTexRaw;


		private Camera selectiveGlowCamera;


		private GameObject selectiveGlowCameraObject;


		private int srcHeight;


		private VRTextureUsage srcVRUsage = VRTextureUsage.TwoEyes;


		private int srcWidth;


		[SerializeField] private Camera Cam => GetComponent<Camera>();


		
		public LayerMask GlowLayer {
			get => glowLayer;
			set => glowLayer = value;
		}


		
		public GlowType GlowType {
			get => glowType;
			set => glowType = value;
		}


		
		public Color GlowTint {
			get => glowTint;
			set => glowTint = value;
		}


		
		public int Samples {
			get => samples;
			set => samples = value;
		}


		
		public int BlurIterations {
			get => blurIterations;
			set => blurIterations = Mathf.Clamp(value, 0, 10);
		}


		
		public float GlowIntensityInner {
			get => glowIntensityInner;
			set => glowIntensityInner = value;
		}


		
		public float BlurSpreadInner {
			get => blurSpreadInner;
			set => blurSpreadInner = value;
		}


		private GameObject SelectiveGlowCameraObject {
			get
			{
				if (!selectiveGlowCameraObject)
				{
					selectiveGlowCameraObject = new GameObject("selectiveGlowCameraObject");
					selectiveGlowCameraObject.AddComponent<Camera>();
					selectiveGlowCameraObject.hideFlags = HideFlags.HideAndDontSave;
					SelectiveGlowCamera.orthographic = false;
					SelectiveGlowCamera.enabled = false;
					SelectiveGlowCamera.renderingPath = RenderingPath.VertexLit;
					SelectiveGlowCamera.hideFlags = HideFlags.HideAndDontSave;
				}

				return selectiveGlowCameraObject;
			}
		}


		private Camera SelectiveGlowCamera {
			get
			{
				if (selectiveGlowCamera == null)
				{
					selectiveGlowCamera = SelectiveGlowCameraObject.GetComponent<Camera>();
				}

				return selectiveGlowCamera;
			}
		}


		private void Awake()
		{
			GlowInitialize();
		}


		private void Reset()
		{
			GlowInitialize();
		}


		private void OnEnable()
		{
			GlowInitialize();
		}


		private void OnDisable()
		{
			Cleanup();
		}


		private void OnDestroy()
		{
			Cleanup();
		}


		private void OnPostRender()
		{
			GlowType glowType = this.glowType;
			if (glowType != GlowType.Selective)
			{
				if (glowType != GlowType.Fullscreen) { }
			}
			else
			{
				RenderTexture.ReleaseTemporary(glowTexRaw);
				glowTexRaw = RenderTexture.GetTemporary(Cam.pixelWidth / samples, Cam.pixelHeight / samples, 16,
					rtFormat, RenderTextureReadWrite.Default, 1, RenderTextureMemoryless.None, srcVRUsage);
				SetupGlowCamera();
				SelectiveGlowCamera.RenderWithShader(selectiveRenderShader, "RenderType");
			}

			blurMaterial.SetFloat("_VRMult", Cam.stereoEnabled ? 0.5f : 1f);
			compositeMaterial.SetFloat("_GlowIntensityInner",
				glowIntensityInner * (this.glowType != GlowType.Fullscreen ? 12.5f * blurSpreadInner : 10f));
			compositeMaterial.SetColor("_GlowTint", glowTint);
		}


		private void OnRenderImage(RenderTexture src, RenderTexture dest)
		{
			rtFormat = src.format;
			srcWidth = src.width / samples;
			srcHeight = src.height / samples;
			srcVRUsage = src.vrUsage;
			RenderTexture temporaryRT = GetTemporaryRT(srcWidth, srcHeight, src.vrUsage);
			RenderTexture temporaryRT2 = GetTemporaryRT(srcWidth, srcHeight, src.vrUsage);
			GlowType glowType = this.glowType;
			if (glowType != GlowType.Selective)
			{
				if (glowType == GlowType.Fullscreen)
				{
					FullScreenGlow(src, dest, temporaryRT, temporaryRT2);
				}
			}
			else
			{
				SelectiveGlow(src, dest, temporaryRT, temporaryRT2);
			}

			RenderTexture.ReleaseTemporary(temporaryRT);
			RenderTexture.ReleaseTemporary(temporaryRT2);
		}


		public void GlowInitialize()
		{
			Cleanup();
			SetupShaders();
			SetupMaterials();
		}


		private void SetupShaders()
		{
			if (!blurShader)
			{
				blurShader = Shader.Find("Hidden/MK/Glow/Blur");
			}

			if (!compositeShader)
			{
				compositeShader = Shader.Find("Hidden/MK/Glow/Composite");
			}

			if (!selectiveRenderShader)
			{
				selectiveRenderShader = Shader.Find("Hidden/MK/Glow/SelectiveRender");
			}
		}


		private void Cleanup()
		{
			DestroyImmediate(selectiveGlowCamera);
			DestroyImmediate(SelectiveGlowCameraObject);
		}


		private RenderTexture GetTemporaryRT(int width, int height, VRTextureUsage vrUsage)
		{
			return RenderTexture.GetTemporary(width, height, 0, rtFormat, RenderTextureReadWrite.Default, 1,
				RenderTextureMemoryless.None, vrUsage);
		}


		private void Blur(RenderTexture main, RenderTexture tmpMain)
		{
			for (int i = 1; i <= blurIterations; i++)
			{
				float num = i * (blurSpreadInner * 10f) / blurIterations / samples;
				num *= gaussFilter[i];
				blurMaterial.SetFloat("_Offset", num);
				Graphics.Blit(main, tmpMain, blurMaterial);
				blurMaterial.SetFloat("_Offset", num);
				Graphics.Blit(tmpMain, main, blurMaterial);
			}
		}


		private void SetupMaterials()
		{
			if (blurMaterial == null)
			{
				blurMaterial = new Material(blurShader);
				blurMaterial.hideFlags = HideFlags.HideAndDontSave;
			}

			if (compositeMaterial == null)
			{
				compositeMaterial = new Material(compositeShader);
				compositeMaterial.hideFlags = HideFlags.HideAndDontSave;
			}
		}


		private void SetupGlowCamera()
		{
			SelectiveGlowCamera.CopyFrom(Cam);
			SelectiveGlowCamera.depthTextureMode = DepthTextureMode.None;
			SelectiveGlowCamera.targetTexture = glowTexRaw;
			SelectiveGlowCamera.clearFlags = CameraClearFlags.Color;
			SelectiveGlowCamera.rect = new Rect(0f, 0f, 1f, 1f);
			SelectiveGlowCamera.backgroundColor = new Color(0f, 0f, 0f, 0f);
			SelectiveGlowCamera.cullingMask = glowLayer;
			SelectiveGlowCamera.renderingPath = RenderingPath.VertexLit;
		}


		private void FullScreenGlow(RenderTexture src, RenderTexture dest, RenderTexture glowTexInner,
			RenderTexture tmpGlowTex)
		{
			Graphics.Blit(src, glowTexInner);
			Blur(glowTexInner, tmpGlowTex);
			compositeMaterial.SetTexture("_MKGlowTexInner", glowTexInner);
			Graphics.Blit(src, dest, compositeMaterial, 1);
		}


		private void SelectiveGlow(RenderTexture src, RenderTexture dest, RenderTexture glowTexInner,
			RenderTexture tmpGlowTex)
		{
			Graphics.Blit(glowTexRaw, glowTexInner);
			Blur(glowTexInner, tmpGlowTex);
			compositeMaterial.SetTexture("_MKGlowTexInner", glowTexInner);
			Graphics.Blit(src, dest, compositeMaterial);
		}
	}
}