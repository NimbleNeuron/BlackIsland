using System.Collections.Generic;
using System.Linq;
using Blis.Client;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.Rendering;

namespace BIFog
{
	
	public class FogManager : MonoBehaviourInstance<FogManager>
	{
		
		protected override void _Awake()
		{
			this._commandBuffer = new CommandBuffer();
			this._fogMaterial = new Material(Shader.Find("Hidden/BIFogMeta"));
			this._fogReducer = new Material(Shader.Find("Hidden/BIFogReducer"));
			this.fogPostProcessor = base.GetComponent<FogPostProcessor>();
		}

		
		protected void Start()
		{
			if (this.Camera == null)
			{
				this.Camera = this.CreateFogCamera();
			}
			this.OnResolution();
		}

		
		protected override void _OnDestroy()
		{
			base._OnDestroy();
			UnityEngine.Object.Destroy(this._fogMaterial);
			UnityEngine.Object.Destroy(this._fogReducer);
		}

		
		private Camera CreateFogCamera()
		{
			Camera camera = new GameObject("Fog Camera")
			{
				transform = 
				{
					parent = base.transform,
					localPosition = new Vector3(0f, 25f, 0f),
					rotation = Quaternion.Euler(90f, 0f, 0f)
				}
			}.AddComponent<Camera>();
			camera.farClipPlane = 50f;
			camera.orthographicSize = this._worldHalfSize;
			camera.enabled = false;
			camera.renderingPath = RenderingPath.VertexLit;
			camera.backgroundColor = Color.clear;
			camera.clearFlags = CameraClearFlags.Nothing;
			camera.orthographic = true;
			camera.rect = new Rect(0f, 0f, 1f, 1f);
			camera.cullingMask = 0;
			camera.enabled = false;
			camera.allowHDR = false;
			return camera;
		}

		
		public void SetMyFogSight(FogSight fogSight)
		{
			this.myFogSight = fogSight;
		}

		
		private void LateUpdate()
		{
			if (this.myFogSight == null)
			{
				return;
			}
			this.tick += Time.deltaTime;
			if (this.tick >= 0.033333335f)
			{
				this.RenderFogTexture();
				this.tick = 0f;
			}
			this.UpdateFogHiderOnCenter();
		}

		
		private void UpdateFogHiderOnCenter()
		{
			this.tempFogHiderCenters.Clear();
			this.curFogHiderCenterBuffer.Clear();
			for (int i = 0; i < this._fogSights.Count; i++)
			{
				FogSight fogSight = this._fogSights[i];
				if (this.IsUsable(this.myFogSight, fogSight))
				{
					fogSight.FindFogHiderOnCenter(this.tempFogHiderCenters);
				}
			}
			if (this.tempFogHiderCenters.Any<FogHiderOnCenter>())
			{
				this.curFogHiderCenterBuffer.UnionWith(this.tempFogHiderCenters);
			}
			foreach (FogHiderOnCenter fogHiderOnCenter in this.curFogHiderCenterBuffer)
			{
				fogHiderOnCenter.InSight();
				this.preFogHiderCenterBuffer.Remove(fogHiderOnCenter);
			}
			foreach (FogHiderOnCenter fogHiderOnCenter2 in this.preFogHiderCenterBuffer)
			{
				if (fogHiderOnCenter2 != null)
				{
					fogHiderOnCenter2.OutSight();
				}
			}
			this.preFogHiderCenterBuffer.Clear();
			HashSet<FogHiderOnCenter> hashSet = this.curFogHiderCenterBuffer;
			this.curFogHiderCenterBuffer = this.preFogHiderCenterBuffer;
			this.preFogHiderCenterBuffer = hashSet;
		}

		
		private void UpdateFogHiderOnCollider()
		{
			this.tempFogHiderColliders.Clear();
			this.curFogHiderColliderBuffer.Clear();
			for (int i = 0; i < this._fogSights.Count; i++)
			{
				FogSight fogSight = this._fogSights[i];
				if (this.IsUsable(this.myFogSight, fogSight) && (!(MonoBehaviourInstance<MobaCamera>.inst != null) || GameUtil.DistanceOnPlane(MonoBehaviourInstance<MobaCamera>.inst.transform.position, fogSight.transform.position) <= 50f))
				{
					fogSight.FindFogHiderOnCollider(this.tempFogHiderColliders);
				}
			}
			if (this.tempFogHiderColliders.Any<FogHiderOnCollider>())
			{
				this.curFogHiderColliderBuffer.UnionWith(this.tempFogHiderColliders);
			}
			foreach (FogHiderOnCollider fogHiderOnCollider in this.curFogHiderColliderBuffer)
			{
				fogHiderOnCollider.InSight();
				this.preFogHiderColliderBuffer.Remove(fogHiderOnCollider);
			}
			foreach (FogHiderOnCollider fogHiderOnCollider2 in this.preFogHiderColliderBuffer)
			{
				if (fogHiderOnCollider2 != null)
				{
					fogHiderOnCollider2.OutSight();
				}
			}
			this.preFogHiderColliderBuffer.Clear();
			HashSet<FogHiderOnCollider> hashSet = this.curFogHiderColliderBuffer;
			this.curFogHiderColliderBuffer = this.preFogHiderColliderBuffer;
			this.preFogHiderColliderBuffer = hashSet;
		}

		
		public void RegisterFogSight(FogSight fogSight)
		{
			if (this._fogSights != null && !this._fogSights.Contains(fogSight))
			{
				this._fogSights.Add(fogSight);
			}
		}

		
		public void UnregisterFogSight(FogSight fogSight)
		{
			this._fogSights.Remove(fogSight);
		}

		
		public void RegisterFogHider(FogHiderOnCenter fogHiderOnCenter)
		{
			if (fogHiderOnCenter != null && !this.fogHiders.Contains(fogHiderOnCenter))
			{
				this.fogHiders.Add(fogHiderOnCenter);
			}
		}

		
		public void UnregisterFogHider(FogHiderOnCenter fogHiderOnCenter)
		{
			List<FogHiderOnCenter> list = this.fogHiders;
			if (list == null)
			{
				return;
			}
			list.Remove(fogHiderOnCenter);
		}

		
		public void SetResolution(Vector2 resolution)
		{
			this._fogResolution.x = (float)Mathf.Max(1, Mathf.RoundToInt(resolution.x));
			this._fogResolution.y = (float)Mathf.Max(1, Mathf.RoundToInt(resolution.y));
			this.OnResolution();
		}

		
		private void OnResolution()
		{
			this.preFogCache = new Texture2D((int)this._fogResolution.x, (int)this._fogResolution.y, TextureFormat.R8, false);
			if (this._fogRenderTexture != null)
			{
				this._fogRenderTexture.Release();
			}
			this._fogRenderTexture = new RenderTexture((int)this._fogResolution.x, (int)this._fogResolution.y, 0, RenderTextureFormat.R8);
			Graphics.SetRenderTarget(this._fogRenderTexture);
			GL.Clear(true, true, Color.black);
			Graphics.CopyTexture(this._fogRenderTexture, this.preFogCache);
			this.Camera.targetTexture = this._fogRenderTexture;
			float num = this._fogResolution.x / this._fogResolution.y;
			Shader.SetGlobalVector("BIFogWorldScale", new Vector2(this._worldHalfSize * num * 2f, this._worldHalfSize * 2f));
			Shader.SetGlobalTexture("BIFogTexture", this._fogRenderTexture);
		}

		
		private void RenderFogTexture()
		{
			return;
			
			if (this._commandBuffer == null)
			{
				return;
			}
			this._commandBuffer.Clear();
			Graphics.SetRenderTarget(this._fogRenderTexture);
			this._fogReducer.SetFloat("_Reduce", this._fogReducerLevel * Time.deltaTime);
			Graphics.Blit(this.preFogCache, this._fogRenderTexture, this._fogReducer);
			GL.LoadProjectionMatrix(this.Camera.projectionMatrix * this.Camera.worldToCameraMatrix);
			for (int i = 0; i < this._fogSights.Count; i++)
			{
				FogSight fogSight = this._fogSights[i];
				if (fogSight.SightRange > Mathf.Epsilon && this.IsUsable(this.myFogSight, fogSight))
				{
					this._commandBuffer.DrawMesh(fogSight.MakeFogMesh(), fogSight.transform.localToWorldMatrix, this._fogMaterial);
				}
			}
			Graphics.ExecuteCommandBuffer(this._commandBuffer);
			Graphics.CopyTexture(this._fogRenderTexture, this.preFogCache);
			if (this.fogPostProcessor != null)
			{
				this.fogPostProcessor.Process(this._fogRenderTexture, this._fogRenderTexture);
			}
		}

		
		private void OnDrawGizmos()
		{
			Gizmos.color = Color.black;
			float num = this._fogResolution.x / this._fogResolution.y;
			Gizmos.DrawWireCube(base.transform.position, new Vector3(this._worldHalfSize * num * 2f, 50f, this._worldHalfSize * 2f));
		}

		
		private bool IsUsable(FogSight myFogSight, FogSight otherFogSight)
		{
			return !(myFogSight == null) && !(otherFogSight == null) && (myFogSight.ObjectId.Equals(otherFogSight.ObjectId) || (myFogSight.UseOtherSight && (myFogSight.ObjectId.Equals(otherFogSight.SightOwnerId) || myFogSight.IsAlly(otherFogSight.SightOwnerId) || myFogSight.IsAttachSight(otherFogSight.SightOwnerId))));
		}

		
		private const string BI_FOG_TEXTURE = "BIFogTexture";

		
		private const string BI_FOG_WORLD_SCALE = "BIFogWorldScale";

		
		private const string FOG_REDUCE_PROPERTY = "_Reduce";

		
		private const int CAMEARA_FAR = 50;

		
		private Camera Camera;

		
		[SerializeField]
		private Vector2 _fogResolution = new Vector2(1024f, 1024f);

		
		public float _worldHalfSize = 100f;

		
		private CommandBuffer _commandBuffer;

		
		private RenderTexture _fogRenderTexture;

		
		[SerializeField]
		private float _fogReducerLevel = 1f;

		
		private Material _fogMaterial;

		
		private Material _fogReducer;

		
		private Texture2D preFogCache;

		
		private FogPostProcessor fogPostProcessor;

		
		private readonly List<FogSight> _fogSights = new List<FogSight>();

		
		private readonly List<FogHiderOnCenter> fogHiders = new List<FogHiderOnCenter>();

		
		private FogSight myFogSight;

		
		private float tick;

		
		private HashSet<FogHiderOnCenter> curFogHiderCenterBuffer = new HashSet<FogHiderOnCenter>();

		
		private HashSet<FogHiderOnCenter> preFogHiderCenterBuffer = new HashSet<FogHiderOnCenter>();

		
		private readonly HashSet<FogHiderOnCenter> tempFogHiderCenters = new HashSet<FogHiderOnCenter>();

		
		private HashSet<FogHiderOnCollider> curFogHiderColliderBuffer = new HashSet<FogHiderOnCollider>();

		
		private HashSet<FogHiderOnCollider> preFogHiderColliderBuffer = new HashSet<FogHiderOnCollider>();

		
		private readonly List<FogHiderOnCollider> tempFogHiderColliders = new List<FogHiderOnCollider>();

		
		private const float FogHiderOnColliderCheckInRange = 50f;
	}
}
