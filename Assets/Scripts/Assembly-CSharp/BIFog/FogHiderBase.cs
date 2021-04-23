using System.Collections.Generic;
using Ara;
using Blis.Client;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace BIFog
{
	
	[DisallowMultipleComponent]
	public abstract class FogHiderBase : MonoBehaviour
	{
		
		
		public bool Ignore
		{
			get
			{
				return this.ignore;
			}
		}

		
		
		public float Radius
		{
			get
			{
				return this.radius;
			}
		}

		
		
		public bool IsInSight
		{
			get
			{
				return this.isInSight;
			}
		}

		
		protected virtual void Awake()
		{
			this.RebuildRendererList();
			this.OnOutSight();
			SphereCollider component = base.GetComponent<SphereCollider>();
			if (component != null)
			{
				this.SetRadius(component.radius);
			}
		}

		
		public virtual void Init(ObjectType objectType)
		{
			this.radius = 0f;
			this.isInSight = false;
			if (MonoBehaviourInstance<ClientService>.inst.IsPlayer)
			{
				this.ignore = (objectType.IsPlayerObject() || objectType.IsSummonObject() || objectType == ObjectType.Monster);
				return;
			}
			this.ignore = false;
		}

		
		public void SetIgnore(bool ignore)
		{
			this.ignore = ignore;
		}

		
		public void SetRadius(float radius)
		{
			this.radius = radius;
		}

		
		public virtual void SetHideRenderer(bool hideRenderer)
		{
			this.hideRenderer = hideRenderer;
		}

		
		public virtual void RebuildRendererList()
		{
			if (this.hideRenderer)
			{
				base.GetComponentsInChildren<Renderer>(true, this.renderList);
				base.GetComponentsInChildren<AraTrail>(true, this.araTrails);
				base.GetComponentsInChildren<Light>(true, this.lights);
				for (int i = this.renderList.Count - 1; i >= 0; i--)
				{
					if (this.renderList[i].GetComponentInParent<FogHiderBase>() != this)
					{
						this.renderList.RemoveAt(i);
					}
				}
				for (int j = this.araTrails.Count - 1; j >= 0; j--)
				{
					if (this.araTrails[j].GetComponentInParent<FogHiderBase>() != this)
					{
						this.araTrails.RemoveAt(j);
					}
				}
				for (int k = this.lights.Count - 1; k >= 0; k--)
				{
					if (this.lights[k].GetComponentInParent<FogHiderBase>() != this)
					{
						this.lights.RemoveAt(k);
					}
				}
				if (!this.isInSight)
				{
					this.EnableRenderer(false);
				}
			}
		}

		
		public virtual void InSight()
		{
			if (!this.isInSight)
			{
				this.isInSight = true;
				this.RebuildRendererList();
				base.GetComponentsInChildren<ISightEventHandler>(true, this.eventHandlers);
				for (int i = 0; i < this.eventHandlers.Count; i++)
				{
					if (this.eventHandlers[i].transform.GetComponentInParent<FogHiderBase>() == this)
					{
						this.eventHandlers[i].OnSight();
					}
				}
				this.InSightInternal();
			}
		}

		
		protected void InSightInternal()
		{
			this.EnableRenderer(true);
		}

		
		public virtual void OutSight()
		{
			if (this.isInSight)
			{
				this.isInSight = false;
				this.OutSightInternal();
			}
		}

		
		protected void OutSightInternal()
		{
			base.GetComponentsInChildren<ISightEventHandler>(true, this.eventHandlers);
			for (int i = 0; i < this.eventHandlers.Count; i++)
			{
				if (this.eventHandlers[i].transform.GetComponentInParent<FogHiderBase>() == this)
				{
					this.eventHandlers[i].OnHide();
				}
			}
			this.OnOutSight();
		}

		
		protected virtual void OnOutSight()
		{
			this.EnableRenderer(false);
		}

		
		private void EnableRenderer(bool enable)
		{
			if (this.hideRenderer)
			{
				for (int i = 0; i < this.renderList.Count; i++)
				{
					if (this.renderList[i] != null)
					{
						this.renderList[i].enabled = enable;
					}
				}
				for (int j = 0; j < this.araTrails.Count; j++)
				{
					if (this.araTrails[j] != null)
					{
						this.araTrails[j].enabled = enable;
					}
				}
				for (int k = 0; k < this.lights.Count; k++)
				{
					if (this.lights[k] != null)
					{
						this.lights[k].enabled = enable;
					}
				}
			}
		}

		
		private List<Renderer> renderList = new List<Renderer>();

		
		private List<AraTrail> araTrails = new List<AraTrail>();

		
		private List<Light> lights = new List<Light>();

		
		protected List<ISightEventHandler> eventHandlers = new List<ISightEventHandler>();

		
		protected bool isPlayer;

		
		private bool ignore;

		
		private float radius;

		
		protected bool isInSight;

		
		[SerializeField]
		protected bool hideRenderer;
	}
}
