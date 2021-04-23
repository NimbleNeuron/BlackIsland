using System;
using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.ZahirActive2)]
	public class ZahirActive2 : LocalSkillScript
	{
		protected static readonly int TriggerFire = Animator.StringToHash("tFire");

		public override void Start()
		{
			SetAnimation(Self, BooleanSkill02, true);
			PlayWeaponAnimation(Self, WeaponMountType.Special_2, TriggerFire);
			PlayAnimation(Self, TriggerSkill02);
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel)
		{
			SetAnimation(Self, BooleanSkill02, false);
		}


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return Singleton<ZahirSkillActive2Data>.inst.DamageByLevel[skillData.level].ToString();
				case 1:
					return ((int) (Singleton<ZahirSkillActive2Data>.inst.SkillApCoef * SelfStat.AttackPower))
						.ToString();
				case 2:
					return Singleton<ZahirSkillActive2Data>.inst.AddStackCount.ToString();
				case 3:
					return GameDB.characterState.GetData(Singleton<ZahirSkillActive2Data>.inst.BuffState).maxStack
						.ToString();
				case 4:
					return GameDB.characterState.GetData(Singleton<ZahirSkillActive2Data>.inst.BuffState).duration
						.ToString();
				case 5:
					return GameDB.characterState.GetData(Singleton<ZahirSkillActive2Data>.inst.DebuffState).duration
						.ToString();
				case 6:
				{
					float num = Math.Abs(GameDB.characterState
						.GetData(Singleton<ZahirSkillActive2Data>.inst.DebuffState).statValue1);
					return string.Format("{0}%", num);
				}
				case 7:
					return Math.Abs(Singleton<ZahirSkillActive2Data>.inst.SkillCooldownReduce).ToString();
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

			return "";
		}


		public override string GetNextLevelTooltipValue(SkillData skillData, int level, int index)
		{
			if (index == 0)
			{
				return Singleton<ZahirSkillActive2Data>.inst.DamageByLevel[level].ToString();
			}

			return "";
		}
	}
}