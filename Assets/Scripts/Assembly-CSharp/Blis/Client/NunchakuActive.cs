using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.NunchakuActive)]
	public class NunchakuActive : LocalSkillScript
	{
		private const string Nunchaku_WeaponSkill01 = "Nunchaku_WeaponSkill01";


		private const string Nunchaku_WeaponSkill02 = "Nunchaku_WeaponSkill02";


		private const string FX_BI_WSkill_Nunchaku_01 = "FX_BI_WSkill_Nunchaku_01";


		private const string WSkill_Nunchaku_01 = "WSkill_Nunchaku_01";


		private const string Fx_Center = "Fx_Center";


		protected static readonly int TriggerSkill_01 = Animator.StringToHash("tNunchaku_skill_start");


		protected static readonly int TriggerSkill_02 = Animator.StringToHash("tNunchaku_skill_loop");


		protected static readonly int TriggerSkill_03 = Animator.StringToHash("tNunchaku_skill_end");


		protected static readonly int BooleanSkill = Animator.StringToHash("bTwoHandSword_Skill");


		public override void Start()
		{
			PlayAnimation(Self, TriggerSkill_01);
			PlayEffectChildManual(Self, "WSkill_Nunchaku_01", "FX_BI_WSkill_Nunchaku_01", "Fx_Center");
			PlaySoundChildManual(Self, "Nunchaku_WeaponSkill01", 15);
		}


		public override void Play(int actionNo, LocalObject target, Vector3? targetPosition)
		{
			if (actionNo == 1)
			{
				PlayAnimation(Self, TriggerSkill_03);
				PlaySoundPoint(Self, "Nunchaku_WeaponSkill02", 15);
			}
		}


		public override void Finish(bool cancel)
		{
			GameObject gameObject = PlayEffectPoint(Self, "FX_BI_WSkill_Nunchaku_02");
			if (gameObject != null)
			{
				float num = (Time.time - ConcentrationStartTime) / data.ConcentrationTime;
				if (1f < num)
				{
					num = 1f;
				}

				float num2 = Mathf.Lerp(Singleton<NunchakuSkillActiveData>.inst.MinSkillRange,
					Singleton<NunchakuSkillActiveData>.inst.MaxSkillRange, num);
				gameObject.transform.localScale =
					new Vector3(1f, 1f, num2 / Singleton<NunchakuSkillActiveData>.inst.MaxSkillRange);
				gameObject.transform.forward = SelfForward;
			}

			StopEffectChildManual(Self, "WSkill_Nunchaku_01", true);
			StopSoundChildManual(Self, "Nunchaku_WeaponSkill01");
		}


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return Singleton<NunchakuSkillActiveData>.inst.MinDamageByLevel[skillData.level].ToString();
				case 1:
					return Mathf.RoundToInt(Singleton<NunchakuSkillActiveData>.inst.MinApCoef * SelfStat.AttackPower)
						.ToString();
				case 2:
					return Singleton<NunchakuSkillActiveData>.inst.MaxDamageByLevel[skillData.level].ToString();
				case 3:
					return Mathf.RoundToInt(Singleton<NunchakuSkillActiveData>.inst.MaxApCoef * SelfStat.AttackPower)
						.ToString();
				case 4:
					return Singleton<NunchakuSkillActiveData>.inst.BonusEffectChargingTime.ToString();
				case 5:
					return Singleton<NunchakuSkillActiveData>.inst.StunDuration.ToString();
				default:
					return "";
			}
		}


		public override string GetNextLevelTooltipParam(int index)
		{
			if (index == 0)
			{
				return "ToolTipType/MinDamage";
			}

			if (index != 1)
			{
				return "";
			}

			return "ToolTipType/MaxDamage";
		}


		public override string GetNextLevelTooltipValue(SkillData skillData, int level, int index)
		{
			if (index == 0)
			{
				return Singleton<NunchakuSkillActiveData>.inst.MinDamageByLevel[level].ToString();
			}

			if (index != 1)
			{
				return "";
			}

			return Singleton<NunchakuSkillActiveData>.inst.MaxDamageByLevel[level].ToString();
		}


		public override void StartConcentration()
		{
			base.StartConcentration();
			if (SingletonMonoBehaviour<PlayerController>.inst.IsMe(Self.ObjectId))
			{
				StartPlayerConcentrating(SkillSlotSet.WeaponSkill, data.ConcentrationTime,
					Singleton<NunchakuSkillActiveData>.inst.MinSkillRange,
					Singleton<NunchakuSkillActiveData>.inst.MaxSkillRange, 0f, 0f, false, true, SelfForward);
			}
		}
	}
}