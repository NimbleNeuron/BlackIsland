using System;
using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.HighAngleFireActive)]
	public class HighAngleFireActive : LocalSkillScript
	{
		protected static readonly int TriggerSkill = Animator.StringToHash("tHighAngleFire_Skill");


		protected static readonly int BooleanSkill = Animator.StringToHash("bHighAngleFire_Skill");


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
			if (index == 0)
			{
				return Singleton<HighAngleFireSkillActiveData>.inst.Duration.ToString();
			}

			if (index != 1)
			{
				return "";
			}

			float num = Math.Abs(GameDB.characterState
				.GetData(Singleton<HighAngleFireSkillActiveData>.inst.DebuffState[skillData.level]).statValue2);
			return string.Format("{0}%", num);
		}


		public override string GetNextLevelTooltipParam(int index)
		{
			if (index == 0)
			{
				return "ToolTipType/DecreaseMoveRatio";
			}

			return "";
		}


		public override string GetNextLevelTooltipValue(SkillData skillData, int level, int index)
		{
			if (index == 0)
			{
				float num = Math.Abs(GameDB.characterState
					.GetData(Singleton<HighAngleFireSkillActiveData>.inst.DebuffState[level]).statValue2);
				return string.Format("{0}%", num);
			}

			return "";
		}
	}
}