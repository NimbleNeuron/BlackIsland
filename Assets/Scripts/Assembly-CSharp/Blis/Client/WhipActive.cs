using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.WhipActive)]
	public class WhipActive : LocalSkillScript
	{
		private const string Whip_start = "Wskill_start";


		private const string Whip_end = "Wskill_end";


		protected static readonly int TriggerSkill = Animator.StringToHash("tWhip_Skill");


		protected static readonly int BooleanSkill = Animator.StringToHash("bWhip_Skill");


		protected static readonly int BooleanSkill_01 = Animator.StringToHash("bWhip_Skill_Success");


		public override void Start()
		{
			PlayAnimation(Self, TriggerSkill);
			SetAnimation(Self, BooleanSkill, true);
			PlaySoundPoint(Self, "Wskill_start", 15);
		}


		public override void Play(int actionNo, LocalObject target, Vector3? targetPosition)
		{
			if (actionNo == 1)
			{
				if (target == null)
				{
					SetAnimation(Self, BooleanSkill_01, false);
				}
				else
				{
					SetAnimation(Self, BooleanSkill_01, true);
				}

				SetAnimation(Self, BooleanSkill, false);
				PlaySoundPoint(Self, "Wskill_end", 15);
			}
		}


		public override void Finish(bool cancel) { }


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			if (index == 0)
			{
				return Singleton<WhipSkillActiveData>.inst.DamageByLevel[skillData.level].ToString();
			}

			if (index != 1)
			{
				return "";
			}

			return Mathf.RoundToInt(Singleton<WhipSkillActiveData>.inst.DamageCoef * SelfStat.AttackPower).ToString();
		}


		public override string GetNextLevelTooltipParam(int index)
		{
			if (index == 0)
			{
				return "ToolTipType/Damage";
			}

			if (index != 1)
			{
				return "";
			}

			return "ToolTipType/CoolTime";
		}


		public override string GetNextLevelTooltipValue(SkillData skillData, int level, int index)
		{
			if (index == 0)
			{
				return Singleton<WhipSkillActiveData>.inst.DamageByLevel[level].ToString();
			}

			if (index != 1)
			{
				return "";
			}

			return skillData.cooldown.ToString();
		}
	}
}