using System;
using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.ShoichiActive4)]
	public class ShoichiActive4 : LocalSkillScript
	{
		private const string Projectile_FX_BI_Shoichi_Skill04 = "Projectile_FX_BI_Shoichi_Skill04";


		public override void Start()
		{
			PlayAnimation(Self, TriggerSkill04);
			SetAnimation(Self, BooleanSkill04, true);
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition)
		{
			if (action == 1)
			{
				Vector3 vector = Singleton<ShoichiSkillActive4Data>.inst.DaggerAngles[0] * Vector3.forward;
				Self.PlayLocalEffectPoint("Projectile_FX_BI_Shoichi_Skill04", Vector3.zero,
					GameUtil.LookRotation(vector.normalized));
				return;
			}

			if (action == 2)
			{
				Vector3 vector2 = Singleton<ShoichiSkillActive4Data>.inst.DaggerAngles[1] * Vector3.forward;
				Self.PlayLocalEffectPoint("Projectile_FX_BI_Shoichi_Skill04", Vector3.zero,
					GameUtil.LookRotation(vector2.normalized));
				return;
			}

			if (action == 3)
			{
				Vector3 vector3 = Singleton<ShoichiSkillActive4Data>.inst.DaggerAngles[2] * Vector3.forward;
				Self.PlayLocalEffectPoint("Projectile_FX_BI_Shoichi_Skill04", Vector3.zero,
					GameUtil.LookRotation(vector3.normalized));
				return;
			}

			if (action == 4)
			{
				Vector3 vector4 = Singleton<ShoichiSkillActive4Data>.inst.DaggerAngles[3] * Vector3.forward;
				Self.PlayLocalEffectPoint("Projectile_FX_BI_Shoichi_Skill04", Vector3.zero,
					GameUtil.LookRotation(vector4.normalized));
			}
		}


		public override void Finish(bool cancel)
		{
			SetAnimation(Self, BooleanSkill04, false);
		}


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return Singleton<ShoichiSkillActive4Data>.inst.DamageByLevel[skillData.level].ToString();
				case 1:
					return ((int) (Singleton<ShoichiSkillActive4Data>.inst.SkillApCoef * SelfStat.AttackPower))
						.ToString();
				case 2:
					return GameDB.characterState.GetData(Singleton<ShoichiSkillActive4Data>.inst.DebuffStateCode)
						.duration.ToString();
				case 3:
					return Singleton<ShoichiSkillActive4Data>.inst.DaggerDamageByLevel[skillData.level].ToString();
				case 4:
					return ((int) (Singleton<ShoichiSkillActive4Data>.inst.SkillApCoef * SelfStat.AttackPower))
						.ToString();
				case 5:
				{
					int num = Math.Abs(Mathf.RoundToInt(GameDB.characterState
						.GetData(Singleton<ShoichiSkillActive4Data>.inst.DebuffStateCode).statValue1));
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
					return "ToolTipType/CoolTime";
				case 2:
					return "ToolTipType/DaggerDamage";
				default:
					return "";
			}
		}


		public override string GetNextLevelTooltipValue(SkillData skillData, int level, int index)
		{
			switch (index)
			{
				case 0:
					return Singleton<ShoichiSkillActive4Data>.inst.DamageByLevel[skillData.level].ToString();
				case 1:
					return skillData.cooldown.ToString();
				case 2:
					return Singleton<ShoichiSkillActive4Data>.inst.DaggerDamageByLevel[skillData.level].ToString();
				default:
					return "";
			}
		}
	}
}