using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.HyejinActive4)]
	public class LocalHyejinActive4 : LocalSkillScript
	{
		public override void Start()
		{
			LockSkillSlot(SkillSlotSet.Active1_1);
			LockSkillSlot(SkillSlotSet.Active2_1);
			LockSkillSlot(SkillSlotSet.WeaponSkill);
			SetAnimation(Self, BooleanSkill04, true);
			SetAnimation(Self, BooleanSkill04_02, true);
			PlayAnimation(Self, TriggerSkill04);
			PlaySoundPoint(Self, "Hyejin_Skill04_Start", 15);
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition)
		{
			if (action == 1)
			{
				LockSkillSlot(SkillSlotSet.Active3_1);
				return;
			}

			if (action == 2)
			{
				SetAnimation(Self, BooleanSkill04, false);
				PlayAnimation(Self, TriggerSkill04_2);
				PlaySoundPoint(Self, "Hyejin_Skill04_Explore", 15);
			}
		}


		public override void Finish(bool cancel)
		{
			SetAnimation(Self, BooleanSkill04, false);
			SetAnimation(Self, BooleanSkill04_02, false);
			UnlockSkillSlot(SkillSlotSet.Active1_1);
			UnlockSkillSlot(SkillSlotSet.Active2_1);
			UnlockSkillSlot(SkillSlotSet.Active3_1);
			UnlockSkillSlot(SkillSlotSet.WeaponSkill);
		}


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return Singleton<HyejinSkillData>.inst.A4ConcentrationEndBaseDamage[skillData.level].ToString();
				case 1:
					return ((int) (Singleton<HyejinSkillData>.inst.A4ConcentrationEndApDamage * SelfStat.AttackPower))
						.ToString();
				case 2:
					return GameDB.characterState.GetData(Singleton<HyejinSkillData>.inst.A4ProjectileState).duration
						.ToString();
				case 3:
					return Singleton<HyejinSkillData>.inst.A4ProjectileBaseDamage[skillData.level].ToString();
				case 4:
					return ((int) (Singleton<HyejinSkillData>.inst.A4ProjectileApDamage * SelfStat.AttackPower))
						.ToString();
				case 5:
					return Mathf.Abs(GameDB.characterState
						.GetData(Singleton<HyejinSkillData>.inst.A4ConcentrationDebuffCode).statValue1) + "%";
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
					return "ToolTipType/ProjectileDamage";
				case 2:
					return "ToolTipType/CoolTime";
				default:
					return "";
			}
		}


		public override string GetNextLevelTooltipValue(SkillData skillData, int level, int index)
		{
			switch (index)
			{
				case 0:
					return Singleton<HyejinSkillData>.inst.A4ConcentrationEndBaseDamage[skillData.level].ToString();
				case 1:
					return Singleton<HyejinSkillData>.inst.A4ProjectileBaseDamage[skillData.level].ToString();
				case 2:
					return skillData.cooldown.ToString();
				default:
					return "";
			}
		}
	}
}