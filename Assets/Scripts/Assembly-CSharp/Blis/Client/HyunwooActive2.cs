using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.HyunwooActive2)]
	public class HyunwooActive2 : LocalSkillScript
	{
		public override void Start()
		{
			PlayAnimation(Self, TriggerSkill02);
			PlaySoundPoint(Self, "hyunwoo_Skill02_Activation");
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel) { }


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return GameDB.characterState
						.GetData(Singleton<HyunwooSkillActive2Data>.inst.BuffState_2[skillData.level]).statValue1
						.ToString();
				case 1:
				{
					CharacterStateData data =
						GameDB.characterState.GetData(
							Singleton<HyunwooSkillActive2Data>.inst.BuffState_2[skillData.level]);
					return Mathf.RoundToInt(data.coefStatValue1 * SelfStat.GetValue(data.coefStatType1)).ToString();
				}
				case 2:
					return GameDB.characterState.GetData(Singleton<HyunwooSkillActive2Data>.inst.BuffState).duration
						.ToString();
				case 3:
					return GameDB.characterState
						.GetData(Singleton<HyunwooSkillActive2Data>.inst.BuffState_2[skillData.level]).duration
						.ToString();
				default:
					return "";
			}
		}


		public override string GetNextLevelTooltipParam(int index)
		{
			if (index == 0)
			{
				return "StatType/Defense";
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
				return GameDB.characterState
					.GetData(Singleton<HyunwooSkillActive2Data>.inst.BuffState_2[skillData.level]).statValue1
					.ToString();
			}

			if (index != 1)
			{
				return "";
			}

			return skillData.cooldown.ToString();
		}
	}
}