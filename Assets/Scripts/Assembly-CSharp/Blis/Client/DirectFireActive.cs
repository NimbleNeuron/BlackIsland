using System;
using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.DirectFireActive)]
	public class DirectFireActive : LocalSkillScript
	{
		protected static readonly int TriggerSkill = Animator.StringToHash("tDirectFire_Skill");


		protected static readonly int BooleanSkill = Animator.StringToHash("bDirectFire_Skill");


		public override void Start()
		{
			PlayAnimation(Self, TriggerSkill);
			SetAnimation(Self, BooleanSkill, true);
			StartCoroutine(CoroutineUtil.DelayedAction(1.1f, delegate { SetAnimation(Self, BooleanSkill, false); }));
		}


		public override void Play(int actionNo, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel) { }


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return Singleton<DirectFireSkillActiveData>.inst.Duration.ToString();
				case 1:
					return Singleton<DirectFireSkillActiveData>.inst.DamageByLevel_2[skillData.level].ToString();
				case 2:
				{
					float num = Math.Abs(GameDB.characterState
						.GetData(Singleton<DirectFireSkillActiveData>.inst.DebuffState_2[skillData.level]).statValue1);
					return string.Format("{0}%", num);
				}
				case 3:
				{
					float num2 = 100f * Singleton<DirectFireSkillActiveData>.inst.DamageReduce_2;
					return string.Format("{0}%", num2);
				}
				case 4:
					return GameDB.characterState
						.GetData(Singleton<DirectFireSkillActiveData>.inst.DebuffState_2[skillData.level]).duration
						.ToString();
				default:
					return "";
			}
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

			return "ToolTipType/DecreaseMoveRatio";
		}


		public override string GetNextLevelTooltipValue(SkillData skillData, int level, int index)
		{
			if (index == 0)
			{
				return Singleton<DirectFireSkillActiveData>.inst.DamageByLevel_2[level].ToString();
			}

			if (index != 1)
			{
				return "";
			}

			float num = Math.Abs(GameDB.characterState
				.GetData(Singleton<DirectFireSkillActiveData>.inst.DebuffState_2[level]).statValue1);
			return string.Format("{0}%", num);
		}
	}
}