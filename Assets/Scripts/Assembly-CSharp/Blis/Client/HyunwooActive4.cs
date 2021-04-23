using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.HyunwooActive4)]
	public class HyunwooActive4 : LocalSkillScript
	{
		private const string Hyunwoo_Skill04_Charge = "Hyunwoo_Skill04_Charge";


		private const string Hyunwoo_Skill04_Range = "Hyunwoo_Skill04_Range";


		private const string FX_BI_Hyunwoo_Skill04_Charge = "FX_BI_Hyunwoo_Skill04_Charge";


		private const string FX_BI_Hyunwoo_Skill04_Range = "FX_BI_Hyunwoo_Skill04_Range";


		private const string FX_BI_Hyunwoo_Skill04_Hit = "FX_BI_Hyunwoo_Skill04_Hit";


		private const string FX_BI_Hyunwoo_Skill04_Hit2 = "FX_BI_Hyunwoo_Skill04_Hit2";


		private const string FX_BI_Hyunwoo_Skill04_Hit3 = "FX_BI_Hyunwoo_Skill04_Hit3";


		private const string FX_BI_Hyunwoo_Skill04_02 = "FX_BI_Hyunwoo_Skill04_02";


		private const string hyunwoo_Skill04_Charging = "hyunwoo_Skill04_Charging";


		private const string hyunwoo_Skill04_Activation = "hyunwoo_Skill04_Activation";


		private const string hyunwoo_Skill04_Hit = "hyunwoo_Skill04_Hit";


		private const string Fx_Hand_R = "Fx_Hand_R";


		private bool isDoSkill04Attack;


		public override void Start()
		{
			isDoSkill04Attack = false;
			SetAnimation(Self, BooleanSkill04, true);
			PlayAnimation(Self, TriggerSkill04);
			PlayEffectChildManual(Self, "Hyunwoo_Skill04_Charge", "FX_BI_Hyunwoo_Skill04_Charge", "Fx_Hand_R");
			PlaySoundChildManual(Self, "hyunwoo_Skill04_Charging", 15, true);
			PlayEffectChildManual(Self, "Hyunwoo_Skill04_Range", "FX_BI_Hyunwoo_Skill04_Range");
			PlaySoundChildManual(Self, "hyunwoo_Skill04_Activation", 15);
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition)
		{
			if (action == 1)
			{
				isDoSkill04Attack = true;
				SetAnimation(Self, BooleanSkill04, false);
				PlayAnimation(Self, TriggerSkill04_2);
				StopEffectChildManual(Self, "Hyunwoo_Skill04_Charge", true);
				StopEffectChildManual(Self, "Hyunwoo_Skill04_Range", true);
			}
		}


		public override void Finish(bool cancel)
		{
			if (!isDoSkill04Attack)
			{
				PlayAnimation(Self, TriggerSkill04_3);
				SetAnimation(Self, BooleanSkill04, false);
				StopEffectChildManual(Self, "Hyunwoo_Skill04_Charge", true);
				PlayEffectPoint(Self, "FX_BI_Hyunwoo_Skill04_Hit", new Vector3(-0.375f, 1.096f, 1.74f));
				PlayEffectPoint(Self, "FX_BI_Hyunwoo_Skill04_Hit2");
				PlayEffectPoint(Self, "FX_BI_Hyunwoo_Skill04_Hit3");
				PlayEffectPoint(Self, "FX_BI_Hyunwoo_Skill04_02", new Vector3(-0.46f, 0.916f, 0f));
				PlaySoundPoint(Self, "hyunwoo_Skill04_Hit", 15);
			}

			StopSoundChildManual(Self, "hyunwoo_Skill04_Charging");
			StopSoundChildManual(Self, "hyunwoo_Skill04_Activation");
			StopEffectChildManual(Self, "Hyunwoo_Skill04_Range", true);
		}


		public override void StartConcentration()
		{
			base.StartConcentration();
			if (SingletonMonoBehaviour<PlayerController>.inst.IsMe(Self.ObjectId))
			{
				float minRange = 0f;
				float maxRange = 0f;
				GetSkillRange(ref minRange, ref maxRange);
				SkillData skillData =
					SingletonMonoBehaviour<PlayerController>.inst.PlayerSkill.GetSkillData(SkillSlotIndex.Active4);
				StartPlayerConcentrating(SkillSlotSet.Active4_1, data.ConcentrationTime, minRange, maxRange,
					skillData.angle, skillData.angle, true, false, SelfForward);
			}
		}


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return Singleton<HyunwooSkillActive4Data>.inst.MinDamageByLevel[skillData.level].ToString();
				case 1:
					return Mathf
						.RoundToInt(Singleton<HyunwooSkillActive4Data>.inst.MinSkillApCoef * SelfStat.AttackPower)
						.ToString();
				case 2:
					return Singleton<HyunwooSkillActive4Data>.inst.MaxDamageByLevel[skillData.level].ToString();
				case 3:
					return Mathf
						.RoundToInt(Singleton<HyunwooSkillActive4Data>.inst.MaxSkillApCoef * SelfStat.AttackPower)
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
					return "ToolTipType/CoolTime";
				default:
					return "";
			}
		}


		public override string GetNextLevelTooltipValue(SkillData skillData, int level, int index)
		{
			switch (index)
			{
				case 0:
					return Singleton<HyunwooSkillActive4Data>.inst.MinDamageByLevel[skillData.level].ToString();
				case 1:
					return Singleton<HyunwooSkillActive4Data>.inst.MaxDamageByLevel[skillData.level].ToString();
				case 2:
					return skillData.cooldown.ToString();
				default:
					return "";
			}
		}


		public override bool GetSkillRange(ref float minRange, ref float maxRange)
		{
			if (!SingletonMonoBehaviour<PlayerController>.inst.IsMe(Self.ObjectId))
			{
				return false;
			}

			SkillData skillData =
				SingletonMonoBehaviour<PlayerController>.inst.PlayerSkill.GetSkillData(SkillSlotIndex.Active4);
			minRange = Singleton<HyunwooSkillActive4Data>.inst.MinSkillRange;
			maxRange = skillData.range;
			return true;
		}
	}
}