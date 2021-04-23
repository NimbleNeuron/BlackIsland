using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.XiukaiActive1)]
	public class XiukaiActive1 : LocalSkillScript
	{
		private const string Skill01_Ground = "FX_BI_Xiukai_Skill02_Start";


		public override void Start()
		{
			PlayAnimation(Self, TriggerSkill01);
			SetAnimation(Self, BooleanSkill01, true);
			SetAnimation(Self, BooleanMotionWait, true);
			StartCoroutine(CoroutineUtil.DelayedAction(1f, delegate { SetAnimation(Self, BooleanSkill01, false); }));
			PlayAnimation(Self, TriggerSkill01);
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel) { }


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return Singleton<XiukaiSkillActive1Data>.inst.BaseDamage[skillData.level].ToString();
				case 1:
					return ((int) (Singleton<XiukaiSkillActive1Data>.inst.SkillApCoef * SelfStat.AttackPower))
						.ToString();
				case 2:
					return GameDB.characterState
						.GetData(Singleton<XiukaiSkillActive1Data>.inst.DebuffState[skillData.level]).duration
						.ToString();
				case 3:
				{
					CharacterStateData data =
						GameDB.characterState.GetData(
							Singleton<XiukaiSkillActive1Data>.inst.DebuffState[skillData.level]);
					return string.Format("{0}%", Mathf.Abs(data.statValue1));
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
					return "ToolTipType/Damage";
				case 1:
					return "ToolTipType/CoolTime";
				case 2:
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
					return Singleton<XiukaiSkillActive1Data>.inst.BaseDamage[level].ToString();
				case 1:
					return skillData.cooldown.ToString();
				case 2:
					return skillData.cost.ToString();
				default:
					return "";
			}
		}
	}
}