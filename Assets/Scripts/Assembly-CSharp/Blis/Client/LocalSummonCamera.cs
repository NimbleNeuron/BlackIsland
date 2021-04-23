using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Client
{
	[ObjectAttr(ObjectType.SummonCamera)]
	public class LocalSummonCamera : LocalSummonBase
	{
		protected override ObjectType GetObjectType()
		{
			return ObjectType.SummonCamera;
		}


		public override void Init(byte[] snapshotData)
		{
			base.Init(snapshotData);
			CreateIndicator();
		}


		protected override void CreateIndicator()
		{
			base.CreateIndicator();
			GameUtil.BindOrAdd<SplatManager>(gameObject, ref splatManager);
			splatManager.CreateIndicator("TrapIndicator");
			Splat indicator = splatManager.GetIndicator("TrapIndicator");
			if (indicator != null)
			{
				indicatorRenderers = new List<Renderer>();
				indicator.GetComponentsInChildren<Renderer>(indicatorRenderers);
				indicator.Range = 0f;
				indicator.gameObject.SetActive(false);
				if (splatManager.CurrentIndicator == null)
				{
					splatManager.SetIndicator(indicator);
				}

				Material[] array =
				{
					SingletonMonoBehaviour<ResourceManager>.inst.LoadIndicatorMaterial("RangeOutlineSummonRange"),
					SingletonMonoBehaviour<ResourceManager>.inst.LoadIndicatorMaterial("RangeGlowSummonRange")
				};
				for (int i = 0; i < indicatorRenderers.Count; i++)
				{
					indicatorRenderers[i].sharedMaterial = array[i];
				}
			}
		}


		public override ObjectOrder GetObjectOrder()
		{
			if (!MonoBehaviourInstance<ClientService>.inst.IsAlly(this))
			{
				return ObjectOrder.SummonObjectEnemy_Camera;
			}

			if (OwnerId.Equals(MonoBehaviourInstance<ClientService>.inst.MyObjectId))
			{
				return ObjectOrder.SummonObjectMy_Camera;
			}

			return ObjectOrder.SummonObjectAlly_Camera;
		}


		public override void SetSummonRangeIndicator(bool isActive, float range)
		{
			base.SetSummonRangeIndicator(isActive, range);
			if (splatManager != null && splatManager.CurrentIndicator != null)
			{
				splatManager.CurrentIndicator.gameObject.SetActive(isActive);
				if (isActive && splatManager.CurrentIndicator.Range != range)
				{
					splatManager.CurrentIndicator.Range = range;
				}
			}
		}
	}
}