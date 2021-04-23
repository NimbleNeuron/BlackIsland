using System;
using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.EmmaActive3)]
	public class EmmaActive3 : LocalSkillScript
	{
		public override void Start()
		{
			PlayAnimation(Self, TriggerSkill03);
			SetAnimation(Self, BooleanMotionWait, true);
			PlaySoundPoint(Self, "Emma_Skill03_Fire", 15);
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel) { }


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return GameDB.characterState
						.GetData(Singleton<EmmaSkillActive3Data>.inst.MagicRabbitStateCode[skillData.level]).duration
						.ToString();
				case 1:
					return GameDB.characterState
						.GetData(Singleton<EmmaSkillActive3Data>.inst.MagicRabbitFetterStateCode).duration.ToString();
				case 2:
					return Math.Abs(GameDB.characterState
							.GetData(Singleton<EmmaSkillActive3Data>.inst.MagicRabbitStateCode[skillData.level])
							.statValue1)
						.ToString();
				case 3:
				{
					int num = (int) Math.Abs(
						Singleton<EmmaSkillActive3Data>.inst.HealRatioPerConsumeSPBySkillLevel[skillData.level] * 100f);
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
					return "ToolTipType/StaminaCostRatio";
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
					return ((int) Math.Abs(
							Singleton<EmmaSkillActive3Data>.inst.HealRatioPerConsumeSPBySkillLevel[level] * 100f))
						.ToString();
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