using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.XiukaiActive4)]
	public class XiukaiActive4 : LocalSkillScript
	{
		private const string Skill04_Start = "FX_BI_Xiukai_Skill04_Start";


		private const string Skill04_Start_key = "Skill04_Start_key";


		private const string Skill04_Stop = "Skill04_Attack";


		private const string Wack = "Weapon_Special_Xiukai_01";


		private const string Skill04_Attack = "FX_BI_Xiukai_Skill04_Attack";


		private const string Skill04_Hit = "FX_BI_Xiukai_Skill04_Hit";


		private const string ShotPoint = "ShotPoint";


		private const string HitPoint = "Bip001";


		public override void Start()
		{
			SetAnimation(Self, BooleanSkill04, true);
			PlayAnimation(Self, TriggerSkill04);
		}


		public override void Play(int actionNo, LocalObject target, Vector3? targetPosition)
		{
			if (actionNo == 1)
			{
				if (targetPosition != null)
				{
					Self.PlayLocalEffectPoint("FX_BI_Xiukai_Skill04_Attack", "ShotPoint",
						GameUtil.LookRotation(targetPosition.Value));
					return;
				}

				Self.PlayLocalEffectPoint("FX_BI_Xiukai_Skill04_Attack", "ShotPoint");
			}
		}


		public override void Finish(bool cancel)
		{
			PlayAnimation(Self, TriggerSkill04_2);
			StopEffectByTag(Self, "Skill04_Attack");
			StopEffectChildManual(Self, "Skill04_Start_key", true);
			SetAnimation(Self, BooleanSkill04, false);
		}


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return ((int) (Singleton<XiukaiSkillActive4Data>.inst.AttackDelay *
					               Singleton<XiukaiSkillActive4Data>.inst.AttackCount)).ToString();
				case 1:
					return Singleton<XiukaiSkillActive4Data>.inst.BaseDamage[skillData.level].ToString();
				case 2:
					return ((int) (Singleton<XiukaiSkillActive4Data>.inst.SkillApCoef * SelfStat.AttackPower))
						.ToString();
				case 3:
				{
					CharacterStateData data =
						GameDB.characterState.GetData(Singleton<XiukaiSkillPassiveData>.inst.BuffState);
					return GetStateStackByGroup(Self, data.group, Self.ObjectId).ToString();
				}
				case 4:
				{
					float addStackDamage = Singleton<XiukaiSkillActive4Data>.inst.AddStackDamage;
					return addStackDamage.ToString();
				}
				case 5:
					return GameDB.characterState
						.GetData(Singleton<XiukaiSkillActive4Data>.inst.DebuffState[skillData.level]).duration
						.ToString();
				case 6:
				{
					CharacterStateData data2 =
						GameDB.characterState.GetData(
							Singleton<XiukaiSkillActive4Data>.inst.DebuffState[skillData.level]);
					return string.Format("{0}%", Mathf.Abs(data2.statValue1));
				}
				case 7:
				{
					int attackCount = Singleton<XiukaiSkillActive4Data>.inst.AttackCount;
					return attackCount.ToString();
				}
				case 8:
				{
					CharacterStateData data3 =
						GameDB.characterState.GetData(Singleton<XiukaiSkillPassiveData>.inst.BuffState);
					return (GetStateStackByGroup(Self, data3.group, Self.ObjectId) * 0.9f).ToString();
				}
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
					return "ToolTipType/DecreaseDefenseRatio";
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
					return Singleton<XiukaiSkillActive4Data>.inst.BaseDamage[level].ToString();
				case 1:
				{
					CharacterStateData data =
						GameDB.characterState.GetData(Singleton<XiukaiSkillActive4Data>.inst.DebuffState[level]);
					return string.Format("{0}%", Mathf.Abs(data.statValue1));
				}
				case 2:
					return skillData.cost.ToString();
				default:
					return "";
			}
		}
	}
}