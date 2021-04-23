using System;
using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.RozziActive2)]
	public class LocalRozziActive2 : LocalSkillScript
	{
		public override void Start()
		{
			LockSkillSlot(SkillSlotSet.Attack_1);
			LockSkillSlot(SkillSlotSet.Active1_1);
			LockSkillSlot(SkillSlotSet.Active3_1);
			LockSkillSlot(SkillSlotSet.Active4_1);
			LockSkillSlot(SkillSlotSet.WeaponSkill);
			PlayAnimation(Self, TriggerReloadCancel);
			PlayAnimation(Self, TriggerSkill02);
			SetAnimation(Self, BooleanSkill02, true);
			SetAnimation(Self, BooleanMotionWait, true);
			PlaySoundPoint(Self, "Rozzi_Skill02_Fire", 15);
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel)
		{
			UnlockSkillSlot(SkillSlotSet.Attack_1);
			UnlockSkillSlot(SkillSlotSet.Active1_1);
			UnlockSkillSlot(SkillSlotSet.Active3_1);
			UnlockSkillSlot(SkillSlotSet.Active4_1);
			UnlockSkillSlot(SkillSlotSet.WeaponSkill);
			ResetAnimatorTrigger(Self, TriggerReloadCancel);
			SetAnimation(Self, BooleanSkill02, false);
			SetAnimation(Self, BooleanMotionWait, false);
		}


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return Singleton<RozziSkillActive2Data>.inst.DamageByLevel[skillData.level].ToString();
				case 1:
					return ((int) (Singleton<RozziSkillActive2Data>.inst.DamageApCoef * SelfStat.AttackPower))
						.ToString();
				case 2:
				{
					CharacterStateData data =
						GameDB.characterState.GetData(Singleton<RozziSkillActive2Data>.inst.Active2SpeedUpStateCode);
					return string.Format("{0}%", data.statValue1);
				}
				case 3:
					return GameDB.characterState
						.GetData(Singleton<RozziSkillActive2Data>.inst.DebuffStateCodeByLevel[skillData.level]).duration
						.ToString();
				case 4:
				{
					int num = (int) Math.Abs(GameDB.characterState
						.GetData(Singleton<RozziSkillActive2Data>.inst.DebuffStateCodeByLevel[skillData.level])
						.statValue1);
					return string.Format("{0}%", num);
				}
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
					return "ToolTipType/CoolTime";
				case 2:
					return "ToolTipType/Cost";
				case 3:
					return "ToolTipType/DecreaseDefenseRatio";
				default:
					return "";
			}
		}


		public override string GetNextLevelTooltipValue(SkillData skillData, int level, int index)
		{
			switch (index)
			{
				case 0:
					return Singleton<RozziSkillActive2Data>.inst.DamageByLevel[level].ToString();
				case 1:
					return skillData.cooldown.ToString();
				case 2:
					return skillData.cost.ToString();
				case 3:
				{
					int num = (int) Math.Abs(GameDB.characterState
						.GetData(Singleton<RozziSkillActive2Data>.inst.DebuffStateCodeByLevel[level]).statValue1);
					return string.Format("{0}%", num);
				}
				default:
					return "";
			}
		}
	}
}