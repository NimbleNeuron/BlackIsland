using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.NadineActive3_2)]
	public class NadineActive3_2 : LocalSkillScript
	{
		public override void Start()
		{
			PlayAnimation(Self, TriggerSkill03_2);
			SetAnimation(Self, BooleanSkill03, true);
			PlayEffectChildManual(Self, "Nadine_Skill03_Move", "FX_BI_Nadine_Skill03");
		}


		public override void Play(int actionNo, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel)
		{
			PlayAnimation(Self, TriggerSkill03_3);
			SetAnimation(Self, BooleanSkill03, false);
			StopEffectChildManual(Self, "Nadine_Skill03_Move", false);
		}


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
				{
					CharacterStateData data =
						GameDB.characterState.GetData(
							Singleton<NadineSkillActive3Data>.inst.BuffState1[skillData.level]);
					return string.Format("{0}%", data.statValue1);
				}
				case 1:
					return Singleton<NadineSkillActive3Data>.inst.ExclusiveViewMaintainTime.ToString();
				case 2:
					return Singleton<NadineSkillActive3Data>.inst.MaxLineConnectionRange.ToString();
				case 3:
				{
					CharacterStateData data2 =
						GameDB.characterState.GetData(
							Singleton<NadineSkillActive3Data>.inst.BuffState1[skillData.level]);
					int num = (int) (GameDB.characterState
						                 .GetData(Singleton<NadineSkillActive3Data>.inst.BuffState2[skillData.level])
						                 .statValue1 -
					                 data2.statValue1);
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
					return "StatType/AttackSpeedRatio";
				case 1:
					return "ToolTipType/AddAttackSpeedRatio";
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
				{
					CharacterStateData data =
						GameDB.characterState.GetData(Singleton<NadineSkillActive3Data>.inst.BuffState1[level]);
					return string.Format("{0}%", data.statValue1);
				}
				case 1:
				{
					CharacterStateData data2 =
						GameDB.characterState.GetData(Singleton<NadineSkillActive3Data>.inst.BuffState1[level]);
					int num = (int) (GameDB.characterState
						                 .GetData(Singleton<NadineSkillActive3Data>.inst.BuffState2[level]).statValue1 -
					                 data2.statValue1);
					return string.Format("{0}%", num);
				}
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