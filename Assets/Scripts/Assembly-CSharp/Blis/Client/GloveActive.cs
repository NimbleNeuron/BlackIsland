using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.GloveActive)]
	public class GloveActive : LocalSkillScript
	{
		private const string Glove_Sfx = "Globe_skill_D";


		public override void Start()
		{
			PlaySoundChildManual(Self, "Globe_skill_D", 15);
		}


		public override void Play(int actionNo, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel) { }


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return GameDB.characterState
						.GetData(Singleton<GloveSkillActiveData>.inst.BuffState[skillData.level]).duration.ToString();
				case 1:
					return Singleton<GloveSkillActiveData>.inst.UppercutIncreaseAttackRange[skillData.level].ToString();
				case 2:
					return ((int) (Singleton<GloveSkillActiveData>.inst.GloveUppercutAttackApCoef *
					               SelfStat.AttackPower)).ToString();
				case 3:
				{
					int num = Mathf.RoundToInt(
						Singleton<GloveSkillActiveData>.inst.UppercutFinalMoreDamage[skillData.level] * 100f);
					return string.Format("{0}%", num);
				}
				case 4:
					return Singleton<GloveSkillActiveData>.inst.DamageByLevel[skillData.level].ToString();
				default:
					return "";
			}
		}


		public override string GetNextLevelTooltipParam(int index)
		{
			if (index == 0)
			{
				return "ToolTipType/AdditionalDamage";
			}

			if (index != 1)
			{
				return "";
			}

			return "ToolTipType/TrueDamage";
		}


		public override string GetNextLevelTooltipValue(SkillData skillData, int level, int index)
		{
			if (index == 0)
			{
				int num = Mathf.RoundToInt(Singleton<GloveSkillActiveData>.inst.UppercutFinalMoreDamage[level] * 100f);
				return string.Format("{0}%", num);
			}

			if (index != 1)
			{
				return "";
			}

			return Singleton<GloveSkillActiveData>.inst.DamageByLevel[level].ToString();
		}
	}
}