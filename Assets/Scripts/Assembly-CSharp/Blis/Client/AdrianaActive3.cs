using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.AdrianaActive3)]
	public class AdrianaActive3 : LocalSkillScript
	{
		private const string adriana_Skill03_sfx = "adriana_Skill03_Start";


		private const string FX_BI_Adriana_Skill03_Loop = "FX_BI_Adriana_Skill03_Loop";


		private const string FX_BI_Adriana_Skill03_Loop_key = "FX_BI_Adriana_Skill03_Loop_key";


		private const string FX_BI_Adriana_Skill03_Loop_02 = "FX_BI_Adriana_Skill03_Loop_02";


		private const string FX_BI_Adriana_Skill03_Loop_02_key = "FX_BI_Adriana_Skill03_Loop_02_key";


		private const string adriana_Skill02_FireGround = "adriana_Skill02_FireGround";

		public override void Start()
		{
			LockSkillSlot(SkillSlotSet.Active2_1);
			LockSkillSlot(SkillSlotSet.Active4_1);
			PlayAnimation(Self, TriggerSkill03);
			SetAnimation(Self, BooleanSkill03, true);
			PlaySoundChildManual(Self, "adriana_Skill03_Start", 15);
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition)
		{
			if (action == 1)
			{
				PlayEffectChildManual(Self, "FX_BI_Adriana_Skill03_Loop_key", "FX_BI_Adriana_Skill03_Loop");
				PlayEffectChildManual(Self, "FX_BI_Adriana_Skill03_Loop_02_key", "FX_BI_Adriana_Skill03_Loop_02");
			}

			if (action == 2)
			{
				PlaySoundPoint(Self, "adriana_Skill02_FireGround", 15);
			}
		}


		public override void Finish(bool cancel)
		{
			UnlockSkillSlot(SkillSlotSet.Active2_1);
			UnlockSkillSlot(SkillSlotSet.Active4_1);
			StopEffectChildManual(Self, "FX_BI_Adriana_Skill03_Loop_key", true);
			StopEffectChildManual(Self, "FX_BI_Adriana_Skill03_Loop_02_key", false);
			SetAnimation(Self, BooleanSkill03, false);
		}


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			if (index == 0)
			{
				return GameDB.projectile.GetData(Singleton<AdrianaSkillActive3Data>.inst.FireFlame2ProjectileCode)
					.lifeTimeAfterExplosion.ToString();
			}

			return "";
		}


		public override string GetNextLevelTooltipParam(int index)
		{
			if (index == 0)
			{
				return "ToolTipType/CoolTime";
			}

			return "";
		}


		public override string GetNextLevelTooltipValue(SkillData skillData, int level, int index)
		{
			if (index == 0)
			{
				return skillData.cooldown.ToString();
			}

			return "";
		}
	}
}