using System.Collections.Generic;
using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.HartActive3_3)]
	public class HartActive3_3 : LocalSkillScript
	{
		private const string Hart_Skill03_3 = "Hart_Skill03_3";


		private const string FX_BI_Hart_Skill03_B = "FX_BI_Hart_Skill03_B";


		private const string FX_BI_Hart_Skill03_AttackRange = "FX_BI_Hart_Skill03_AttackRange";


		private readonly Dictionary<int, string> Skill03Move_SFX = new Dictionary<int, string>();


		private readonly Dictionary<int, string> Skill03Shot_SFX = new Dictionary<int, string>();


		private int stack;


		public HartActive3_3()
		{
			Skill03Move_SFX.Add(0, "hart_Skill03_Move");
			Skill03Move_SFX.Add(1, "hart_Skill03_Evo_Move");
			Skill03Shot_SFX.Add(0, "hart_Skill03_Attack");
			Skill03Shot_SFX.Add(1, "hart_Skill03_Attack_S");
		}


		public override void Start()
		{
			Self.IfTypeOf<LocalCharacter>(delegate(LocalCharacter character)
			{
				stack = character.GetStateStackByGroup(Singleton<HartSkillActive2Data>.inst.Active2BuffGroup,
					Self.ObjectId);
			});
			PlayAnimation(Self, TriggerSkill03_3);
			SetAnimation(Self, BooleanSkill03, true);
			PlayEffectChildManual(Self, "Hart_Skill03_3", "FX_BI_Hart_Skill03_B");
			PlaySoundPoint(Self, Skill03Move_SFX[stack]);
		}


		public override void Play(int actionNo, LocalObject target, Vector3? targetPosition)
		{
			if (actionNo == 1)
			{
				PlayEffectPoint(Self, "FX_BI_Hart_Skill03_AttackRange");
				PlaySoundPoint(Self, Skill03Shot_SFX[stack]);
			}
		}


		public override void Finish(bool cancel)
		{
			SetAnimation(Self, BooleanSkill03, false);
			StopEffectChildManual(Self, "Hart_Skill03_3", false);
		}


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return skillData.range.ToString();
				case 1:
					return Singleton<HartSkillActive3Data>.inst.SkillAttackRange.ToString();
				case 2:
					return Singleton<HartSkillActive3Data>.inst.DamageByLevel[skillData.level].ToString();
				case 3:
					return ((int) (Singleton<HartSkillActive3Data>.inst.SkillApCoef * SelfStat.AttackPower)).ToString();
				case 4:
				{
					int num = evolutionLevel;
					if (num < 1)
					{
						num = 1;
					}

					float statValue = GameDB.characterState.GetData(Singleton<HartSkillActive3Data>.inst.BuffState[num])
						.statValue1;
					return string.Format("{0}%", statValue);
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
					return Singleton<HartSkillActive3Data>.inst.DamageByLevel[level].ToString();
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