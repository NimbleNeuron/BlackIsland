using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.IsolActive1)]
	public class LocalIsolActive1 : LocalSkillScript
	{
		public override void Start()
		{
			PlayAnimation(Self, TriggerReloadCancel);
			PlayAnimation(Self, TriggerSkill01);
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel)
		{
			ResetAnimatorTrigger(Self, TriggerReloadCancel);
		}


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return Singleton<IsolSkillActive1Data>.inst.BombTimerOnGround.ToString();
				case 1:
					return GameDB.characterState.GetData(Singleton<IsolSkillActive1Data>.inst.AttachState).duration
						.ToString();
				case 2:
					return Singleton<IsolSkillActive1Data>.inst.DurationDecreasePerHit.ToString();
				case 3:
					return Singleton<IsolSkillActive1Data>.inst.AdditionalDamagePerHit[skillData.level].ToString() ??
					       "";
				case 4:
					return Singleton<IsolSkillActive1Data>.inst.BaseDamage[skillData.level].ToString();
				case 5:
					return ((int) (Singleton<IsolSkillActive1Data>.inst.BaseSkillApCoef * SelfStat.AttackPower))
						.ToString();
				case 6:
					return GameDB.characterState.GetData(Singleton<IsolSkillActive1Data>.inst.DebuffState).duration
						.ToString();
				case 7:
					return ((int) (Singleton<IsolSkillActive1Data>.inst.AdditionalSkillApCoefPerHit *
					               SelfStat.AttackPower)).ToString();
				case 8:
					return Singleton<IsolSkillActive1Data>.inst.DebuffStateDurationIncreasePerStack.ToString();
				case 9:
					return ((int) Singleton<IsolSkillActive1Data>.inst.DebuffStateMaxDuration).ToString();
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
					return Singleton<IsolSkillActive1Data>.inst.BaseDamage[level].ToString();
				case 1:
					return Singleton<IsolSkillActive1Data>.inst.AdditionalDamagePerHit[level].ToString();
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