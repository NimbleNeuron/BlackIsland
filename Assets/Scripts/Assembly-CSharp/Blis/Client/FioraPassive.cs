using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.FioraPassive)]
	public class FioraPassive : LocalSkillScript
	{
		public override void Start() { }


		public override void Play(int actionNo, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel) { }


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			if (index == 0)
			{
				return GameDB.characterState.GetData(Singleton<FioraSkillPassiveData>.inst.DebuffState[skillData.level])
					.maxStack.ToString();
			}

			if (index != 1)
			{
				return "";
			}

			return GameDB.characterState.GetData(Singleton<FioraSkillPassiveData>.inst.DebuffState[skillData.level])
				.duration.ToString();
		}


		public override string GetNextLevelTooltipParam(int index)
		{
			if (index == 0)
			{
				return "ToolTipType/MaxStack";
			}

			return "";
		}


		public override string GetNextLevelTooltipValue(SkillData skillData, int level, int index)
		{
			if (index == 0)
			{
				return GameDB.characterState.GetData(Singleton<FioraSkillPassiveData>.inst.DebuffState[level]).maxStack
					.ToString();
			}

			return "";
		}
	}
}