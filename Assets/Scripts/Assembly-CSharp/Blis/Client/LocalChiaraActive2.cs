using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.ChiaraActive2)]
	public class LocalChiaraActive2 : LocalSkillScript
	{
		private const string Chiara_Skill03_Barrier = "Chiara_Skill03_Barrier";


		private const string Chiara_Skill03_Barrier_name = "FX_BI_Chiara_Skill03_Barrier";


		private const string Setpoint = "Root";


		public override void Start()
		{
			SetAnimation(Self, BooleanSkill03, false);
			PlayEffectChildManual(Self, "Chiara_Skill03_Barrier", "FX_BI_Chiara_Skill03_Barrier", "Root");
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel) { }


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return GameDB.characterState.GetData(Singleton<ChiaraSkillData>.inst.A2ShieldState).duration
						.ToString();
				case 1:
					return (GameDB.characterState.GetData(Singleton<ChiaraSkillData>.inst.A2ShieldState).duration / 2f)
						.ToString();
				case 2:
					return Singleton<ChiaraSkillData>.inst.A2BaseDamage[skillData.level].ToString();
				case 3:
					return ((int) (Singleton<ChiaraSkillData>.inst.A2ApDamage * SelfStat.AttackPower)).ToString();
				case 4:
					return Singleton<ChiaraSkillData>.inst.A2BaseShield[skillData.level].ToString();
				case 5:
					return ((int) (Singleton<ChiaraSkillData>.inst.A2ApShield * SelfStat.AttackPower)).ToString();
				default:
					return "";
			}
		}


		public override string GetNextLevelTooltipParam(int index)
		{
			switch (index)
			{
				case 0:
					return "ToolTipType/Shield";
				case 1:
					return "ToolTipType/Damage";
				case 2:
					return "ToolTipType/CoolTime";
				case 3:
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
					return Singleton<ChiaraSkillData>.inst.A2BaseShield[level].ToString();
				case 1:
					return Singleton<ChiaraSkillData>.inst.A2BaseDamage[level].ToString();
				case 2:
					return skillData.cooldown.ToString();
				case 3:
					return skillData.cost.ToString();
				default:
					return "";
			}
		}
	}
}