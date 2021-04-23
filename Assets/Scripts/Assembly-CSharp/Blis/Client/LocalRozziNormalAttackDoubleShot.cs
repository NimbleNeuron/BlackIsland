using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.RozziNormalAttackDoubleShot)]
	public class LocalRozziNormalAttackDoubleShot : LocalSkillScript
	{
		private const string NormalAttackCancelTag = "RozziNormalAttackCancel";


		private readonly int TriggerAttackDoubleShot = Animator.StringToHash("tAttack03");


		public override void Start()
		{
			PlayAnimation(Self, TriggerAttackDoubleShot);
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel)
		{
			if (cancel)
			{
				StopEffectByTag(Self, "RozziNormalAttackCancel");
				StopSoundByTag(Self, "RozziNormalAttackCancel");
			}
		}


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return Mathf
						.RoundToInt(
							(Singleton<RozziSkillDoubleShotData>.inst.DoubleShotPistolApCoefByLevel_1[skillData.level] *
								SelfStat.AttackPower + SelfStat.IncreaseBasicAttackDamage) *
							(1f + SelfStat.IncreaseBasicAttackDamageRatio)).ToString();
				case 1:
					return Mathf
						.RoundToInt(
							(Singleton<RozziSkillDoubleShotData>.inst.DoubleShotPistolApCoefByLevel_2[skillData.level] *
								SelfStat.AttackPower + SelfStat.IncreaseBasicAttackDamage) *
							(1f + SelfStat.IncreaseBasicAttackDamageRatio)).ToString();
				case 2:
					return GameDB.characterState
						.GetData(Singleton<RozziSkillPassiveData>.inst.DoubleShotStateCodeByLevel[skillData.level])
						.duration.ToString();
				case 3:
				{
					int num = (int) (Singleton<RozziSkillPassiveData>.inst.ChocolateSpRatio * 100f);
					return string.Format("{0}%", num);
				}
				case 4:
				{
					int num2 = (int) (Singleton<RozziSkillPassiveData>.inst.ChocolateHpRatio * 100f);
					return string.Format("{0}%", num2);
				}
				default:
					return "";
			}
		}


		public override string GetNextLevelTooltipParam(int index)
		{
			if (index == 0)
			{
				return "ToolTipType/SecondDamage";
			}

			return "";
		}


		public override string GetNextLevelTooltipValue(SkillData skillData, int level, int index)
		{
			if (index == 0)
			{
				return Mathf
					.RoundToInt(
						(Singleton<RozziSkillDoubleShotData>.inst.DoubleShotPistolApCoefByLevel_2[level] *
							SelfStat.AttackPower + SelfStat.IncreaseBasicAttackDamage) *
						(1f + SelfStat.IncreaseBasicAttackDamageRatio)).ToString();
			}

			return "";
		}
	}
}