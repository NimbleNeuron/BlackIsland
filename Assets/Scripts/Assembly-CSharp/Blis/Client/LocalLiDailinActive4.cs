using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.LiDailinActive4)]
	public class LocalLiDailinActive4 : LocalSkillScript
	{
		private static readonly int TriggerSkill04_p = Animator.StringToHash("tSkill04_p");


		private static readonly int BooleanSkill04_p = Animator.StringToHash("bSkill04_p");


		private GameObject DashEffect;


		public override void Start() { }


		public override void Play(int action, LocalObject target, Vector3? targetPosition)
		{
			if (action == 1)
			{
				SetAnimation(Self, BooleanSkill04, true);
				DashEffect = PlayEffectChild(Self, "FX_BI_Dailin_Skill04_Dash");
			}
			else if (action == 2)
			{
				SetAnimation(Self, BooleanSkill04_p, true);
				DashEffect = PlayEffectChild(Self, "FX_BI_Dailin_Skill04_Dash");
			}

			if (action == 11)
			{
				SetAnimation(Self, BooleanSkill04, false);
				PlayAnimation(Self, TriggerSkill04);
				LocalPlayerCharacter.CharacterVoiceControl.PlayCharacterVoice("PlaySkill1010500Seq0_1", 15,
					Self.GetPosition(), true);
			}
			else if (action == 12)
			{
				SetAnimation(Self, BooleanSkill04_p, false);
				PlayAnimation(Self, TriggerSkill04_p);
				LocalPlayerCharacter.CharacterVoiceControl.PlayCharacterVoice("PlaySkill1010500Seq0_2", 15,
					Self.GetPosition(), true);
			}

			if (action == 3)
			{
				SetAnimation(Self, BooleanSkill04, false);
				SetAnimation(Self, BooleanSkill04_p, false);
			}
		}


		public override void Finish(bool cancel)
		{
			StopEffectByTag(Self, " Lidailin_Dash");
			if (DashEffect != null)
			{
				DashEffect.transform.parent = null;
			}

			SetAnimation(Self, BooleanSkill04, false);
			SetAnimation(Self, BooleanSkill04_p, false);
		}


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return Singleton<LiDailinSkillData>.inst.A4DamageBase[skillData.level].ToString();
				case 1:
					return ((int) (Singleton<LiDailinSkillData>.inst.A4ApDamage * SelfStat.AttackPower)).ToString();
				case 2:
					return (3 * Singleton<LiDailinSkillData>.inst.A4DamageBase[skillData.level]).ToString();
				case 3:
					return ((int) (3f * Singleton<LiDailinSkillData>.inst.A4ApDamage * SelfStat.AttackPower))
						.ToString();
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
					return "ToolTipType/CoolTime";
				default:
					return "";
			}
		}


		public override string GetNextLevelTooltipValue(SkillData skillData, int level, int index)
		{
			switch (index)
			{
				case 0:
					return Singleton<LiDailinSkillData>.inst.A4DamageBase[level].ToString();
				case 1:
					return (3 * Singleton<LiDailinSkillData>.inst.A4DamageBase[level]).ToString();
				case 2:
					return skillData.cooldown.ToString();
				default:
					return "";
			}
		}
	}
}