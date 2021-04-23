using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.PistolActive)]
	public class PistolActive : LocalSkillScript
	{
		public override void Start() { }


		public override void Play(int actionNo, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel) { }


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return GameDB.characterState
						.GetData(Singleton<PistolSkillActiveData>.inst.BuffState[skillData.level]).duration.ToString();
				case 1:
				{
					int num = (int) GameDB.characterState
						.GetData(Singleton<PistolSkillActiveData>.inst.BuffState[skillData.level]).statValue1;
					return string.Format("{0}%", num);
				}
				case 2:
					return Mathf.Abs(Singleton<PistolSkillActiveData>.inst.SkillCooldownReduce).ToString();
				default:
					return "";
			}
		}


		public override string GetNextLevelTooltipParam(int index)
		{
			if (index == 0)
			{
				return "StatType/MoveSpeedRatio";
			}

			if (index != 1)
			{
				return "";
			}

			return "ToolTipType/CoolTime";
		}


		public override string GetNextLevelTooltipValue(SkillData skillData, int level, int index)
		{
			if (index == 0)
			{
				int num = (int) GameDB.characterState
					.GetData(Singleton<PistolSkillActiveData>.inst.BuffState[skillData.level]).statValue1;
				return string.Format("{0}%", num);
			}

			if (index != 1)
			{
				return "";
			}

			return skillData.cooldown.ToString();
		}
	}
}