using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.MagnusActive4)]
	public class MagnusActive4 : LocalSkillScript
	{
		private MobaCameraMode preMode;


		public override void Start()
		{
			SetAnimation(Self, BooleanSkill04, true);
			PlayAnimation(Self, TriggerSkill04);
			PlaySoundChildManual(Self, "magnus_Skill04_Drive", 15, true);
			LockSkillSlot(SkillSlotSet.Active1_1);
			LockSkillSlot(SkillSlotSet.Active2_1);
			LockSkillSlot(SkillSlotSet.Active3_1);
			LockSkillSlot(SkillSlotSet.WeaponSkill);
			if (SingletonMonoBehaviour<PlayerController>.inst.IsMe(Self.ObjectId))
			{
				MonoBehaviourInstance<MobaCamera>.inst.SetZoomSpeed(0.5f);
				MonoBehaviourInstance<MobaCamera>.inst.ResetCameraPosition();
				preMode = MonoBehaviourInstance<MobaCamera>.inst.Mode;
				MonoBehaviourInstance<MobaCamera>.inst.SetCameraMode(MobaCameraMode.Tracking);
				MonoBehaviourInstance<MobaCamera>.inst.SetZOffset(5f);
			}
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition)
		{
			if (action == 1)
			{
				SetAnimation(Self, BooleanSkill04, false);
				PlayAnimation(Self, TriggerSkill04_2);
			}
		}


		public override void Finish(bool cancel)
		{
			SetAnimation(Self, BooleanSkill04, false);
			StopSoundChildManual(Self, "magnus_Skill04_Drive");
			UnlockSkillSlot(SkillSlotSet.Active1_1);
			UnlockSkillSlot(SkillSlotSet.Active2_1);
			UnlockSkillSlot(SkillSlotSet.Active3_1);
			UnlockSkillSlot(SkillSlotSet.WeaponSkill);
			if (SingletonMonoBehaviour<PlayerController>.inst.IsMe(Self.ObjectId))
			{
				MonoBehaviourInstance<MobaCamera>.inst.SetZoomSpeed(0.5f);
				MonoBehaviourInstance<MobaCamera>.inst.SetCameraMode(preMode);
				MonoBehaviourInstance<MobaCamera>.inst.SetZOffset(0f);
			}
		}


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return Singleton<MagnusSkillActive4Data>.inst.SkillDuration[skillData.level].ToString();
				case 1:
					return Singleton<MagnusSkillActive4Data>.inst.DamageByLevel[skillData.level].ToString();
				case 2:
					return ((int) (Singleton<MagnusSkillActive4Data>.inst.SkillApCoef[skillData.level] *
					               SelfStat.AttackPower)).ToString();
				default:
					return "";
			}
		}


		public override string GetNextLevelTooltipParam(int index)
		{
			switch (index)
			{
				case 0:
					return "ToolTipType/Damage";
				case 1:
					return "ToolTipType/Time";
				case 2:
					return "ToolTipType/CoolTime";
				case 3:
					return "ToolTipType/Cost";
				default:
					return "";
			}
		}


		public override string GetNextLevelTooltipValue(SkillData skillData, int level, int index)
		{
			switch (index)
			{
				case 0:
					return Singleton<MagnusSkillActive4Data>.inst.DamageByLevel[skillData.level].ToString();
				case 1:
					return Singleton<MagnusSkillActive4Data>.inst.SkillDuration[skillData.level].ToString();
				case 2:
					return skillData.cooldown.ToString();
				case 3:
					return skillData.cost.ToString();
				default:
					return "";
			}
		}
	}
}