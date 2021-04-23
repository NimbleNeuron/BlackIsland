using System;
using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.NadineActive4)]
	public class NadineActive4 : LocalSkillScript
	{
		public override void Start()
		{
			PlayAnimation(Self, TriggerSkill04);
		}


		public override void Play(int actionNo, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel) { }


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return (GameDB.characterState
							.GetData(Singleton<NadineSkillActive4Data>.inst.BuffState2[skillData.level]).maxStack + 1)
						.ToString();
				case 1:
					return Singleton<NadineSkillActive4Data>.inst.DamageByLevel[skillData.level].ToString();
				case 2:
				{
					CharacterStateData data =
						GameDB.characterState.GetData(Singleton<NadineSkillActive4Data>.inst.PassiveBuffState);
					return (Singleton<NadineSkillActive4Data>.inst.DamagePerPassiveStackCount *
					        GetStateStackByGroup(Self, data.group, Self.ObjectId)).ToString();
				}
				case 3:
				{
					int num = (int) Math.Abs(GameDB.characterState
						.GetData(Singleton<NadineSkillActive4Data>.inst.DebuffState).statValue1);
					return string.Format("{0}%", num);
				}
				case 4:
					return ((int) (Singleton<NadineSkillActive4Data>.inst.DamageAp * SelfStat.AttackPower)).ToString();
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

			return "ToolTipType/CoolTime";
		}


		public override string GetNextLevelTooltipValue(SkillData skillData, int level, int index)
		{
			if (index == 0)
			{
				return Singleton<NadineSkillActive4Data>.inst.DamageByLevel[level].ToString();
			}

			if (index != 1)
			{
				return "";
			}

			return skillData.cooldown.ToString();
		}
	}
}