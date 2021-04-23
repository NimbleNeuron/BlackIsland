using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.TonfaActive)]
	public class TonfaActive : LocalSkillScript
	{
		protected static readonly int BooleanSkill = Animator.StringToHash("bTonfa_Skill");


		public override void Start()
		{
			SetAnimation(Self, BooleanSkill, true);
			PlayEffectPoint(Self, "FX_BI_WSkill_Tonfa_01");
		}


		public override void Play(int actionNo, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel)
		{
			SetAnimation(Self, BooleanSkill, false);
		}


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			if (index == 0)
			{
				return GameDB.characterState.GetData(Singleton<TonfaSkillActiveData>.inst.BuffState[skillData.level])
					.duration.ToString();
			}

			if (index != 1)
			{
				return "";
			}

			return (1f + Singleton<TonfaSkillActiveData>.inst.ReturnApCoef[skillData.level]).ToString();
		}


		public override string GetNextLevelTooltipParam(int index)
		{
			if (index == 0)
			{
				return "ToolTipType/ReturnDamageCoef";
			}

			return "";
		}


		public override string GetNextLevelTooltipValue(SkillData skillData, int level, int index)
		{
			if (index == 0)
			{
				return (1f + Singleton<TonfaSkillActiveData>.inst.ReturnApCoef[level]).ToString();
			}

			return "";
		}
	}
}