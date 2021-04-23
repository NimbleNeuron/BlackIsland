using System;
using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.RozziActive1)]
	public class LocalRozziActive1 : LocalSkillScript
	{
		public override void Start()
		{
			PlayAnimation(Self, TriggerReloadCancel);
			PlayAnimation(Self, TriggerSkill01);
			PlayEffectPoint(Self, "FX_BI_Rozzi_Skill01_Gun", "ShotPoint_Skill01");
			PlaySoundPoint(Self, "Rozzi_Skill01_Fire", 15);
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel)
		{
			ResetAnimatorTrigger(Self, TriggerReloadCancel);
		}


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return Singleton<RozziSkillActive1Data>.inst.DamageByLevel[skillData.level].ToString();
				case 1:
					return ((int) (Singleton<RozziSkillActive1Data>.inst.DamageApCoef * SelfStat.AttackPower))
						.ToString();
				case 2:
					return Math.Abs(Singleton<RozziSkillActive1Data>.inst.Aactive1CooldownReduce).ToString();
				case 3:
					return GameDB.characterState.GetData(Singleton<RozziSkillActive1Data>.inst.Active1MoveStateCode)
						.duration.ToString();
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

			return "ToolTipType/Cost";
		}


		public override string GetNextLevelTooltipValue(SkillData skillData, int level, int index)
		{
			if (index == 0)
			{
				return Singleton<RozziSkillActive1Data>.inst.DamageByLevel[level].ToString();
			}

			if (index != 1)
			{
				return "";
			}

			return skillData.cost.ToString();
		}
	}
}