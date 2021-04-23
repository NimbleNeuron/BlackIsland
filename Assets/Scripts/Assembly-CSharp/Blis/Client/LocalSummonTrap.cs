using System;
using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Client
{
	[ObjectAttr(ObjectType.SummonTrap)]
	public class LocalSummonTrap : LocalSummonBase
	{
		private float defaultRange;


		protected Material[] defaultSharedRangeMaterial;


		private BulletLine ropeTrap;


		protected Material[] summonSharedRangeMaterial;


		protected override void OnDestroy()
		{
			base.OnDestroy();
			if (ropeTrap != null)
			{
				Destroy(ropeTrap.gameObject);
			}

			if (splatManager != null)
			{
				splatManager.CancelIndicator();
			}
		}

		protected override ObjectType GetObjectType()
		{
			return ObjectType.SummonTrap;
		}


		public override void Init(byte[] snapshotData)
		{
			base.Init(snapshotData);
			CreateIndicator();
		}


		protected override void CreateIndicator()
		{
			base.CreateIndicator();
			if (SummonData.attackPower <= 0f)
			{
				return;
			}

			GameUtil.BindOrAdd<SplatManager>(gameObject, ref splatManager);
			splatManager.CreateIndicator("TrapIndicator");
			Splat indicator = splatManager.GetIndicator("TrapIndicator");
			if (indicator != null)
			{
				indicatorRenderers = new List<Renderer>();
				indicator.GetComponentsInChildren<Renderer>(indicatorRenderers);
				indicator.Range = defaultRange = Math.Abs(SummonData.attackRange) < 0.01f
					? 0f
					: SummonData.attackRange + Stat.Radius;
				indicator.gameObject.SetActive(false);
				if (splatManager.CurrentIndicator == null)
				{
					splatManager.SetIndicator(indicator);
				}

				defaultSharedRangeMaterial = new[]
				{
					SingletonMonoBehaviour<ResourceManager>.inst.LoadIndicatorMaterial("RangeOutline2"),
					SingletonMonoBehaviour<ResourceManager>.inst.LoadIndicatorMaterial("RangeGlow2")
				};
				summonSharedRangeMaterial = new[]
				{
					SingletonMonoBehaviour<ResourceManager>.inst.LoadIndicatorMaterial("RangeOutlineSummonRange"),
					SingletonMonoBehaviour<ResourceManager>.inst.LoadIndicatorMaterial("RangeGlowSummonRange")
				};
			}
		}


		public void OnActiveTrap()
		{
			if (characterObject != null)
			{
				characterObject.SetActive(true);
			}

			if (characterRenderer != null)
			{
				characterRenderer.EnableRenderer(true);
			}

			if (clientCharacter != null)
			{
				clientCharacter.SetStealth(false);
				clientCharacter.SetInSight(true);
				clientCharacter.SetVisible(true);
			}

			SetCharacterAnimatorEnable(true);
			SetCharacterAnimatorTrigger("attack", false);
			if (splatManager != null)
			{
				splatManager.CancelIndicator();
			}
		}


		public override void OnVisible()
		{
			base.OnVisible();
			SetCharacterAnimatorEnable(true);
		}


		public override void OnInvisible()
		{
			base.OnInvisible();
			SetCharacterAnimatorEnable(false);
		}


		public void OnBurst()
		{
			floatingUi.Hide();
			if (ropeTrap != null)
			{
				Destroy(ropeTrap.gameObject);
			}
		}


		public override void OnSight()
		{
			base.OnSight();
			if (indicatorRenderers != null)
			{
				foreach (Renderer renderer in indicatorRenderers)
				{
					renderer.enabled = true;
				}
			}
		}


		public override void OnHide()
		{
			base.OnHide();
			if (indicatorRenderers != null)
			{
				foreach (Renderer renderer in indicatorRenderers)
				{
					renderer.enabled = false;
				}
			}
		}


		protected override void ShowIndicator()
		{
			if (SummonData.attackPower <= 0f)
			{
				return;
			}

			if (splatManager == null)
			{
				return;
			}

			if (splatManager.CurrentIndicator == null)
			{
				return;
			}

			splatManager.CurrentIndicator.gameObject.SetActive(true);
		}


		protected override void HideIndicator()
		{
			if (SummonData.attackPower <= 0f)
			{
				return;
			}

			if (splatManager == null)
			{
				return;
			}

			if (splatManager.CurrentIndicator == null)
			{
				return;
			}

			splatManager.CurrentIndicator.gameObject.SetActive(false);
		}


		public override void OnDead(LocalCharacter attacker)
		{
			base.OnDead(attacker);
			if (ropeTrap != null)
			{
				Destroy(ropeTrap.gameObject);
			}

			if (splatManager != null)
			{
				splatManager.CancelIndicator();
			}
		}


		public void InstallRopeTrap(LocalObject target, SkillId skillId)
		{
			if (target == null)
			{
				return;
			}

			if (ropeTrap == null)
			{
				GameObject gameObject =
					Instantiate<GameObject>(SingletonMonoBehaviour<ResourceManager>.inst.GetBulletLinePrefab());
				ropeTrap = gameObject.GetComponent<BulletLine>();
				ropeTrap.transform.parent = this.transform;
			}

			Transform transform = this.transform;
			BulletLineAnchor componentInChildren = this.transform.GetComponentInChildren<BulletLineAnchor>();
			if (componentInChildren != null)
			{
				transform = componentInChildren.transform;
			}

			Transform transform2 = target.transform;
			BulletLineAnchor componentInChildren2 = target.GetComponentInChildren<BulletLineAnchor>();
			if (componentInChildren2 != null)
			{
				transform2 = componentInChildren2.transform;
			}

			ropeTrap.Link(transform, transform2);
			ropeTrap.SetMaterial(Singleton<NadineSkillActive2Data>.inst.LineMaterial);
			ropeTrap.SetWidth(Singleton<NadineSkillActive2Data>.inst.LineStartWidth,
				Singleton<NadineSkillActive2Data>.inst.LineEndWidth);
		}


		public override ObjectOrder GetObjectOrder()
		{
			if (!MonoBehaviourInstance<ClientService>.inst.IsAlly(this))
			{
				return ObjectOrder.SummonObjectEnemy_Trap;
			}

			if (OwnerId.Equals(MonoBehaviourInstance<ClientService>.inst.MyObjectId))
			{
				return ObjectOrder.SummonObjectMy_Trap;
			}

			return ObjectOrder.SummonObjectAlly_Trap;
		}


		public override void SetSummonRangeIndicator(bool isActive, float range)
		{
			base.SetSummonRangeIndicator(isActive, range);
			if (splatManager != null && splatManager.CurrentIndicator != null)
			{
				splatManager.CurrentIndicator.Range = isActive ? range : defaultRange;
				for (int i = 0; i < indicatorRenderers.Count; i++)
				{
					indicatorRenderers[i].sharedMaterial =
						isActive ? summonSharedRangeMaterial[i] : defaultSharedRangeMaterial[i];
				}
			}
		}
	}
}