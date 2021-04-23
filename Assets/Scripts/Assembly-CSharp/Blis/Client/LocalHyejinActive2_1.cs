using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.HyejinActive2_1)]
	public class LocalHyejinActive2_1 : LocalSkillScript
	{
		public override void Start()
		{
			PlayAnimation(Self, TriggerSkill02);
			PlaySoundPoint(Self, "Hyejin_Skill02_Bow_Fire", 15);
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel) { }


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return GameDB.character.GetSummonData(Singleton<HyejinSkillData>.inst.A2SummonCodeBow).duration
						.ToString();
				case 1:
					return GameDB.characterState.GetData(Singleton<HyejinSkillData>.inst.A2DebuffCode).duration
						.ToString();
				case 2:
					return Singleton<HyejinSkillData>.inst.A2BaseMinDamage[skillData.level].ToString();
				case 3:
					return ((int) (Singleton<HyejinSkillData>.inst.A2ApDamage * SelfStat.AttackPower)).ToString();
				case 4:
					return Singleton<HyejinSkillData>.inst.A2BaseMaxDamage[skillData.level].ToString();
				case 5:
					return ((int) (Singleton<HyejinSkillData>.inst.A2ApDamage * SelfStat.AttackPower)).ToString();
				default:
					return "";
			}
		}


		public override string GetNextLevelTooltipParam(int index)
		{
			switch (index)
			{
				case 0:
					return "ToolTipType/MinDamage";
				case 1:
					return "ToolTipType/MaxDamage";
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
					return Singleton<HyejinSkillData>.inst.A2BaseMinDamage[skillData.level].ToString();
				case 1:
					return Singleton<HyejinSkillData>.inst.A2BaseMaxDamage[skillData.level].ToString();
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