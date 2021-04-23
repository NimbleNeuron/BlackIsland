using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.XiukaiPassiveBuff)]
	public class XiukaiPassiveBuff : LocalSkillScript
	{
		public override void Start() { }


		public override void Play(int action, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel) { }


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
				{
					int addRecoveryPercent = Singleton<XiukaiSkillPassiveData>.inst.AddRecoveryPercent;
					return string.Format("{0}%", addRecoveryPercent);
				}
				case 1:
					return ((int) GameDB.characterState.GetData(Singleton<XiukaiSkillPassiveData>.inst.BuffState)
						.calculatorParameter).ToString();
				case 2:
					return Singleton<XiukaiSkillPassiveData>.inst.UncommonStack[skillData.level].ToString();
				case 3:
					return Singleton<XiukaiSkillPassiveData>.inst.RareStack[skillData.level].ToString();
				case 4:
					return Singleton<XiukaiSkillPassiveData>.inst.EpicStack[skillData.level].ToString();
				case 5:
					return Singleton<XiukaiSkillPassiveData>.inst.LegendStack[skillData.level].ToString();
				case 6:
				{
					int addMaxHp = Singleton<XiukaiSkillPassiveData>.inst.AddMaxHp;
					return addMaxHp.ToString();
				}
				default:
					return "";
			}
		}


		public override string GetNextLevelTooltipParam(int index)
		{
			return "";
		}


		public override string GetNextLevelTooltipValue(SkillData skillData, int level, int index)
		{
			switch (index)
			{
				case 0:
					return Singleton<XiukaiSkillPassiveData>.inst.UncommonStack[level].ToString();
				case 1:
					return Singleton<XiukaiSkillPassiveData>.inst.RareStack[level].ToString();
				case 2:
					return Singleton<XiukaiSkillPassiveData>.inst.EpicStack[level].ToString();
				case 3:
					return Singleton<XiukaiSkillPassiveData>.inst.LegendStack[level].ToString();
				default:
					return "";
			}
		}
	}
}