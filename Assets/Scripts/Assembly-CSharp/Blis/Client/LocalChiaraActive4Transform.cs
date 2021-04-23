using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.ChiaraActive4Transform)]
	public class LocalChiaraActive4Transform : LocalSkillScript
	{
		private const string Skill04_Start_sfx = "Chiara_Skill04_Start";

		public override void Start()
		{
			SetAnimation(Self, BooleanSkill03, false);
			SetAnimation(Self, BooleanMotionWait, true);
			SetAnimation(Self, BooleanSkill04_02, true);
			ActiveWeaponObject(Self, WeaponMountType.Special_2, true);
			PlayAnimation(Self, TriggerSkill04);
			SetAnimation(Self, BooleanSkill04, true);
			PlaySoundPoint(Self, "Chiara_Skill04_Start", 15);
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel)
		{
			ActiveWeaponObject(Self, WeaponMountType.Special_1, false);
		}


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return GameDB.characterState.GetData(Singleton<ChiaraSkillData>.inst.A4TransformStateCode).duration
						.ToString();
				case 1:
					return Singleton<ChiaraSkillData>.inst.A4TransformMaxHpBonus[skillData.level].ToString();
				case 2:
					return Singleton<ChiaraSkillData>.inst.A4TransformAroundBaseDamage[skillData.level].ToString();
				case 3:
					return ((int) (Singleton<ChiaraSkillData>.inst.A4TransformAroundApDamage * SelfStat.AttackPower))
						.ToString();
				case 4:
					return Singleton<ChiaraSkillData>.inst.A4TransformAroundLifeStealRate[skillData.level] + "%";
				case 5:
					return GameDB.characterState
						.GetData(Singleton<ChiaraSkillData>.inst.PassiveDebuffStateCode[skillData.level]).maxStack
						.ToString();
				case 6:
					return Singleton<ChiaraSkillData>.inst.A4JudgmentBaseDamage[skillData.level].ToString();
				case 7:
					return ((int) (Singleton<ChiaraSkillData>.inst.A4JudgmentApDamage * SelfStat.AttackPower))
						.ToString();
				case 8:
				{
					int num = Mathf.Abs((int) (Singleton<ChiaraSkillData>.inst.A4KillCooldownModify * 100f));
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
					return "ToolTipType/DotDamage";
				case 1:
					return "ToolTipType/JudgmentDamage";
				case 2:
					return "ToolTipType/MaxHpUp";
				case 3:
					return "ToolTipType/CoolTime";
				case 4:
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
					return Singleton<ChiaraSkillData>.inst.A4TransformAroundBaseDamage[level].ToString();
				case 1:
					return Singleton<ChiaraSkillData>.inst.A4JudgmentBaseDamage[level].ToString();
				case 2:
					return Singleton<ChiaraSkillData>.inst.A4TransformMaxHpBonus[level].ToString();
				case 3:
					return skillData.cooldown.ToString();
				case 4:
					return skillData.cost.ToString();
				default:
					return "";
			}
		}
	}
}