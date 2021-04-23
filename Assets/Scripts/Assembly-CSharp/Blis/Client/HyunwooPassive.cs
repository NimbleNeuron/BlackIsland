using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.HyunwooPassive)]
	public class HyunwooPassive : LocalSkillScript
	{
		public override void Start() { }


		public override void Play(int action, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel) { }


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return GameDB.characterState
						.GetData(Singleton<HyunwooSkillPassiveData>.inst.BuffState[skillData.level]).maxStack
						.ToString();
				case 1:
				{
					int num = Mathf.RoundToInt(Singleton<HyunwooSkillPassiveData>.inst.HpCoef[skillData.level] * 100f);
					return string.Format("{0}%", num);
				}
				case 2:
					return Mathf
						.RoundToInt(Singleton<HyunwooSkillPassiveData>.inst.IncreasePassiveBuffStackCountOnAttack)
						.ToString();
				case 3:
					return Mathf.RoundToInt(Singleton<HyunwooSkillPassiveData>.inst.IncreasePassiveBuffStackCountOnHit)
						.ToString();
				default:
					return "";
			}
		}


		public override string GetNextLevelTooltipParam(int index)
		{
			if (index == 0)
			{
				return "ToolTipType/Heal";
			}

			return "";
		}


		public override string GetNextLevelTooltipValue(SkillData skillData, int level, int index)
		{
			if (index == 0)
			{
				int num = Mathf.RoundToInt(Singleton<HyunwooSkillPassiveData>.inst.HpCoef[skillData.level] * 100f);
				return string.Format("{0}%", num);
			}

			return "";
		}
	}
}