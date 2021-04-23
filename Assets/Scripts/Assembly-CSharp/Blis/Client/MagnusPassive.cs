using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.MagnusPassive)]
	public class MagnusPassive : LocalSkillScript
	{
		public override void Start() { }


		public override void Play(int action, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel) { }


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			if (index == 0)
			{
				float num = GameDB.characterState
					.GetData(Singleton<MagnusSkillPassiveData>.inst.BuffState[skillData.level]).statValue1 / 100f;
				return string.Format("{0}%", num);
			}

			return "";
		}


		public override string GetNextLevelTooltipParam(int index)
		{
			if (index == 0)
			{
				return "StatType/DefenseRatio";
			}

			return "";
		}


		public override string GetNextLevelTooltipValue(SkillData skillData, int level, int index)
		{
			if (index == 0)
			{
				float num = GameDB.characterState
					.GetData(Singleton<MagnusSkillPassiveData>.inst.BuffState[skillData.level]).statValue1 / 100f;
				return string.Format("{0}%", num);
			}

			return "";
		}
	}
}