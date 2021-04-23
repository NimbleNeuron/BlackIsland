using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.LiDailinActive3)]
	public class LocalLiDailinActive3 : LocalSkillScript
	{
		private const string Skill03_Stop = "Skill03";


		private static readonly int TriggerSkill03_p = Animator.StringToHash("tSkill03_p");


		public override void Start() { }


		public override void Play(int action, LocalObject target, Vector3? targetPosition)
		{
			if (action == 1)
			{
				PlayAnimation(Self, TriggerSkill03);
				return;
			}

			if (action == 2)
			{
				PlayAnimation(Self, TriggerSkill03_p);
			}
		}


		public override void Finish(bool cancel)
		{
			SetNoParentEffectByTag(Self, "Skill03");
		}


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return GameDB.characterState
						.GetData(Singleton<LiDailinSkillData>.inst.A3BaseDebuff[skillData.level]).duration.ToString();
				case 1:
					return Singleton<LiDailinSkillData>.inst.A3BaseDamage[skillData.level].ToString();
				case 2:
					return (Singleton<LiDailinSkillData>.inst.A3ApDamage * SelfStat.AttackPower).ToString();
				case 3:
					return GameDB.characterState
						.GetData(Singleton<LiDailinSkillData>.inst.A3ReinforceDebuff[skillData.level]).duration
						.ToString();
				case 4:
					return Mathf.Abs(GameDB.characterState
						       .GetData(Singleton<LiDailinSkillData>.inst.A3ReinforceDebuff[skillData.level])
						       .statValue2) +
					       "%";
				default:
					return "";
			}
		}


		public override string GetNextLevelTooltipParam(int index)
		{
			switch (index)
			{
				case 0:
					return "ToolTipType/Silence";
				case 1:
					return "ToolTipType/Damage";
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
					return GameDB.characterState.GetData(Singleton<LiDailinSkillData>.inst.A3BaseDebuff[level]).duration
						.ToString();
				case 1:
					return Singleton<LiDailinSkillData>.inst.A3BaseDamage[level].ToString();
				case 2:
					return skillData.cooldown.ToString();
				default:
					return "";
			}
		}
	}
}