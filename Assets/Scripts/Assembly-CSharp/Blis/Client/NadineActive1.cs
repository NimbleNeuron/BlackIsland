using System;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.NadineActive1)]
	public class NadineActive1 : LocalSkillScript
	{
		private MasteryType equipWeaponMasteryType;


		public override void Start()
		{
			LocalPlayerCharacter localPlayerCharacter = Self as LocalPlayerCharacter;
			equipWeaponMasteryType = GetEquipWeaponMasteryType(localPlayerCharacter);
			CharacterMasteryData characterMasteryData =
				GameDB.mastery.GetCharacterMasteryData(localPlayerCharacter.CharacterCode);
			SetAnimatorLayerWeight(equipWeaponMasteryType, characterMasteryData, "Skill01UpperBody", 1f);
			SetAnimatorLayerWeight(equipWeaponMasteryType, characterMasteryData, "Skill01LowerBody", 1f);
			PlayEffectChildManual(Self, "Nadine_Skill01_Charge", "FX_BI_Nadine_Skill01_Charge", "ShotPoint");
			PlayEffectChildManual(Self, "Nadine_Skill01_Effect1", "FX_BI_Nadine_Skill01_Charge_End", "ShotPoint");
			PlayEffectChildManual(Self, "Nadine_Skill01_Effect2", "FX_BI_Nadine_Skill01_Charge_End2", "ShotPoint");
			PlayAnimation(Self, TriggerSkill01);
			SetAnimation(Self, BooleanSkill01, true);
			PlaySoundChildManual(Self, "nadine_Skill01_Start", 15);
		}


		public override void StartConcentration()
		{
			base.StartConcentration();
			if (SingletonMonoBehaviour<PlayerController>.inst.IsMe(Self.ObjectId))
			{
				float minRange = 0f;
				float num = 0f;
				GetSkillRange(ref minRange, ref num);
				StartPlayerConcentrating(SkillSlotSet.Active1_1, data.ConcentrationTime, minRange, num, 0f, 0f, false,
					true);
				MonoBehaviourInstance<MobaCamera>.inst.SetZoomSpeed(3f);
				MonoBehaviourInstance<MobaCamera>.inst.SetZOffset(num + 2f);
			}
		}


		public override void Play(int actionNo, LocalObject target, Vector3? targetPosition)
		{
			if (actionNo == 1)
			{
				PlayAnimation(Self, TriggerSkill01_2);
			}
		}


		public override void Finish(bool cancel)
		{
			LocalPlayerCharacter localPlayerCharacter = Self as LocalPlayerCharacter;
			CharacterMasteryData masteryData =
				GameDB.mastery.GetCharacterMasteryData(localPlayerCharacter.CharacterCode);
			Self.StartThrowingCoroutine(CoroutineUtil.DelayedAction(0.6f, delegate
				{
					SetAnimatorLayerWeight(equipWeaponMasteryType, masteryData, "Skill01LowerBody", 0f);
					SetAnimation(Self, BooleanSkill01, false);
				}),
				delegate(Exception exception)
				{
					Log.E(string.Format("[EXCEPTION][SKILL][{0}] Message:{1}, StackTrace:{2}", SkillId,
						exception.Message,
						exception.StackTrace));
				});
			StopEffectChildManual(Self, "Nadine_Skill01_Charge", true);
			StopEffectChildManual(Self, "Nadine_Skill01_Effect1", true);
			StopEffectChildManual(Self, "Nadine_Skill01_Effect2", true);
			if (SingletonMonoBehaviour<PlayerController>.inst.IsMe(Self.ObjectId))
			{
				MonoBehaviourInstance<MobaCamera>.inst.SetZoomSpeed(0.5f);
				MonoBehaviourInstance<MobaCamera>.inst.SetZOffset(0f);
			}
		}


		public override bool GetSkillRange(ref float minRange, ref float maxRange)
		{
			CharacterStateData data =
				GameDB.characterState.GetData(Singleton<NadineSkillActive1Data>.inst.PassiveBuffState);
			bool flag = Singleton<NadineSkillActive1Data>.inst.PassiveStackCount <=
			            GetStateStackByGroup(Self, data.group, Self.ObjectId);
			minRange = SelfStat.AttackRange;
			maxRange = SelfStat.AttackRange *
			           (flag
				           ? Singleton<NadineSkillActive1Data>.inst.MaxSkillRange2
				           : Singleton<NadineSkillActive1Data>.inst.MaxSkillRange);
			return true;
		}


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return skillData.ConcentrationTime.ToString();
				case 1:
					return Singleton<NadineSkillActive1Data>.inst.MaxSkillRange.ToString();
				case 2:
				{
					int num = (int) Math.Abs(GameDB.characterState
						.GetData(Singleton<NadineSkillActive1Data>.inst.BuffState).statValue1);
					return string.Format("{0}%", num);
				}
				case 3:
					return Singleton<NadineSkillActive1Data>.inst.MinDamageByLevel[skillData.level].ToString();
				case 4:
					return Singleton<NadineSkillActive1Data>.inst.MaxDamageByLevel[skillData.level].ToString();
				case 5:
					return ((int) (Singleton<NadineSkillActive1Data>.inst.MinSkillApCoef * SelfStat.AttackPower))
						.ToString();
				case 6:
					return ((int) (Singleton<NadineSkillActive1Data>.inst.MaxSkillApCoef * SelfStat.AttackPower))
						.ToString();
				case 7:
					return (skillData.ConcentrationTime + Singleton<NadineSkillActive1Data>.inst.WaitingTime)
						.ToString();
				case 8:
					return Singleton<NadineSkillActive1Data>.inst.PassiveStackCount.ToString();
				case 9:
					return Singleton<NadineSkillActive1Data>.inst.MaxSkillRange2.ToString();
				case 10:
				{
					float recoverySpRatio = Singleton<NadineSkillActive1Data>.inst.RecoverySpRatio;
					return string.Format("{0}%", recoverySpRatio * 100f);
				}
				case 11:
				{
					CharacterStateData data =
						GameDB.characterState.GetData(Singleton<NadineSkillActive1Data>.inst.PassiveBuffState);
					return GetStateStackByGroup(Self, data.group, Self.ObjectId).ToString();
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
					return "ToolTipType/MinDamage";
				case 1:
					return "ToolTipType/MaxDamage";
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
					return Singleton<NadineSkillActive1Data>.inst.MinDamageByLevel[level].ToString();
				case 1:
					return Singleton<NadineSkillActive1Data>.inst.MaxDamageByLevel[level].ToString();
				case 2:
					return skillData.cost.ToString();
				default:
					return "";
			}
		}
	}
}