using System;
using System.Collections.Generic;
using Blis.Client;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace BIFog
{
	
	[ExecuteInEditMode]
	public class FogSight : MonoBehaviour
	{
		
		
		public int ObjectId
		{
			get
			{
				if (!(this.sightAgent != null))
				{
					return 0;
				}
				return this.sightAgent.ObjectId;
			}
		}

		
		
		public int SightOwnerId
		{
			get
			{
				if (!(this.sightAgent != null))
				{
					return 0;
				}
				return this.sightAgent.SightOwnerId;
			}
		}

		
		
		public float SightRange
		{
			get
			{
				if (!(this.sightAgent != null))
				{
					return this.defaultSightRange;
				}
				return this.sightAgent.SightRange;
			}
		}

		
		
		private int SightAngle
		{
			get
			{
				if (!(this.sightAgent != null))
				{
					return this.defaultSightAngle;
				}
				return this.sightAgent.SightAngle;
			}
		}

		
		
		public bool UseOtherSight
		{
			get
			{
				return this.sightAgent != null && this.sightAgent.UseOtherSight;
			}
		}

		
		private void OnEnable()
		{
			if (Application.isPlaying)
			{
				MonoBehaviourInstance<FogManager>.inst.RegisterFogSight(this);
			}
			this.mainCamera = Camera.main;
		}

		
		private void OnDisable()
		{
			if (Application.isPlaying && MonoBehaviourInstance<FogManager>.inst != null)
			{
				MonoBehaviourInstance<FogManager>.inst.UnregisterFogSight(this);
			}
		}

		
		public void SetSightAgent(LocalSightAgent sightAgent)
		{
			this.sightAgent = sightAgent;
		}

		
		public void FindFogHiderOnCenter(HashSet<FogHiderOnCenter> fogHiders)
		{
			if (!base.enabled)
			{
				return;
			}
			Vector3 position = base.transform.position;
			int num = Physics.OverlapSphereNonAlloc(position, this.SightRange + 0.5f, this.resultCache, this.targetMask);
			while (this.resultCache.Length <= num)
			{
				this.resultCache = new Collider[this.resultCache.Length + 100];
				num = Physics.OverlapSphereNonAlloc(position, this.SightRange + 0.5f, this.resultCache, this.targetMask);
			}
			for (int i = 0; i < num; i++)
			{
				Collider collider = this.resultCache[i];
				FogHiderOnCenter component = collider.GetComponent<FogHiderOnCenter>();
				if (!fogHiders.Contains(component) && !(component == null) && !component.Ignore)
				{
					if (this.sightAgent != null)
					{
						LocalSightAgent localSightAgent = collider.GetComponent<LocalSightAgent>();
						if (localSightAgent == null)
						{
							localSightAgent = component.OwnerSightAgent;
						}
						if (localSightAgent != null)
						{
							if (this.sightAgent.IsInSight(localSightAgent, component.transform.position, component.Radius, localSightAgent.IsInvisibleCheckWithMemorizer(this.sightAgent.ObjectId)))
							{
								fogHiders.Add(component);
							}
						}
						else if (this.sightAgent.IsInSight(component.transform.position, component.Radius))
						{
							fogHiders.Add(component);
						}
						else if (component.AnotherTargetTransform != null && this.sightAgent.IsInSight(component.AnotherTargetTransform.position, component.Radius))
						{
							fogHiders.Add(component);
						}
					}
					else if (SightAgent.IsInSight(base.transform.position, base.transform.TransformDirection(Vector3.forward), this.SightRange, this.SightAngle, component.transform.position, component.Radius))
					{
						fogHiders.Add(component);
					}
				}
			}
		}

		
		public void FindFogHiderOnCollider(List<FogHiderOnCollider> fogHiders)
		{
			if (!base.enabled)
			{
				return;
			}
			Vector3 position = base.transform.position;
			this.CheckFogHiderOnColliderRayDirection();
			int num = Physics.OverlapSphereNonAlloc(position, 0.2f, this.resultCache, this.targetMask);
			while (this.resultCache.Length <= num)
			{
				this.resultCache = new Collider[this.resultCache.Length + 100];
				num = Physics.OverlapSphereNonAlloc(position, 0.2f, this.resultCache, this.targetMask);
			}
			for (int i = 0; i < num; i++)
			{
				FogHiderOnCollider item;
				if (this.resultCache[i].TryGetComponent<FogHiderOnCollider>(out item))
				{
					fogHiders.Add(item);
				}
			}
			position.y += 1.5f;
			for (int j = 0; j < 36; j++)
			{
				num = Physics.RaycastNonAlloc(position, FogSight.fogHiderOnColliderRayDirection[j], this.resultHits, this.SightRange, GameConstants.LayerMask.SIGHT_COLLIDER_LAYER);
				if (num != 0)
				{
					while (this.resultHits.Length <= num)
					{
						this.resultHits = new RaycastHit[this.resultHits.Length + 100];
						num = Physics.RaycastNonAlloc(position, FogSight.fogHiderOnColliderRayDirection[j], this.resultHits, this.SightRange, GameConstants.LayerMask.SIGHT_COLLIDER_LAYER);
					}
					Array.Sort<RaycastHit>(this.resultHits, 0, num, GameUtil.raycastHitDistanceComparer);
					for (int k = 0; k < num; k++)
					{
						Transform transform = this.resultHits[k].transform;
						if (GameConstants.LayerNumber.IsObastacleLayer(transform.gameObject.layer))
						{
							break;
						}
						FogHiderOnCollider item2;
						if (transform.TryGetComponent<FogHiderOnCollider>(out item2))
						{
							fogHiders.Add(item2);
						}
					}
				}
			}
		}

		
		private void CheckFogHiderOnColliderRayDirection()
		{
			if (FogSight.fogHiderOnColliderRayDirection != null)
			{
				return;
			}
			FogSight.fogHiderOnColliderRayDirection = new Dictionary<int, Vector3>();
			float num = 0f;
			float num2 = 10f;
			for (int i = 0; i < 36; i++)
			{
				float f = num * 0.017453292f;
				num += num2;
				FogSight.fogHiderOnColliderRayDirection.Add(i, new Vector3(Mathf.Sin(f), 0f, Mathf.Cos(f)));
			}
		}

		
		public bool IsAlly(int sightOwnerId)
		{
			return this.sightAgent != null && this.sightAgent.IsAlly(sightOwnerId);
		}

		
		public bool IsAttachSight(int sightOwnerId)
		{
			return this.sightAgent != null && this.sightAgent.IsAttachSight(sightOwnerId);
		}

		
		[ContextMenu("LoadDefaultSetting")]
		public void LoadDefaultSetting(float defaultRange)
		{
			this.eyeOffset = 1.5f;
			this.defaultSightRange = defaultRange;
			this.defaultSightAngle = 360;
			this.obstacleMask = LayerMask.GetMask(this.DefaultLayers);
			this.targetMask = LayerMask.GetMask(this.TargetLayers);
			this.meshResolution = 0.2f;
		}

		
		public void SetStaticSight(bool isStaticSight)
		{
			this.isStaticSight = isStaticSight;
		}

		
		public void UpdateSightQuality(SightQuality sightQuality)
		{
			switch (sightQuality)
			{
			case SightQuality.Auto:
				if (this.SightRange > 10f)
				{
					this.meshResolution = 0.2f;
					return;
				}
				if (this.SightRange > 5f)
				{
					this.meshResolution = 0.1f;
					return;
				}
				this.meshResolution = 0.02f;
				return;
			case SightQuality.Low:
				this.meshResolution = 0.02f;
				return;
			case SightQuality.Normal:
				this.meshResolution = 0.1f;
				return;
			case SightQuality.High:
				this.meshResolution = 0.2f;
				return;
			default:
				return;
			}
		}

		
		public void Rebuild()
		{
			this.dirtyFlag = true;
		}

		
		public Mesh MakeFogMesh()
		{
			if (!this.isStaticSight || this.dirtyFlag)
			{
				float num = this.meshResolution;
				if (!this.isStaticSight)
				{
					Ray ray = this.mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
					Vector3 vector = ray.origin;
					Vector3 position = base.transform.position;
					position.y = 0f;
					if (ray.direction.y == 0f)
					{
						vector.y = 0f;
					}
					else
					{
						vector -= ray.direction * (vector.y / ray.direction.y);
					}
					float b = 0.02f;
					float value = Vector3.Distance(vector, position);
					float t = Mathf.Clamp01(Mathf.InverseLerp(25f, 50f, value));
					num = Mathf.Lerp(this.meshResolution, b, t);
				}
				this.fogMesh = Singleton<FogMeshHelper>.inst.BuildFogMesh(base.transform, this.SightRange, (float)this.SightAngle, this.eyeOffset, num, this.obstacleMask, this.fogMesh);
			}
			return this.fogMesh;
		}

		
		private readonly string[] DefaultLayers = new string[]
		{
			"FogOfWar",
			"Bush"
		};

		
		private readonly string[] TargetLayers = new string[]
		{
			"WorldObject",
			"Fade",
			"Fx"
		};

		
		public bool isStaticSight;

		
		public float eyeOffset;

		
		private float defaultSightRange = 10f;

		
		private int defaultSightAngle = 360;

		
		public LayerMask obstacleMask;

		
		public LayerMask targetMask;

		
		public float meshResolution;

		
		private Mesh fogMesh;

		
		private bool dirtyFlag;

		
		private const int CacheSize = 100;

		
		private Collider[] resultCache = new Collider[100];

		
		private const int FogHiderOnColliderRayCount = 36;

		
		private RaycastHit[] resultHits = new RaycastHit[100];

		
		private static Dictionary<int, Vector3> fogHiderOnColliderRayDirection;

		
		private LocalSightAgent sightAgent;

		
		private Camera mainCamera;
	}
}
