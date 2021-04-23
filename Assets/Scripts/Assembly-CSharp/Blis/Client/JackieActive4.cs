using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.JackieActive4)]
	public class JackieActive4 : LocalSkillScript
	{
		public override void Start()
		{
			PlaySoundPoint(Self, "jackie_Skill04_Activation_v1");
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition)
		{
			if (action == 1)
			{
				PlayAnimation(Self, TriggerSkill04);
			}
		}


		public override void Finish(bool cancel) { }


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return GameDB.characterState
						.GetData(Singleton<JackieSkillActive4Data>.inst.BuffState[skillData.level]).duration
						.ToString("G");
				case 1:
				{
					CharacterStateData data =
						GameDB.characterState.GetData(
							Singleton<JackieSkillActive4Data>.inst.BuffState[skillData.level]);
					return string.Format("{0}%", data.statValue1);
				}
				case 2:
					return Singleton<JackieSkillActive4Data>.inst.FinishDamageByLevel[skillData.level].ToString();
				case 3:
					return ((int) (Singleton<JackieSkillActive4Data>.inst.FinishSkillApCoef * SelfStat.AttackPower))
						.ToString();
				default:
					return "";
			}
		}


		public override string GetNextLevelTooltipParam(int index)
		{
			switch (index)
			{
				case 0:
					return "ToolTipType/Time";
				case 1:
					return "ToolTipType/Damage";
				case 2:
					return "ToolTipType/CoolTime";
				case 3:
					return "ToolTipType/Cost";
				default:
					return "";
			}
		}


		public override string GetNextLevelTooltipValue(SkillData skillData, int level, int index)
		{
			switch (index)
			{
				case 0:
					return GameDB.characterState
						.GetData(Singleton<JackieSkillActive4Data>.inst.BuffState[skillData.level]).duration
						.ToString("G");
				case 1:
					return Singleton<JackieSkillActive4Data>.inst.FinishDamageByLevel[skillData.level].ToString();
				case 2:
					return skillData.cooldown.ToString();
				case 3:
					return skillData.cost.ToString();
				default:
					return "";
			}
		}
	}
}