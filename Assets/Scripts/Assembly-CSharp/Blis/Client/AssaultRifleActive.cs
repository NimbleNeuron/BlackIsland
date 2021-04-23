using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.AssaultRifleActive)]
	public class AssaultRifleActive : LocalSkillScript
	{
		public override void Start() { }


		public override void Play(int actionNo, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel) { }


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return Singleton<AssaultRifleSkillActiveData>.inst.OverHeatAddStack.ToString();
				case 1:
					return GameDB.characterState.GetData(Singleton<AssaultRifleSkillActiveData>.inst.AssaultPassiveBuff)
						.maxStack.ToString();
				case 2:
					return Singleton<AssaultRifleSkillActiveData>.inst.OverHeatRemoveTime.ToString();
				case 3:
					return Singleton<AssaultRifleSkillActiveData>.inst.OverHeatRemoveStack.ToString();
				case 4:
					return GameDB.characterState
						.GetData(Singleton<AssaultRifleSkillActiveData>.inst.BuffState[skillData.level]).duration
						.ToString();
				case 5:
				{
					CharacterStateData data =
						GameDB.characterState.GetData(
							Singleton<AssaultRifleSkillActiveData>.inst.BuffState[skillData.level]);
					return string.Format("{0}%", data.statValue2);
				}
				case 6:
					return Singleton<AssaultRifleSkillActiveData>.inst.OverHeatCheckTime.ToString();
				default:
					return "";
			}
		}


		public override string GetNextLevelTooltipParam(int index)
		{
			if (index == 0)
			{
				return "ToolTipType/Time";
			}

			if (index != 1)
			{
				return "";
			}

			return "StatType/AttackSpeedRatio";
		}


		public override string GetNextLevelTooltipValue(SkillData skillData, int level, int index)
		{
			if (index == 0)
			{
				return GameDB.characterState.GetData(Singleton<AssaultRifleSkillActiveData>.inst.BuffState[level])
					.duration.ToString();
			}

			if (index != 1)
			{
				return "";
			}

			CharacterStateData data =
				GameDB.characterState.GetData(Singleton<AssaultRifleSkillActiveData>.inst.BuffState[level]);
			return string.Format("{0}%", data.statValue2);
		}
	}
}