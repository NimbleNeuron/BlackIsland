using System;
using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.EmmaActive2)]
	public class EmmaActive2 : LocalSkillScript
	{
		public override void Start()
		{
			PlayAnimation(Self, TriggerSkill02);
			SetAnimation(Self, BooleanMotionWait, true);
			PlaySoundPoint(Self, "Emma_Skill02_Fire", 15);
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition)
		{
			if (action == 1)
			{
				PlayEffectChild(target, "FX_BI_Emma_Skill02_Hat_Drop");
				PlaySoundPoint(Self, "Emma_Skill02_Hat", 15);
			}
		}


		public override void Finish(bool cancel) { }


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return Singleton<EmmaSkillActive2Data>.inst.DamageBySkillLevel[skillData.level].ToString();
				case 1:
					return ((int) (Singleton<EmmaSkillActive2Data>.inst.DamageApCoef * SelfStat.AttackPower))
						.ToString();
				case 2:
					return GameDB.character.GetSummonData(Singleton<EmmaSkillActive2Data>.inst.FireworkHatSummonCode)
						.duration.ToString();
				case 3:
					return ((int) Math.Abs(Singleton<EmmaSkillActive2Data>.inst.CooldownReduce)).ToString();
				case 4:
					return GameDB.projectile
						.GetData(Singleton<EmmaSkillActive2Data>.inst.FireworkHatExplosionAreaProjectileCode)
						.lifeTimeAfterArrival.ToString();
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
					return Singleton<EmmaSkillActive2Data>.inst.DamageBySkillLevel[level].ToString();
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