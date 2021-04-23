using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.SisselaActive2)]
	public class LocalSisselaActive2 : LocalSisselaSkill
	{
		private const string tStart = "tStart";


		private const string tIng = "tIng";


		private const string tEnd = "tEnd";


		private static readonly int hashStart = Animator.StringToHash("tStart");


		private static readonly int hashIng = Animator.StringToHash("tIng");


		private static readonly int hashEnd = Animator.StringToHash("tEnd");


		private readonly string effectName = "FX_BI_Sissela_Skill02_Drop_Loop";


		private bool triggerEndStarted;

		private void Ref()
		{
			Reference.Use(effectName);
		}


		public override void Start()
		{
			LockSkillSlot(SkillSlotSet.Active1_1);
			LockSkillSlot(SkillSlotSet.Active3_1);
			LockSkillSlot(SkillSlotSet.Active4_1);
			SetAnimation(Self, BooleanMotionWait, true);
			SetAnimation(Self, BooleanSkill02, true);
			PlayAnimation(Self, TriggerSkill02);
			PlaySoundPoint(Self, "Sissela_Skill02_Start", 15);
			PlayWeaponAnimation(Self, WeaponMountType.Special_2, hashStart);
			if (localWilsonData != null)
			{
				localWilsonData.WilsonResourcePrefab.gameObject.SetActive(false);
			}

			triggerEndStarted = false;
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition)
		{
			base.Play(action, target, targetPosition);
			if (action == 1)
			{
				PlayEffectChildManual(Self, "Loop", "FX_BI_Sissela_Skill02_Drop_Loop", "Bip001");
				return;
			}

			if (action == 2)
			{
				StopEffectChildManual(Self, "Loop", true);
				PlayWeaponAnimation(Self, WeaponMountType.Special_2, hashEnd);
				triggerEndStarted = true;
				PlayAnimation(Self, TriggerSkill02_2);
				PlaySoundPoint(Self, "Sissela_Skill02_End", 15);
				return;
			}

			if (action == 3)
			{
				PlayEffectChild(Self, "FX_BI_Sissela_Skill02_Drop_Explosion", "Bip001");
			}
		}


		public override void Finish(bool cancel)
		{
			SetAnimation(Self, BooleanSkill02, false);
			UnlockSkillSlot(SkillSlotSet.Active1_1);
			UnlockSkillSlot(SkillSlotSet.Active3_1);
			UnlockSkillSlot(SkillSlotSet.Active4_1);
			if (localWilsonData != null)
			{
				localWilsonData.WilsonResourcePrefab.gameObject.SetActive(true);
			}

			if (!triggerEndStarted)
			{
				StopEffectChildManual(Self, "Loop", true);
				PlayWeaponAnimation(Self, WeaponMountType.Special_2, hashEnd);
			}
		}


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return GameDB.characterState.GetData(Singleton<SisselaSkillData>.inst.A2UntargetableStateCode)
						.duration.ToString();
				case 1:
					return Singleton<SisselaSkillData>.inst.A2BaseDamage[skillData.level].ToString();
				case 2:
					return ((int) (Singleton<SisselaSkillData>.inst.A2ApDamage * SelfStat.AttackPower)).ToString();
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
					return "SkillCostType/HpCost";
				default:
					return "";
			}
		}


		public override string GetNextLevelTooltipValue(SkillData skillData, int level, int index)
		{
			switch (index)
			{
				case 0:
					return Singleton<SisselaSkillData>.inst.A2BaseDamage[skillData.level].ToString();
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