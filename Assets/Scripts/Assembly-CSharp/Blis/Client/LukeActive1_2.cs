using System.Collections.Generic;
using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.LukeActive1_2)]
	public class LukeActive1_2 : LocalSkillScript
	{
		private const string Temp_M_05 = "Temp_M_05";


		private const string Luke_Skill_Mob = "Luke_Skill_Mob";


		private static readonly int bSkill01_t = Animator.StringToHash("bSkill01_t");


		private readonly Dictionary<int, string> Active1_2_Sfx = new Dictionary<int, string>();


		private readonly Dictionary<int, string> Active1_3_Sfx = new Dictionary<int, string>();


		private readonly Dictionary<int, string> Effect_Skill = new Dictionary<int, string>();


		private readonly Dictionary<int, string> Effect_Skill1_3 = new Dictionary<int, string>();


		private CollisionCircle3D skillCollision;


		public LukeActive1_2()
		{
			Effect_Skill.Add(0, "FX_BI_Luke_Skill01_2_Mop");
			Effect_Skill.Add(1, "FX_BI_Luke_Skill01_2_Mop_Evo");
			Effect_Skill1_3.Add(0, "FX_BI_Luke_Skill01_3_Attack");
			Effect_Skill1_3.Add(1, "FX_BI_Luke_Skill01_3_Attack_Evo");
		}


		public override void Start()
		{
			SetAnimation(Self, BooleanSkill01, true);
			SetAnimation(Self, bSkill01_t, true);
			PlayAnimation(Self, TriggerSkill01_2);
			PlayEffectChildManual(Self, "Luke_Skill_Mob", Effect_Skill[evolutionLevel], "Temp_M_05");
		}


		public override void Play(int actionNo, LocalObject target, Vector3? targetPosition)
		{
			if (actionNo == 1)
			{
				PlayAnimation(Self, TriggerSkill01_3);
				PlayEffectPoint(Self, Effect_Skill1_3[evolutionLevel]);
				StopEffectChildManual(Self, "Luke_Skill_Mob", true);
			}

			if (actionNo == 2)
			{
				SetAnimation(Self, BooleanSkill01, false);
				SetAnimation(Self, bSkill01_t, false);
				StopEffectChildManual(Self, "Luke_Skill_Mob", true);
			}
		}


		public override void Finish(bool cancel)
		{
			SetAnimation(Self, BooleanSkill01, false);
			SetAnimation(Self, bSkill01_t, false);
			StopEffectChildManual(Self, "Luke_Skill_Mob", true);
		}


		public override UseSkillErrorCode IsEnableSkillSlot()
		{
			if (skillCollision == null)
			{
				skillCollision = new CollisionCircle3D(Self.GetPosition(), SkillRange);
			}
			else
			{
				skillCollision.UpdatePosition(Self.GetPosition());
			}

			foreach (LocalCharacter localCharacter in GetCharacterWithinRange(skillCollision, false, true))
			{
				if (localCharacter != null &&
				    localCharacter.GetCharacterStateValueByGroup(Singleton<LukeSkillActive1_1Data>.inst.DebuffGroupCode,
					    Self.ObjectId) != null)
				{
					UnlockSkillSlot(SkillSlotSet.Active1_2);
					return UseSkillErrorCode.None;
				}
			}

			LockSkillSlot(SkillSlotSet.Active1_2);
			return UseSkillErrorCode.NotAvailableNow;
		}


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return Singleton<LukeSkillActive1_1Data>.inst.BaseDamage[skillData.level].ToString();
				case 1:
					return ((int) (Singleton<LukeSkillActive1_1Data>.inst.DamageApCoef * SelfStat.AttackPower))
						.ToString();
				case 2:
					return GameDB.characterState.GetData(Singleton<LukeSkillActive1_1Data>.inst.BuffCode).duration
						.ToString();
				case 3:
					return Singleton<LukeSkillActive1_2Data>.inst.BaseDamage[skillData.level].ToString();
				case 4:
					return ((int) (Singleton<LukeSkillActive1_2Data>.inst.DamageApCoef * SelfStat.AttackPower))
						.ToString();
				case 5:
				{
					float evolutionBuffRatio = Singleton<LukeSkillActive1_2Data>.inst.EvolutionBuffRatio;
					return string.Format("{0}%", evolutionBuffRatio * 100f);
				}
				default:
					return "";
			}
		}


		public override string GetNextLevelTooltipParam(int index)
		{
			if (index == 0)
			{
				return "ToolTipType/BulletDamage";
			}

			if (index != 1)
			{
				return "";
			}

			return "ToolTipType/DashDamage";
		}


		public override string GetNextLevelTooltipValue(SkillData skillData, int level, int index)
		{
			if (index == 0)
			{
				return Singleton<LukeSkillActive1_1Data>.inst.BaseDamage[skillData.level].ToString();
			}

			if (index != 1)
			{
				return "";
			}

			return Singleton<LukeSkillActive1_2Data>.inst.BaseDamage[skillData.level].ToString();
		}
	}
}