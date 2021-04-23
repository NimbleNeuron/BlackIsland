using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.CrossBowActive)]
	public class CrossBowActive : LocalSkillScript
	{
		private const string FX_BI_WSkill_CrossBow_Range = "FX_BI_WSkill_CrossBow_Range";


		protected static readonly int TriggerSkill = Animator.StringToHash("tCrossBow_Skill");


		protected static readonly int BooleanSkill = Animator.StringToHash("bCrossBow_Skill");


		private GameObject RangeEffect;


		public override void Start()
		{
			SetAnimation(Self, BooleanSkill, true);
			PlayAnimation(Self, TriggerSkill);
			RangeEffect = PlayEffectChild(Self, "FX_BI_WSkill_CrossBow_Range");
		}


		public override void Play(int actionNo, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel)
		{
			SetAnimation(Self, BooleanSkill, false);
			if (RangeEffect != null)
			{
				RangeEffect.transform.parent = null;
			}
		}


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			if (index == 0)
			{
				return ((int) (Singleton<CrossBowSkillActiveData>.inst.SkillApCoef[skillData.level] *
				               SelfStat.AttackPower)).ToString();
			}

			return "";
		}


		public override string GetNextLevelTooltipParam(int index)
		{
			if (index == 0)
			{
				return "ToolTipType/Damage";
			}

			return "";
		}


		public override string GetNextLevelTooltipValue(SkillData skillData, int level, int index)
		{
			if (index == 0)
			{
				return ((int) (Singleton<CrossBowSkillActiveData>.inst.SkillApCoef[level] * SelfStat.AttackPower))
					.ToString();
			}

			return "";
		}
	}
}