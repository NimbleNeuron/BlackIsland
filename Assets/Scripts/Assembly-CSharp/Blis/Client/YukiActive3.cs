using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.YukiActive3)]
	public class YukiActive3 : LocalSkillScript
	{
		private const string FX_BI_Yuki_Skill03_Move = "FX_BI_Yuki_Skill03_Move";


		private const string FX_BI_Yuki_Skill03_AttackMove = "FX_BI_Yuki_Skill03_AttackMove";


		private const string Yuki_Skill03_Move = "Yuki_Skill03_Move";


		private const string Yuki_Skill03_AttackMove = "Yuki_Skill03_AttackMove";


		private const string Yuki_Skill03_Move_Sfx = "Yuki_Skill03_Move";


		private const string Yuki_Skill03_Move02 = "Yuki_Skill03_Move02";


		private const string FX_BI_Yuki_Skill03_Body = "FX_BI_Yuki_Skill03_Body";


		private const string Fx_Center = "Fx_Center";


		public override void Start()
		{
			PlayAnimation(Self, TriggerSkill03);
			SetAnimation(Self, BooleanSkill03, true);
			PlayEffectChildManual(Self, "Yuki_Skill03_Move", "FX_BI_Yuki_Skill03_Move");
			PlayEffectChildManual(Self, "Yuki_Skill03_Move02", "FX_BI_Yuki_Skill03_Body", "Fx_Center");
			PlaySoundPoint(Self, "Yuki_Skill03_Move");
		}


		public override void Play(int actionNo, LocalObject target, Vector3? targetPosition)
		{
			if (actionNo == 1)
			{
				PlayAnimation(Self, TriggerSkill03_2);
				PlayEffectChildManual(Self, "Yuki_Skill03_AttackMove", "FX_BI_Yuki_Skill03_AttackMove");
				StopEffectChildManual(Self, "Yuki_Skill03_Move", false);
				StopEffectChildManual(Self, "Yuki_Skill03_Move02", true);
			}
		}


		public override void Finish(bool cancel)
		{
			SetAnimation(Self, BooleanSkill03, false);
			StopEffectChildManual(Self, "Yuki_Skill03_Move", false);
			StopEffectChildManual(Self, "Yuki_Skill03_Move02", true);
			StopEffectChildManual(Self, "Yuki_Skill03_AttackMove", false);
		}


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return Singleton<YukiSkillActive3Data>.inst.SkillDamage[skillData.level].ToString();
				case 1:
					return ((int) (Singleton<YukiSkillActive3Data>.inst.SkillApCoef * SelfStat.AttackPower)).ToString();
				case 2:
					return Singleton<YukiSkillActive3Data>.inst.AfterHitDistance.ToString();
				case 3:
					return GameDB.characterState.GetData(Singleton<YukiSkillActive3Data>.inst.DebuffState).duration
						.ToString();
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
					return Singleton<YukiSkillActive3Data>.inst.SkillDamage[level].ToString();
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