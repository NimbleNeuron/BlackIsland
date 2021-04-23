using System;
using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.AyaPassive)]
	public class AyaPassive : LocalSkillScript
	{
		public override void Start() { }


		public override void Play(int action, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel) { }


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return GameDB.characterState.GetData(Singleton<AyaSkillPassiveData>.inst.BuffState).duration
						.ToString();
				case 1:
					return Singleton<AyaSkillPassiveData>.inst.ShieldByLevel[skillData.level].ToString();
				case 2:
					return ((int) (Singleton<AyaSkillPassiveData>.inst.SkillApCoef * SelfStat.AttackPower)).ToString();
				case 3:
					return Math.Abs((int) Singleton<AyaSkillPassiveData>.inst.CooldownReduce).ToString();
				default:
					return "";
			}
		}


		public override string GetNextLevelTooltipParam(int index)
		{
			if (index == 0)
			{
				return "ToolTipType/Shield";
			}

			return "";
		}


		public override string GetNextLevelTooltipValue(SkillData skillData, int level, int index)
		{
			if (index == 0)
			{
				return Singleton<AyaSkillPassiveData>.inst.ShieldByLevel[skillData.level].ToString();
			}

			return "";
		}
	}
}