using System;
using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.IsolActive4)]
	public class LocalIsolActive4 : LocalSkillScript
	{
		public override void Start()
		{
			PlayAnimation(Self, TriggerSkill04);
			PlaySoundPoint(Self, "Isol_Skill04_Active", 15);
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel) { }


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return ((int) GameDB.character.GetSummonData(Singleton<IsolSkillActive4Data>.inst.SummonObjectCode)
						.duration).ToString();
				case 1:
					return Singleton<IsolSkillActive4Data>.inst.Damage[skillData.level].ToString();
				case 2:
					return ((int) (Singleton<IsolSkillActive4Data>.inst.SkillApCoef[skillData.level] *
					               SelfStat.AttackPower)).ToString();
				case 3:
					return GameDB.characterState
						.GetData(Singleton<IsolSkillActive4Data>.inst.DebuffState[skillData.level]).duration.ToString();
				case 4:
					return (int) Math.Abs(GameDB.characterState
						.GetData(Singleton<IsolSkillActive4Data>.inst.DebuffState[skillData.level]).statValue1) + "%";
				case 5:
					return ((int) (Singleton<IsolSkillActive4Data>.inst.Damage[skillData.level] *
					               SelfStat.TrapDamageRatio)).ToString();
				case 6:
					return ((int) (Singleton<IsolSkillActive4Data>.inst.SkillApCoef[skillData.level] *
					               SelfStat.AttackPower * SelfStat.TrapDamageRatio)).ToString();
				default:
					return "";
			}
		}


		public override string GetNextLevelTooltipParam(int index)
		{
			if (index == 0)
			{
				return "ToolTipType/Damage";
			}

			if (index != 1)
			{
				return "";
			}

			return "ToolTipType/DecreaseMoveRatio";
		}


		public override string GetNextLevelTooltipValue(SkillData skillData, int level, int index)
		{
			if (index == 0)
			{
				return Singleton<IsolSkillActive4Data>.inst.Damage[level].ToString() ?? "";
			}

			if (index != 1)
			{
				return "";
			}

			return (int) Math.Abs(GameDB.characterState
				.GetData(Singleton<IsolSkillActive4Data>.inst.DebuffState[level]).statValue1) + "%";
		}
	}
}