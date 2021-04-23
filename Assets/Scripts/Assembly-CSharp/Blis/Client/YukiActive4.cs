using System;
using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.YukiActive4)]
	public class YukiActive4 : LocalSkillScript
	{
		private const string YukiSkill04Range = "YukiSkill04Range";


		private const string YukiSkill04Attack = "YukiSkill04Attack";

		public override void Start()
		{
			SetAnimation(Self, BooleanSkill04, true);
			PlayAnimation(Self, TriggerSkill04);
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition)
		{
			if (action == 1)
			{
				SetAnimation(Self, BooleanSkill04, false);
				PlayAnimation(Self, TriggerSkill04_2);
			}
		}


		public override void Finish(bool cancel)
		{
			SetNoParentEffectByTag(Self, "YukiSkill04Range");
			SetNoParentEffectByTag(Self, "YukiSkill04Attack");
		}


		public override void StartConcentration()
		{
			base.StartConcentration();
			if (SingletonMonoBehaviour<PlayerController>.inst.IsMe(Self.ObjectId))
			{
				float minRange = 0f;
				float maxRange = 0f;
				GetSkillRange(ref minRange, ref maxRange);
				SkillData skillData =
					SingletonMonoBehaviour<PlayerController>.inst.PlayerSkill.GetSkillData(SkillSlotIndex.Active4);
				StartPlayerConcentrating(SkillSlotSet.Active4_1, data.ConcentrationTime, minRange, maxRange,
					skillData.angle, skillData.angle, true, true, SelfForward);
			}
		}


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return Singleton<YukiSkillActive4Data>.inst.SkillDamage[skillData.level].ToString();
				case 1:
					return ((int) (Singleton<YukiSkillActive4Data>.inst.SkillApCoef * SelfStat.AttackPower)).ToString();
				case 2:
					return GameDB.characterState
						.GetData(Singleton<YukiSkillActive4Data>.inst.DebuffState_1[skillData.level]).duration
						.ToString();
				case 3:
				{
					float num = Math.Abs(Singleton<YukiSkillActive4Data>.inst.DebuffMoveSpeedRatio);
					return string.Format("{0}%", num);
				}
				case 6:
				{
					int num2 = (int) (Singleton<YukiSkillActive4Data>.inst.HpRateByLevel[skillData.level] * 100f);
					return string.Format("{0}%", num2);
				}
			}

			return "";
		}


		public override string GetNextLevelTooltipParam(int index)
		{
			switch (index)
			{
				case 0:
					return "ToolTipType/Damage";
				case 1:
					return "ToolTipType/MaxhpDamage";
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
					return Singleton<YukiSkillActive4Data>.inst.SkillDamage[level].ToString();
				case 1:
				{
					int num = (int) (Singleton<YukiSkillActive4Data>.inst.HpRateByLevel[level] * 100f);
					return string.Format("{0}%", num);
				}
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