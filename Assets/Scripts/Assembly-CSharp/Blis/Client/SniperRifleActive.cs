using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.SniperRifleActive)]
	public class SniperRifleActive : LocalSkillScript
	{
		protected static readonly int TriggerSkill = Animator.StringToHash("tSniperRifle_Skill");


		protected static readonly int TriggerSkill_2 = Animator.StringToHash("tSniperRifle_Skill_2");


		protected static readonly int TriggerSkill_3 = Animator.StringToHash("tSniperRifle_Skill_3");


		private BulletLine bulletLine;


		private MobaCameraMode preMode;


		public override void Start()
		{
			PlayAnimation(Self, TriggerReloadCancel);
			PlayAnimation(Self, TriggerSkill);
			if (SingletonMonoBehaviour<PlayerController>.inst.IsMe(Self.ObjectId))
			{
				SingletonMonoBehaviour<PlayerController>.inst.SetCursorStatus(CursorStatus.Skill);
			}

			LockSkillSlots(SkillSlotIndex.Active1);
			LockSkillSlots(SkillSlotIndex.Active2);
			LockSkillSlots(SkillSlotIndex.Active3);
			LockSkillSlots(SkillSlotIndex.Active4);
			LockSkillSlot(SkillSlotSet.WeaponSkill);
			StartCoroutine(CoroutineUtil.DelayedAction(data.ConcentrationTime,
				delegate { UnlockSkillSlot(SkillSlotSet.WeaponSkill); }));
		}


		private void CreateBulletLine(Transform src, Transform dest)
		{
			if (bulletLine == null)
			{
				GameObject gameObject =
					Object.Instantiate<GameObject>(SingletonMonoBehaviour<ResourceManager>.inst.GetBulletLinePrefab());
				bulletLine = gameObject.GetComponent<BulletLine>();
			}

			bulletLine.Link(src, dest);
		}


		private void RemoveBulletLine()
		{
			if (bulletLine != null)
			{
				bulletLine.Unlink();
			}
		}


		private void DestroyBulletLine()
		{
			if (bulletLine != null)
			{
				Object.Destroy(bulletLine.gameObject);
			}
		}


		public override void Play(int actionNo, LocalObject target, Vector3? targetPosition)
		{
			switch (actionNo)
			{
				case 1:
					ZoomIn();
					return;
				case 2:
					RotationLocalObject(Self, GameUtil.DirectionOnPlane(Self.GetPosition(), target.GetPosition()));
					CreateBulletLine(Self.transform.FindRecursively("ShotPoint"),
						target.transform.FindRecursively("Bip001 Spine"));
					PlayAnimation(Self, TriggerSkill_2);
					return;
				case 3:
					RemoveBulletLine();
					PlayEffectPoint(Self, "FX_BI_WSkill_SniperRifle_02", "ShotPoint");
					PlayEffectPoint(Self, "FX_BI_WSkill_SniperRifle_01", "ShotPoint");
					PlaySoundPoint(Self, "skillSniperRifle_v1");
					return;
				case 4:
					ZoomOut();
					RemoveBulletLine();
					PlayAnimation(Self, TriggerSkill_3);
					return;
				default:
					return;
			}
		}


		public override void Finish(bool cancel)
		{
			ResetAnimatorTrigger(Self, TriggerReloadCancel);
			RotationLocalObject(Self, SelfForward);
			SingletonMonoBehaviour<PlayerController>.inst.SetCursorStatus(CursorStatus.Normal);
			UnlockSkillSlots(SkillSlotIndex.Active1);
			UnlockSkillSlots(SkillSlotIndex.Active2);
			UnlockSkillSlots(SkillSlotIndex.Active3);
			UnlockSkillSlots(SkillSlotIndex.Active4);
			UnlockSkillSlot(SkillSlotSet.WeaponSkill);
			DestroyBulletLine();
			if (cancel)
			{
				ZoomOut();
			}
		}


		private void ZoomIn()
		{
			if (SingletonMonoBehaviour<PlayerController>.inst.IsMe(Self.ObjectId))
			{
				preMode = MonoBehaviourInstance<MobaCamera>.inst.Mode;
				MonoBehaviourInstance<MobaCamera>.inst.SetZoomSpeed(data.ConcentrationTime);
				MonoBehaviourInstance<MobaCamera>.inst.ResetCameraPosition();
				MonoBehaviourInstance<MobaCamera>.inst.SetCameraMode(MobaCameraMode.Manual);
				MonoBehaviourInstance<MobaCamera>.inst.SetZOffset(15f);
				Vector3 forward = MonoBehaviourInstance<MobaCamera>.inst.transform.forward;
				forward.y = 0f;
				Vector3 selfForward = SelfForward;
				float num = -Vector3.Dot(forward.normalized, selfForward);
				MonoBehaviourInstance<MobaCamera>.inst.SetPivot(selfForward * (10f + num * 3f));
				BlockAllySight(Self, BlockedSightType.SniperRifleActive, true);
			}
		}


		private void ZoomOut()
		{
			if (SingletonMonoBehaviour<PlayerController>.inst.IsMe(Self.ObjectId))
			{
				MonoBehaviourInstance<MobaCamera>.inst.SetZoomSpeed(0.5f);
				MonoBehaviourInstance<MobaCamera>.inst.SetCameraMode(preMode != MobaCameraMode.Manual
					? preMode
					: MobaCameraMode.Tracking);
				MonoBehaviourInstance<MobaCamera>.inst.SetZOffset(0f);
				MonoBehaviourInstance<MobaCamera>.inst.SetPivot(Vector3.zero);
				BlockAllySight(Self, BlockedSightType.SniperRifleActive, false);
			}
		}


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return GameDB.characterState.GetData(Singleton<SniperRifleSkillActiveData>.inst.BuffState)
						.statValue2.ToString();
				case 1:
					return Singleton<SniperRifleSkillActiveData>.inst.aimingDelay.ToString();
				case 2:
					return ((int) (SelfStat.AttackPower *
					               Singleton<SniperRifleSkillActiveData>.inst.ApCoefficient[skillData.level]))
						.ToString();
				default:
					return "";
			}
		}


		public override string GetNextLevelTooltipParam(int index)
		{
			if (index == 0)
			{
				return "ToolTipType/Damage";
			}

			if (index != 1)
			{
				return "";
			}

			return "ToolTipType/CoolTime";
		}


		public override string GetNextLevelTooltipValue(SkillData skillData, int level, int index)
		{
			if (index == 0)
			{
				return ((int) (SelfStat.AttackPower * Singleton<SniperRifleSkillActiveData>.inst.ApCoefficient[level]))
					.ToString();
			}

			if (index != 1)
			{
				return "";
			}

			return skillData.cooldown.ToString();
		}
	}
}