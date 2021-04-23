using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.AyaActive4)]
	public class AyaActive4 : LocalSkillScript
	{
		private const string FX_BI_Aya_Skill04_Shot = "FX_BI_Aya_Skill04_Shot";


		private const string FX_BI_Aya_Skill04_Range = "FX_BI_Aya_Skill04_Range";


		private const string aya_Skill04_Activation = "aya_Skill04_Activation";


		private const string FX_BI_Aya_Skill04_Ready = "FX_BI_Aya_Skill04_Ready";


		private const string Aya_Skill04_Ready = "Aya_Skill04_Ready";

		public override void Start()
		{
			PlayAnimation(Self, TriggerReloadCancel);
			PlayAnimation(Self, TriggerSkill04);
			PlayEffectChildManual(Self, "Aya_Skill04_Ready", "FX_BI_Aya_Skill04_Ready");
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition)
		{
			if (action == 1)
			{
				PlayAnimation(Self, TriggerSkill04_2);
				PlayEffectPoint(Self, "FX_BI_Aya_Skill04_Shot", new Vector3(0f, 0.37f, 0f));
				PlayEffectPoint(Self, "FX_BI_Aya_Skill04_Range");
				PlaySoundPoint(Self, "aya_Skill04_Activation");
				StopEffectChildManual(Self, "Aya_Skill04_Ready", false);
			}
		}


		public override void Finish(bool cancel)
		{
			ResetAnimatorTrigger(Self, TriggerReloadCancel);
		}


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return Singleton<AyaSkillActive4Data>.inst.DebuffStateMinDuration.ToString();
				case 1:
					return Singleton<AyaSkillActive4Data>.inst.DamageByMinLevel[skillData.level].ToString();
				case 2:
					return ((int) (Singleton<AyaSkillActive4Data>.inst.SkillMinApCoef * SelfStat.AttackPower))
						.ToString();
				case 3:
					return Singleton<AyaSkillActive4Data>.inst.DebuffStateMaxDuration[skillData.level].ToString();
				case 4:
					return Singleton<AyaSkillActive4Data>.inst.DamageByMaxLevel[skillData.level].ToString();
				case 5:
					return ((int) (Singleton<AyaSkillActive4Data>.inst.SkillMaxApCoef * SelfStat.AttackPower))
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
					return "ToolTipType/MinDamage";
				case 1:
					return "ToolTipType/MaxDamage";
				case 2:
					return "ToolTipType/FetterTime";
				default:
					return "";
			}
		}


		public override string GetNextLevelTooltipValue(SkillData skillData, int level, int index)
		{
			switch (index)
			{
				case 0:
					return Singleton<AyaSkillActive4Data>.inst.DamageByMinLevel[level].ToString();
				case 1:
					return Singleton<AyaSkillActive4Data>.inst.DamageByMaxLevel[level].ToString();
				case 2:
					return Singleton<AyaSkillActive4Data>.inst.DebuffStateMaxDuration[level].ToString();
				default:
					return "";
			}
		}
	}
}