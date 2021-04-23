using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.AdrianaActive4)]
	public class AdrianaActive4 : LocalSkillScript
	{
		private const string FX_BI_Adriana_Skill04_Start = "FX_BI_Adriana_Skill04_Start";


		private const string FX_BI_Adriana_Skill04_Start_key = "FX_BI_Adriana_Skill04_Start_key";


		private const string FX_BI_Adriana_Skill04_Start_point = "Fx_Hand_R";


		private const string adriana_Skill02_FireGround = "adriana_Skill02_FireGround";


		private const string adriana_Skill04_sfx = "adriana_Skill04";


		public override void Start()
		{
			LockSkillSlot(SkillSlotSet.Active1_1);
			LockSkillSlot(SkillSlotSet.Active3_1);
			LockSkillSlot(SkillSlotSet.WeaponSkill);
			PlayAnimation(Self, TriggerSkill04);
			PlayEffectChildManual(Self, "FX_BI_Adriana_Skill04_Start_key", "FX_BI_Adriana_Skill04_Start", "Fx_Hand_R");
			PlaySoundPoint(Self, "adriana_Skill04", 15);
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition)
		{
			if (action == 1)
			{
				PlaySoundPoint(Self, "adriana_Skill02_FireGround", 15);
			}
		}


		public override void Finish(bool cancel)
		{
			StopEffectChildManual(Self, "FX_BI_Adriana_Skill04_Start_key", false);
			UnlockSkillSlot(SkillSlotSet.Active1_1);
			UnlockSkillSlot(SkillSlotSet.Active3_1);
			UnlockSkillSlot(SkillSlotSet.WeaponSkill);
		}


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return Singleton<AdrianaSkillActive4Data>.inst.DamageBySkillLevel[skillData.level].ToString();
				case 1:
					return ((int) (Singleton<AdrianaSkillActive4Data>.inst.DamageApCoef * SelfStat.AttackPower))
						.ToString();
				case 2:
					return (skillData.cooldown * (1f - SelfStat.CooldownReduction)).ToString();
				case 3:
					return skillData.maxStack.ToString();
				case 4:
					return GameDB.projectile.GetData(Singleton<AdrianaSkillActive4Data>.inst.FireFlame3ProjectileCode)
						.lifeTimeAfterExplosion.ToString();
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
					return "ToolTipType/ChargingTime";
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
					return Singleton<AdrianaSkillActive4Data>.inst.DamageBySkillLevel[level].ToString();
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