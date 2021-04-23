using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.RozziActive3_1)]
	public class LocalRozziActive3_1 : LocalSkillScript
	{
		public override void Start() { }


		public override void Play(int action, LocalObject target, Vector3? targetPosition)
		{
			if (action == 1)
			{
				PlayAnimation(Self, TriggerReloadCancel);
				PlayAnimation(Self, TriggerSkill03);
				SetAnimation(Self, BooleanSkill03, true);
				SwitchMaterialChildManualFromDefault(Self, "Rozzi_01_LOD1", 0, "Rozzi_01_LOD1_Skill");
				PlayEffectPoint(Self, "FX_BI_Rozzi_Skill03_JumpSmoke");
				PlaySoundPoint(Self, "Rozzi_Skill03_Start", 15);
				return;
			}

			if (action == 2)
			{
				PlayEffectPoint(Self, "FX_BI_Rozzi_Skill03_Ground");
				PlaySoundPoint(Self, "Rozzi_Skill03_Gun", 15);
			}
		}


		public override void Finish(bool cancel)
		{
			ResetAnimatorTrigger(Self, TriggerReloadCancel);
			SetAnimation(Self, BooleanSkill03, false);
			SwitchMaterialChildManualFromDefault(Self, "Rozzi_01_LOD1", 0, "Rozzi_01_LOD1");
		}


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return Singleton<RozziSkillActive3Data>.inst.DamageActive3_1ByLevel[skillData.level].ToString();
				case 1:
					return ((int) (Singleton<RozziSkillActive3Data>.inst.DamageActive3_1ApCoef * SelfStat.AttackPower))
						.ToString();
				case 2:
					return GameDB.skill.GetSkillGroupData(Singleton<RozziSkillActive3Data>.inst.Active3_2Group)
						.castWaitTime.ToString();
				case 3:
					return Singleton<RozziSkillActive3Data>.inst.DamageActive3_2ByLevel[skillData.level].ToString();
				case 4:
					return ((int) (Singleton<RozziSkillActive3Data>.inst.DamageActive3_2ApCoef * SelfStat.AttackPower))
						.ToString();
				case 5:
					return Singleton<RozziSkillActive3Data>.inst.Active3KnockBackStunDuration.ToString();
				default:
					return "";
			}
		}


		public override string GetNextLevelTooltipParam(int index)
		{
			switch (index)
			{
				case 0:
					return "ToolTipType/FirstDamage";
				case 1:
					return "ToolTipType/SecondDamage";
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
					return Singleton<RozziSkillActive3Data>.inst.DamageActive3_1ByLevel[level].ToString();
				case 1:
					return Singleton<RozziSkillActive3Data>.inst.DamageActive3_2ByLevel[level].ToString();
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