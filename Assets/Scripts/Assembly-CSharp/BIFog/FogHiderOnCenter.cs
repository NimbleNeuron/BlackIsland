using Blis.Client;
using Blis.Common;
using UnityEngine;

namespace BIFog
{
	
	[DisallowMultipleComponent]
	public class FogHiderOnCenter : FogHiderBase
	{
		
		
		public LocalSightAgent OwnerSightAgent
		{
			get
			{
				return this.ownerSightAgent;
			}
		}

		
		
		public Transform AnotherTargetTransform
		{
			get
			{
				return this.anotherTargetTransform;
			}
		}

		
		protected override void Awake()
		{
			if (this.hideRenderer && base.GetComponentInParent<CharacterRenderer>() != null)
			{
				Collider component = base.transform.GetComponent<Collider>();
				if (component != null)
				{
					UnityEngine.Object.Destroy(component);
				}
				UnityEngine.Object.Destroy(this);
				return;
			}
			base.Awake();
		}

		
		public override void Init(ObjectType objectType)
		{
			base.Init(objectType);
			this.isPlayer = objectType.IsPlayerObject();
			this.outSightDelay = -1f;
			this.ownerSightAgent = null;
		}

		
		public void SetSightAgent(LocalSightAgent sightAgent)
		{
			this.ownerSightAgent = sightAgent;
		}

		
		private void LateUpdate()
		{
			if (!this.isInSight && 0f <= this.outSightDelay)
			{
				this.outSightDelay -= Time.deltaTime;
				if (this.outSightDelay <= 0f)
				{
					base.OutSightInternal();
				}
			}
			this.SetPositionAnotherTargetCollider();
		}

		
		public override void InSight()
		{
			if (!this.isInSight)
			{
				this.isInSight = true;
				this.outSightDelay = -1f;
				this.RebuildRendererList();
				base.GetComponentsInChildren<ISightEventHandler>(this.eventHandlers);
				for (int i = 0; i < this.eventHandlers.Count; i++)
				{
					if (this.eventHandlers[i].transform.GetComponentInParent<FogHiderBase>() == this)
					{
						this.eventHandlers[i].OnSight();
					}
				}
				base.InSightInternal();
			}
		}

		
		public override void OutSight()
		{
			if (this.isInSight)
			{
				this.isInSight = false;
				this.outSightDelay = (this.isPlayer ? -1f : 0.2f);
				if (this.isPlayer)
				{
					base.OutSightInternal();
				}
			}
		}

		
		public void SetAnotherTargetTransform(Transform anotherTargetTransform, float colliderRadius)
		{
			if (this.anotherTargetCollider == null)
			{
				this.anotherTargetCollider = base.gameObject.AddComponent<SphereCollider>();
			}
			if (anotherTargetTransform == null)
			{
				this.anotherTargetTransform = anotherTargetTransform;
				this.anotherTargetCollider.enabled = false;
			}
			else
			{
				this.anotherTargetCollider.radius = colliderRadius;
				this.anotherTargetTransform = anotherTargetTransform;
				this.anotherTargetCollider.enabled = true;
			}
			this.SetPositionAnotherTargetCollider();
		}

		
		private void SetPositionAnotherTargetCollider()
		{
			if (this.anotherTargetTransform == null)
			{
				return;
			}
			Vector3 vector = this.anotherTargetTransform.position - base.transform.position;
			float num = base.transform.localRotation.eulerAngles.y;
			if (num != 0f)
			{
				num *= 0.017453292f;
				float num2 = Mathf.Sin(num);
				float num3 = Mathf.Cos(num);
				float x = vector.x;
				float z = vector.z;
				vector.x = num3 * x - num2 * z;
				vector.z = num2 * x + num3 * z;
			}
			this.anotherTargetCollider.center = vector;
		}

		
		private float outSightDelay = -1f;

		
		private const float OutSightDelayTime = 0.2f;

		
		private LocalSightAgent ownerSightAgent;

		
		private Transform anotherTargetTransform;

		
		private SphereCollider anotherTargetCollider;
	}
}
