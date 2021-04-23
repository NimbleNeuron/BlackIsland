using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.LiDailinActive1)]
	public class LocalLiDailinActive1 : LocalSkillScript
	{
		private static readonly int TriggerSkill01_p = Animator.StringToHash("tSkill01_p");


		private static readonly int TriggerSkill01_2_p = Animator.StringToHash("tSkill01_2_p");


		private static readonly int TriggerSkill01_3_p = Animator.StringToHash("tSkill01_3_p");

		public override void Start()
		{
			SetAnimation(Self, BooleanSkill01, true);
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition)
		{
			if (action <= 11)
			{
				if (action == 1)
				{
					PlayAnimation(Self, TriggerSkill01);
					SetAnimation(Self, BooleanMotionWait, true);
					LocalPlayerCharacter.CharacterVoiceControl.PlayCharacterVoice("PlaySkill1010200Seq0_1", 15,
						Self.GetPosition(), true);
					return;
				}

				if (action == 2)
				{
					PlayAnimation(Self, TriggerSkill01_p);
					SetAnimation(Self, BooleanMotionWait, true);
					LocalPlayerCharacter.CharacterVoiceControl.PlayCharacterVoice("PlaySkill1010200Seq0_2", 15,
						Self.GetPosition(), true);
					return;
				}

				if (action != 11)
				{
					return;
				}

				PlayAnimation(Self, TriggerSkill01_2);
				SetAnimation(Self, BooleanMotionWait, true);
				LocalPlayerCharacter.CharacterVoiceControl.PlayCharacterVoice("PlaySkill1010200Seq1_1", 15,
					Self.GetPosition(), true);
			}
			else
			{
				if (action == 12)
				{
					PlayAnimation(Self, TriggerSkill01_2_p);
					SetAnimation(Self, BooleanMotionWait, true);
					LocalPlayerCharacter.CharacterVoiceControl.PlayCharacterVoice("PlaySkill1010200Seq1_2", 15,
						Self.GetPosition(), true);
					return;
				}

				if (action == 21)
				{
					PlayAnimation(Self, TriggerSkill01_3);
					SetAnimation(Self, BooleanMotionWait, true);
					LocalPlayerCharacter.CharacterVoiceControl.PlayCharacterVoice("PlaySkill1010200Seq2_1", 15,
						Self.GetPosition(), true);
					return;
				}

				if (action != 22)
				{
					return;
				}

				PlayAnimation(Self, TriggerSkill01_3_p);
				SetAnimation(Self, BooleanMotionWait, true);
				LocalPlayerCharacter.CharacterVoiceControl.PlayCharacterVoice("PlaySkill1010200Seq2_1", 15,
					Self.GetPosition(), true);
			}
		}


		public override void Finish(bool cancel)
		{
			SetAnimation(Self, BooleanSkill01, false);
		}


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return Singleton<LiDailinSkillData>.inst.A1BaseDamage[skillData.level].ToString();
				case 1:
					return ((int) (Singleton<LiDailinSkillData>.inst.A1ApDamage * SelfStat.AttackPower)).ToString();
				case 2:
				{
					int num = Singleton<LiDailinSkillData>.inst.A1ReinforceBaseDamage[skillData.level];
					int num2 = Singleton<LiDailinSkillData>.inst.A1BaseDamage[skillData.level];
					return (num - num2).ToString();
				}
				case 3:
				{
					float a1ReinforceApDamage = Singleton<LiDailinSkillData>.inst.A1ReinforceApDamage;
					float a1ApDamage = Singleton<LiDailinSkillData>.inst.A1ApDamage;
					return ((int) (a1ReinforceApDamage * SelfStat.AttackPower) -
					        (int) (a1ApDamage * SelfStat.AttackPower)).ToString();
				}
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

			return "ToolTipType/DrinkDamage";
		}


		public override string GetNextLevelTooltipValue(SkillData skillData, int level, int index)
		{
			if (index == 0)
			{
				return Singleton<LiDailinSkillData>.inst.A1BaseDamage[level].ToString();
			}

			if (index != 1)
			{
				return "";
			}

			return Singleton<LiDailinSkillData>.inst.A1ReinforceBaseDamage[level].ToString();
		}


		public override bool GetSkillRange(ref float minRange, ref float maxRange)
		{
			LocalCharacter localCharacter = Self as LocalCharacter;
			if (localCharacter == null)
			{
				return false;
			}

			if (Singleton<LiDailinSkillData>.inst.SkillReinforceExtraPoint <= localCharacter.Status.ExtraPoint)
			{
				minRange = Singleton<LiDailinSkillData>.inst.A1ReinforceDashDistance;
				maxRange = minRange;
			}
			else
			{
				minRange = Singleton<LiDailinSkillData>.inst.A1DashDistance;
				maxRange = minRange;
			}

			return true;
		}
	}
}