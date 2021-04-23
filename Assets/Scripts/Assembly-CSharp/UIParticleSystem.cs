using UnityEngine;
using UnityEngine.Sprites;
using UnityEngine.UI;


[ExecuteInEditMode]
[RequireComponent(typeof(CanvasRenderer))]
[RequireComponent(typeof(ParticleSystem))]
public class UIParticleSystem : MaskableGraphic
{
	
	
	public override Texture mainTexture
	{
		get
		{
			if (this.particleTexture)
			{
				return this.particleTexture;
			}
			if (this.particleSprite)
			{
				return this.particleSprite.texture;
			}
			return null;
		}
	}

	
	protected bool Initialize()
	{
		if (this._transform == null)
		{
			this._transform = base.transform;
		}
		ParticleSystemRenderer particleSystemRenderer = base.GetComponent<ParticleSystemRenderer>();
		bool flag = false;
		if (this._particleSystem == null)
		{
			this._particleSystem = base.GetComponent<ParticleSystem>();
			if (this._particleSystem == null)
			{
				return false;
			}
			if (particleSystemRenderer == null)
			{
				particleSystemRenderer = this._particleSystem.gameObject.AddComponent<ParticleSystemRenderer>();
			}
			Material sharedMaterial = particleSystemRenderer.sharedMaterial;
			if (sharedMaterial && sharedMaterial.HasProperty("_MainTex"))
			{
				this.particleTexture = sharedMaterial.mainTexture;
			}
			
			ParticleSystem.MainModule main = this._particleSystem.main;
			main.scalingMode = ParticleSystemScalingMode.Local;
			
			this._particles = null;
			flag = true;
		}
		else if (Application.isPlaying)
		{
			flag = (particleSystemRenderer.material == null);
		}
		if (flag)
		{
			this.newMaterial = new Material(Shader.Find("UI/Particles/Hidden"));
			if (Application.isPlaying)
			{
				particleSystemRenderer.material = this.newMaterial;
			}
		}
		if (this._particles == null)
		{
			this._particles = new ParticleSystem.Particle[this._particleSystem.main.maxParticles];
		}
		if (this.particleTexture)
		{
			this._uv = new Vector4(0f, 0f, 1f, 1f);
		}
		else if (this.particleSprite)
		{
			this._uv = DataUtility.GetOuterUV(this.particleSprite);
		}
		this._textureSheetAnimation = this._particleSystem.textureSheetAnimation;
		this._textureSheetAnimationFrames = 0;
		this._textureSheedAnimationFrameSize = Vector2.zero;
		if (this._textureSheetAnimation.enabled)
		{
			this._textureSheetAnimationFrames = this._textureSheetAnimation.numTilesX * this._textureSheetAnimation.numTilesY;
			this._textureSheedAnimationFrameSize = new Vector2(1f / (float)this._textureSheetAnimation.numTilesX, 1f / (float)this._textureSheetAnimation.numTilesY);
		}
		return true;
	}

	
	protected override void Awake()
	{
		base.Awake();
		if (!this.Initialize())
		{
			base.enabled = false;
		}
	}

	
	protected override void OnDestroy()
	{
		base.OnDestroy();
		UnityEngine.Object.DestroyImmediate(this.newMaterial);
	}

	
	protected override void OnPopulateMesh(VertexHelper vh)
	{
		vh.Clear();
		if (!base.gameObject.activeInHierarchy)
		{
			return;
		}
		int particles = this._particleSystem.GetParticles(this._particles);
		for (int i = 0; i < particles; i++)
		{
			ParticleSystem.Particle particle = this._particles[i];
			Vector2 vector = (this._particleSystem.main.simulationSpace == ParticleSystemSimulationSpace.Local) ? particle.position : this._transform.InverseTransformPoint(particle.position);
			float num = -particle.rotation * 0.017453292f;
			float f = num + 1.5707964f;
			Color32 currentColor = particle.GetCurrentColor(this._particleSystem);
			float num2 = particle.GetCurrentSize(this._particleSystem) * 0.5f;
			if (this._particleSystem.main.scalingMode == ParticleSystemScalingMode.Shape)
			{
				vector /= base.canvas.scaleFactor;
			}
			Vector4 uv = this._uv;
			if (this._textureSheetAnimation.enabled)
			{
				float num3 = 1f - particle.remainingLifetime / particle.startLifetime;
				num3 = Mathf.Repeat(num3 * (float)this._textureSheetAnimation.cycleCount, 1f);
				int num4 = 0;
				ParticleSystemAnimationType animType = this._textureSheetAnimation.animation;
				if (animType != ParticleSystemAnimationType.WholeSheet)
				{
					if (animType == ParticleSystemAnimationType.SingleRow)
					{
						num4 = Mathf.FloorToInt(num3 * (float)this._textureSheetAnimation.numTilesX);
						int rowIndex = this._textureSheetAnimation.rowIndex;
						num4 += rowIndex * this._textureSheetAnimation.numTilesX;
					}
				}
				else
				{
					num4 = Mathf.FloorToInt(num3 * (float)this._textureSheetAnimationFrames);
				}
				num4 %= this._textureSheetAnimationFrames;
				uv.x = (float)(num4 % this._textureSheetAnimation.numTilesX) * this._textureSheedAnimationFrameSize.x;
				uv.y = (float)Mathf.FloorToInt((float)(num4 / this._textureSheetAnimation.numTilesX)) * this._textureSheedAnimationFrameSize.y;
				uv.z = uv.x + this._textureSheedAnimationFrameSize.x;
				uv.w = uv.y + this._textureSheedAnimationFrameSize.y;
			}
			this._quad[0] = UIVertex.simpleVert;
			this._quad[0].color = currentColor;
			this._quad[0].uv0 = new Vector2(uv.x, uv.y);
			this._quad[1] = UIVertex.simpleVert;
			this._quad[1].color = currentColor;
			this._quad[1].uv0 = new Vector2(uv.x, uv.w);
			this._quad[2] = UIVertex.simpleVert;
			this._quad[2].color = currentColor;
			this._quad[2].uv0 = new Vector2(uv.z, uv.w);
			this._quad[3] = UIVertex.simpleVert;
			this._quad[3].color = currentColor;
			this._quad[3].uv0 = new Vector2(uv.z, uv.y);
			if (num == 0f)
			{
				Vector2 vector2 = new Vector2(vector.x - num2, vector.y - num2);
				Vector2 vector3 = new Vector2(vector.x + num2, vector.y + num2);
				this._quad[0].position = new Vector2(vector2.x, vector2.y);
				this._quad[1].position = new Vector2(vector2.x, vector3.y);
				this._quad[2].position = new Vector2(vector3.x, vector3.y);
				this._quad[3].position = new Vector2(vector3.x, vector2.y);
			}
			else
			{
				Vector2 b = new Vector2(Mathf.Cos(num), Mathf.Sin(num)) * num2;
				Vector2 b2 = new Vector2(Mathf.Cos(f), Mathf.Sin(f)) * num2;
				this._quad[0].position = vector - b - b2;
				this._quad[1].position = vector - b + b2;
				this._quad[2].position = vector + b + b2;
				this._quad[3].position = vector + b - b2;
			}
			vh.AddUIVertexQuad(this._quad);
		}
	}

	
	private void Update()
	{
		if (Application.isPlaying)
		{
			this._particleSystem.Simulate(Time.unscaledDeltaTime, false, false);
			this.SetAllDirty();
		}
	}

	
	public Texture particleTexture;

	
	public Sprite particleSprite;

	
	private Transform _transform;

	
	private ParticleSystem _particleSystem;

	
	private ParticleSystem.Particle[] _particles;

	
	private UIVertex[] _quad = new UIVertex[4];

	
	private Vector4 _uv = Vector4.zero;

	
	private ParticleSystem.TextureSheetAnimationModule _textureSheetAnimation;

	
	private int _textureSheetAnimationFrames;

	
	private Vector2 _textureSheedAnimationFrameSize;

	
	private Material newMaterial;
}
