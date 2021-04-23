using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.HyejinActive1)]
	public class LocalHyejinActive1 : LocalSkillScript
	{
		public override void Start()
		{
			PlayAnimation(Self, TriggerSkill01);
			PlaySoundPoint(Self, "Hyejin_Skill01_Fire", 15);
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel) { }


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return Singleton<HyejinSkillData>.inst.A1BaseDamage[skillData.level].ToString();
				case 1:
					return ((int) (Singleton<HyejinSkillData>.inst.A1ApDamage * SelfStat.AttackPower)).ToString();
				case 2:
					return GameDB.characterState.GetData(Singleton<HyejinSkillData>.inst.A1HitDebuff).duration
						.ToString();
				case 3:
					return Mathf.Abs(GameDB.characterState.GetData(Singleton<HyejinSkillData>.inst.A1HitDebuff)
						.statValue1) + "%";
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
					return Singleton<HyejinSkillData>.inst.A1BaseDamage[skillData.level].ToString();
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