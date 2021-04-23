using BIFog;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Client
{
	public class LocalSightAgent : SightAgent
	{
		private FogSight fogSight;


		private bool isOutSight;


		private SightQuality sightQuality;


		private LocalObject target;


		public FogSight FogSight => fogSight;


		public bool IsOutSight => isOutSight;


		protected override void OnDisable()
		{
			base.OnDisable();
			DisableFogSight();
			if (isAttachMode)
			{
				target.RemoveAttachedSight(this);
			}
		}


		public override void InitCharacterSight(ObjectBase target)
		{
			base.InitCharacterSight(target);
			this.target = target as LocalObject;
		}


		public override void InitAttachSight(ObjectBase target, int attachSightId)
		{
			base.InitAttachSight(target, attachSightId);
			this.target = target as LocalObject;
			this.target.AddAttachedSight(this);
		}


		public void SetSightQuality(SightQuality sightQuality)
		{
			this.sightQuality = sightQuality;
			if (fogSight != null)
			{
				fogSight.UpdateSightQuality(sightQuality);
			}
		}


		public void EnableFogSight()
		{
			if (fogSight == null)
			{
				fogSight = gameObject.AddComponent<FogSight>();
				fogSight.SetSightAgent(this);
				fogSight.LoadDefaultSetting(10f);
				fogSight.UpdateSightQuality(sightQuality);
			}

			if (isAttachMode)
			{
				return;
			}

			for (int i = 0; i < allySights.Count; i++)
			{
				((LocalSightAgent) allySights[i]).EnableFogSight();
			}

			for (int j = 0; j < attachSights.Count; j++)
			{
				((LocalSightAgent) attachSights[j]).EnableFogSight();
			}
		}


		public void DisableFogSight()
		{
			if (fogSight)
			{
				Destroy(fogSight);
				fogSight = null;
			}

			if (isAttachMode)
			{
				return;
			}

			for (int i = 0; i < allySights.Count; i++)
			{
				((LocalSightAgent) allySights[i]).DisableFogSight();
			}

			for (int j = 0; j < attachSights.Count; j++)
			{
				((LocalSightAgent) attachSights[j]).DisableFogSight();
			}
		}


		public override bool IsInAllySight(SightAgent targetSightAgent, Vector3 targetPos, float radius,
			bool isInvisible)
		{
			return !isOutSight && base.IsInAllySight(targetSightAgent, targetPos, radius, isInvisible);
		}


		public override bool IsInSight(Vector3 targetPos, float targetRadius)
		{
			return !isOutSight && base.IsInSight(targetPos, targetRadius);
		}


		public override bool IsInSight(SightAgent targetSightAgent, Vector3 targetPos, float targetRadius,
			bool isInvisible)
		{
			return !isOutSight && base.IsInSight(targetSightAgent, targetPos, targetRadius, isInvisible);
		}


		public override void AddAllySight(SightAgent agent)
		{
			base.AddAllySight(agent);
			OnAddSight(agent);
		}


		public override void RemoveAllySight(SightAgent agent)
		{
			base.RemoveAllySight(agent);
			OnRemoveSight(agent);
		}


		public override void AddAttachSight(SightAgent agent)
		{
			base.AddAttachSight(agent);
			OnAddSight(agent);
		}


		public override void RemoveAttachSight(SightAgent agent)
		{
			base.RemoveAttachSight(agent);
			OnRemoveSight(agent);
		}


		private void OnAddSight(SightAgent agent)
		{
			if (fogSight)
			{
				((LocalSightAgent) agent).EnableFogSight();
			}
		}


		private void OnRemoveSight(SightAgent agent)
		{
			((LocalSightAgent) agent).DisableFogSight();
		}


		public override void BlockAllySight(BlockedSightType blockedSightType, bool block)
		{
			base.BlockAllySight(blockedSightType, block);
			if (MonoBehaviourInstance<ClientService>.inst.MyObjectId == SightOwnerId)
			{
				if (block)
				{
					allySights.ForEach(delegate(SightAgent x) { ((LocalSightAgent) x).DisableFogSight(); });
					attachSights.ForEach(delegate(SightAgent x) { ((LocalSightAgent) x).DisableFogSight(); });
					return;
				}

				allySights.ForEach(delegate(SightAgent x) { ((LocalSightAgent) x).EnableFogSight(); });
				attachSights.ForEach(delegate(SightAgent x) { ((LocalSightAgent) x).EnableFogSight(); });
			}
		}


		public void InSightOnServer()
		{
			isOutSight = false;
		}


		public void OutSightOnServer()
		{
			isOutSight = true;
		}
	}
}