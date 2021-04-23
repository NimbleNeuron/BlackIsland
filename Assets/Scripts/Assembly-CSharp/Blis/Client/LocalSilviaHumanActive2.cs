using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.SilviaHumanActive2)]
	public class LocalSilviaHumanActive2 : LocalSkillScript
	{
		public override void Start()
		{
			PlayAnimation(Self, TriggerSkill02);
			PlaySoundPoint(Self, "Silvia_Skill02_Fire", 15);
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel) { }


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return Singleton<SilviaSkillHumanData>.inst.A2BaseDamage[skillData.level].ToString();
				case 1:
					return ((int) (Singleton<SilviaSkillHumanData>.inst.A2ApDamage * SelfStat.AttackPower)).ToString();
				case 2:
					return GameDB.characterState
						.GetData(Singleton<SilviaSkillHumanData>.inst.A2BaseDebuffCodes[skillData.level]).duration
						.ToString();
				case 3:
					return Mathf.Abs(GameDB.characterState
						       .GetData(Singleton<SilviaSkillHumanData>.inst.A2BaseDebuffCodes[skillData.level])
						       .statValue1) +
					       "%";
				case 4:
					return Singleton<SilviaSkillHumanData>.inst.A2NotLaunchProjectileAddTime.ToString();
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
					return "ToolTipType/DecreaseMoveRatio";
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
					return Singleton<SilviaSkillHumanData>.inst.A2BaseDamage[level].ToString();
				case 1:
					return Mathf.Abs(GameDB.characterState
						.GetData(Singleton<SilviaSkillHumanData>.inst.A2BaseDebuffCodes[level]).statValue1) + "%";
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