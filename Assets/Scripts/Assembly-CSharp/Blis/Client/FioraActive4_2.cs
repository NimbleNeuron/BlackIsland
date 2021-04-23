using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.FioraActive4_2)]
	public class FioraActive4_2 : LocalSkillScript
	{
		public override void Start()
		{
			SetAnimation(Self, BooleanSkill04, false);
		}


		public override void Play(int actionNo, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel) { }


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return Singleton<FioraSkillActive4Data>.inst.ConsumeSp[skillData.level].ToString();
				case 1:
					return Singleton<FioraSkillActive4Data>.inst.SkillAttack[skillData.level].ToString();
				case 2:
					return ((int) (Singleton<FioraSkillActive4Data>.inst.SkillAttackApCoef[skillData.level] *
					               SelfStat.AttackPower)).ToString();
				default:
					return "";
			}
		}


		public override string GetNextLevelTooltipParam(int index)
		{
			switch (index)
			{
				case 0:
					return "ItemOptionCategory/NormalAttackIncrease";
				case 1:
					return "ToolTipType/SkillApCoef";
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
					return Singleton<FioraSkillActive4Data>.inst.SkillAttack[level].ToString();
				case 1:
					return ((int) (Singleton<FioraSkillActive4Data>.inst.SkillAttackApCoef[level] *
					               SelfStat.AttackPower)).ToString();
				case 2:
					return Singleton<FioraSkillActive4Data>.inst.ConsumeSp[level].ToString();
				default:
					return "";
			}
		}


		[SkillScript(SkillId.FioraActive4Passive)]
		public class FioraActive4Passive : LocalSkillScript
		{
			public override void Start() { }


			public override void Play(int actionNo, LocalObject target, Vector3? targetPosition) { }


			public override void Finish(bool cancel) { }
		}


		[SkillScript(SkillId.FioraActive4Buff)]
		public class FioraActive4Buff : LocalSkillScript
		{
			private const string fiora_Skill04_Activation = "fiora_Skill04_Activation";


			public override void Start()
			{
				PlayEffectPoint(Self, "FX_BI_Fiora_Skill04_Active", "Fx_Center");
				PlayEffectChildManual(Self, "Fiora_Skill04", "FX_BI_Fiora_Skill04");
				PlaySoundPoint(Self, "fiora_Skill04_Activation");
			}


			public override void Play(int actionNo, LocalObject target, Vector3? targetPosition) { }


			public override void Finish(bool cancel)
			{
				StopEffectChildManual(Self, "Fiora_Skill04", true);
				SetAnimation(Self, BooleanSkill04, false);
			}
		}
	}
}