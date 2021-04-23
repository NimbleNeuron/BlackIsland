using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.SilviaBikeActive4)]
	public class LocalSilviaBikeActive4 : LocalSkillScript
	{
		public override void Start() { }


		public override void Play(int action, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel) { }


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			if (index == 0)
			{
				return Mathf
					.RoundToInt(
						Singleton<SilviaSkillHumanData>.inst.NormalAttackReinforceApCoef[skillData.level] * 100f)
					.ToString();
			}

			if (index != 1)
			{
				return "";
			}

			return GameDB.characterState.GetData(Singleton<SilviaSkillHumanData>.inst.NormalAttackReinforceState)
				.duration.ToString();
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
				int num = Mathf.RoundToInt(Singleton<SilviaSkillHumanData>.inst.NormalAttackReinforceApCoef[level] *
				                           100f);
				return string.Format("{0}%", num);
			}

			return "";
		}
	}
}