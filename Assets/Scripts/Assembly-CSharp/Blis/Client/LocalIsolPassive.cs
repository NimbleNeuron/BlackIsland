using System;
using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.IsolPassive)]
	public class LocalIsolPassive : LocalSkillScript
	{
		private readonly float installTrap = 0.5f;

		private void Ref()
		{
			Reference.Use(installTrap);
		}


		public override void Start() { }


		public override void Play(int action, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel) { }


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return Mathf.Abs(Singleton<IsolSkillPassiveData>.inst.InstallTrapCastingTimeReduce[skillData.level])
						.ToString();
				case 1:
					return Mathf.Abs(GameDB.character.GetSummonData(2).attackSpeed -
					                 Singleton<IsolSkillPassiveData>.inst.InstallTrapCreateVisibleTime[skillData.level])
						.ToString();
				case 2:
					return Mathf.Abs(GameDB.characterState
						.GetData(Singleton<IsolSkillPassiveData>.inst.InstallTrapAdditionalStateEffect[skillData.level])
						.duration).ToString();
				case 3:
				{
					int num = (int) Math.Abs(GameDB.characterState
						.GetData(Singleton<IsolSkillPassiveData>.inst.InstallTrapAdditionalStateEffect[skillData.level])
						.statValue1);
					return string.Format("{0}%", num);
				}
				default:
					return "";
			}
		}


		public override string GetNextLevelTooltipParam(int index)
		{
			switch (index)
			{
				case 0:
					return "ToolTipType/InstallTrapTime";
				case 1:
					return "ToolTipType/CreateVisibleTime";
				case 2:
					return "ToolTipType/DecreaseDefenseRatio";
				default:
					return "";
			}
		}


		public override string GetNextLevelTooltipValue(SkillData skillData, int level, int index)
		{
			switch (index)
			{
				case 0:
					return Mathf.Abs(Singleton<IsolSkillPassiveData>.inst.InstallTrapCastingTimeReduce[level])
						.ToString();
				case 1:
					return Mathf.Abs(GameDB.character.GetSummonData(2).attackSpeed -
					                 Singleton<IsolSkillPassiveData>.inst.InstallTrapCreateVisibleTime[level])
						.ToString();
				case 2:
				{
					int num = (int) Math.Abs(GameDB.characterState
						.GetData(Singleton<IsolSkillPassiveData>.inst.InstallTrapAdditionalStateEffect[level])
						.statValue1);
					return string.Format("{0}%", num);
				}
				default:
					return "";
			}
		}
	}
}