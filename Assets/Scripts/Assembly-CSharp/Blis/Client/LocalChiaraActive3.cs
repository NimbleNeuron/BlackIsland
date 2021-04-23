using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.ChiaraActive3)]
	public class LocalChiaraActive3 : LocalSkillScript
	{
		private const string Chiara_Skill02_Start_Sfx = "Chiara_Skill02_Fire";


		public override void Start()
		{
			SetAnimation(Self, BooleanSkill03, false);
			PlayAnimation(Self, TriggerSkill02);
			SetAnimation(Self, BooleanSkill02, true);
			PlaySoundPoint(Self, "Chiara_Skill02_Fire", 15);
			SetAnimation(Self, BooleanMotionWait, true);
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition)
		{
			if (action == 1)
			{
				SetAnimation(Self, BooleanSkill02, false);
			}
		}


		public override void Finish(bool cancel)
		{
			SetAnimation(Self, BooleanSkill02, false);
		}


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return Singleton<ChiaraSkillData>.inst.A3CollisionBaseDamage[skillData.level].ToString();
				case 1:
					return ((int) (Singleton<ChiaraSkillData>.inst.A3CollisionApDamage * SelfStat.AttackPower))
						.ToString();
				case 2:
					return GameDB.projectile.GetHookLineData(Singleton<ChiaraSkillData>.inst.A3ProjectileCode)
						.connectionDuration.ToString();
				case 3:
					return string.Empty;
				case 4:
					return Singleton<ChiaraSkillData>.inst.A3ConnectionCompleteBaseDamage[skillData.level].ToString();
				case 5:
					return ((int) (Singleton<ChiaraSkillData>.inst.A3ConnectionCompleteApDamage * SelfStat.AttackPower))
						.ToString();
				case 6:
					return GameDB.characterState.GetData(Singleton<ChiaraSkillData>.inst.A3FetterStateCode).duration
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
					return "ToolTipType/AdditionalDamage";
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
					return Singleton<ChiaraSkillData>.inst.A3CollisionBaseDamage[level].ToString();
				case 1:
					return Singleton<ChiaraSkillData>.inst.A3ConnectionCompleteBaseDamage[level].ToString();
				case 2:
					return skillData.cooldown.ToString();
				default:
					return "";
			}
		}
	}
}