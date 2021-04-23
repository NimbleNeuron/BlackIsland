using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.SisselaActive4)]
	public class LocalSisselaActive4 : LocalSisselaSkill
	{
		public override void Start()
		{
			PlayAnimation(Self, TriggerSkill04);
			SetAnimation(Self, BooleanSkill04, true);
			SetAnimation(Self, BooleanMotionWait, true);
			PlayEffectPoint(Self, "FX_BI_Sissela_Skill04_Start");
			PlaySoundPoint(Self, "Sissela_Skill04_Start", 15);
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition)
		{
			base.Play(action, target, targetPosition);
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
					return Singleton<SisselaSkillData>.inst.A4BaseDamage[skillData.level].ToString();
				case 1:
					return ((int) (Singleton<SisselaSkillData>.inst.A4ApDamage * SelfStat.AttackPower)).ToString();
				case 2:
					return Singleton<SisselaSkillData>.inst.A4LostHpRateDamage.ToString();
				case 3:
					return GameDB.characterState.GetData(Singleton<SisselaSkillData>.inst.A4BuffStateCode).duration
						.ToString();
				case 4:
					return ((int) (Singleton<SisselaSkillData>.inst.A4BuffIncreaseRate * 100f)).ToString();
				case 5:
					return Singleton<SisselaSkillData>.inst.A4FullDamageDistance.ToString();
				case 6:
					return (Mathf.Abs(Singleton<SisselaSkillData>.inst.A4SameAreaDamageModify) * 100f).ToString();
				case 7:
					return (Mathf.Abs(Singleton<SisselaSkillData>.inst.A4OtherAreaDamageModify) * 100f).ToString();
				case 8:
					return Singleton<SisselaSkillData>.inst.A4SelfDamageMinHp.ToString();
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
					return Singleton<SisselaSkillData>.inst.A4BaseDamage[skillData.level].ToString();
				case 1:
					return skillData.cooldown.ToString();
				case 2:
					return skillData.cost.ToString();
				default:
					return "";
			}
		}


		public override void StartConcentration()
		{
			base.StartConcentration();
		}
	}
}