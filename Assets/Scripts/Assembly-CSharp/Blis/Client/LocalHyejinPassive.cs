using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.HyejinPassive)]
	public class LocalHyejinPassive : LocalSkillScript
	{
		public override void Start() { }


		public override void Play(int action, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel) { }


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return Singleton<HyejinSkillData>.inst.PassiveTriggerStack.ToString();
				case 1:
					return GameDB.characterState
						.GetData(Singleton<HyejinSkillData>.inst.PassiveFearState[skillData.level]).duration.ToString();
				case 2:
					return GameDB.characterState.GetData(Singleton<HyejinSkillData>.inst.PassiveSamjeaImmuneState)
						.duration.ToString();
				default:
					return "";
			}
		}


		public override string GetNextLevelTooltipParam(int index)
		{
			if (index == 0)
			{
				return "ToolTipType/FearTime";
			}

			return "";
		}


		public override string GetNextLevelTooltipValue(SkillData skillData, int level, int index)
		{
			if (index == 0)
			{
				return GameDB.characterState.GetData(Singleton<HyejinSkillData>.inst.PassiveFearState[skillData.level])
					.duration.ToString();
			}

			return "";
		}
	}
}