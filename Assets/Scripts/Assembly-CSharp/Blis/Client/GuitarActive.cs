using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.GuitarActive)]
	public class GuitarActive : LocalSkillScript
	{
		private const string FX_BI_WSkill_Guitar_01 = "FX_BI_WSkill_Guitar_01";


		protected static readonly int TriggerSkill = Animator.StringToHash("tGuitar_Skill");


		public override void Start()
		{
			PlayAnimation(Self, TriggerSkill);
		}


		public override void Play(int actionNo, LocalObject target, Vector3? targetPosition)
		{
			if (actionNo == 1)
			{
				PlayEffectPoint(Self, "FX_BI_WSkill_Guitar_01");
			}
		}


		public override void Finish(bool cancel) { }


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			if (index == 0)
			{
				return ((int) (SelfStat.AttackPower *
				               Singleton<GuitarSkillActiveData>.inst.SkillApCoef[skillData.level])).ToString();
			}

			if (index != 1)
			{
				return "";
			}

			return Singleton<GuitarSkillActiveData>.inst.CharmDuration.ToString();
		}


		public override string GetNextLevelTooltipParam(int index)
		{
			if (index == 0)
			{
				return "ToolTipType/Damage";
			}

			return "";
		}


		public override string GetNextLevelTooltipValue(SkillData skillData, int level, int index)
		{
			if (index == 0)
			{
				return ((int) (SelfStat.AttackPower * Singleton<GuitarSkillActiveData>.inst.SkillApCoef[level]))
					.ToString();
			}

			return "";
		}
	}
}