using System;
using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.HammerActive)]
	public class HammerActive : LocalSkillScript
	{
		protected static readonly int TriggerSkill = Animator.StringToHash("tHammer_Skill");


		public override void Start()
		{
			PlayAnimation(Self, TriggerSkill);
		}


		public override void Play(int actionNo, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel) { }


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return Singleton<HammerSkillActiveData>.inst.DamageByLevel[skillData.level].ToString();
				case 1:
					return ((int) (Singleton<HammerSkillActiveData>.inst.DefCoefficient[skillData.level] *
					               SelfStat.Defense)).ToString();
				case 2:
					return GameDB.characterState
						.GetData(Singleton<HammerSkillActiveData>.inst.DebuffState[skillData.level]).duration
						.ToString();
				case 3:
				{
					int num = Math.Abs((int) GameDB.characterState
						.GetData(Singleton<HammerSkillActiveData>.inst.DebuffState[skillData.level]).statValue1);
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
					return "ToolTipType/Damage";
				case 1:
					return "ToolTipType/SkillDfCoef";
				case 2:
					return "ToolTipType/DecreaseDefenseRatio";
				case 3:
					return "ToolTipType/Time";
				default:
					return "";
			}
		}


		public override string GetNextLevelTooltipValue(SkillData skillData, int level, int index)
		{
			switch (index)
			{
				case 0:
					return Singleton<HammerSkillActiveData>.inst.DamageByLevel[level].ToString();
				case 1:
					return Singleton<HammerSkillActiveData>.inst.DefCoefficient[level].ToString();
				case 2:
				{
					int num = Math.Abs((int) GameDB.characterState
						.GetData(Singleton<HammerSkillActiveData>.inst.DebuffState[level]).statValue1);
					return string.Format("{0}%", num);
				}
				case 3:
					return GameDB.characterState.GetData(Singleton<HammerSkillActiveData>.inst.DebuffState[level])
						.duration.ToString();
				default:
					return "";
			}
		}
	}
}