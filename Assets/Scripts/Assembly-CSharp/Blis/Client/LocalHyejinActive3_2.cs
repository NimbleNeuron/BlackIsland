using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.HyejinActive3_2)]
	public class LocalHyejinActive3_2 : LocalSkillScript
	{
		private const string Skill03_Arrive_Effect01 = "FX_BI_Hyejin_Skill03_Explore";


		private const string Skill03_Arrive_Effect02 = "FX_BI_Hyejin_Skill03_Ground";


		private const string Skill03_Arrive_Sound01 = "Hyejin_Skill03_Explore";


		public override void Start()
		{
			SetAnimation(Self, BooleanSkill03, true);
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition)
		{
			if (action == 1)
			{
				PlayEffectPoint(Self, "FX_BI_Hyejin_Skill03_Explore");
				PlayAnimation(Self, TriggerSkill03_2);
				PlaySoundPoint(Self, "Hyejin_Skill03_Move", 15);
				return;
			}

			if (action == 2)
			{
				PlayEffectPoint(Self, "FX_BI_Hyejin_Skill03_Explore");
				PlayEffectPoint(Self, "FX_BI_Hyejin_Skill03_Ground");
				PlaySoundPoint(Self, "Hyejin_Skill03_Explore", 15);
			}
		}


		public override void Finish(bool cancel)
		{
			SetAnimation(Self, BooleanSkill03, false);
		}


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return Singleton<HyejinSkillData>.inst.A3_1BaseDamage[skillData.level].ToString();
				case 1:
					return ((int) (Singleton<HyejinSkillData>.inst.A3_1ApDamage * SelfStat.AttackPower)).ToString();
				case 2:
					return Singleton<HyejinSkillData>.inst.A3_2BaseDamage[skillData.level].ToString();
				case 3:
					return ((int) (Singleton<HyejinSkillData>.inst.A3_2ApDamage * SelfStat.AttackPower)).ToString();
				default:
					return "";
			}
		}


		public override string GetNextLevelTooltipParam(int index)
		{
			if (index == 0)
			{
				return "ToolTipType/ProjectileDamage";
			}

			if (index != 1)
			{
				return "";
			}

			return "ToolTipType/MoveDamage";
		}


		public override string GetNextLevelTooltipValue(SkillData skillData, int level, int index)
		{
			if (index == 0)
			{
				return Singleton<HyejinSkillData>.inst.A3_1BaseDamage[skillData.level].ToString();
			}

			if (index != 1)
			{
				return "";
			}

			return Singleton<HyejinSkillData>.inst.A3_2BaseDamage[skillData.level].ToString();
		}
	}
}