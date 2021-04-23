using System.Collections;
using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.SilviaPassive)]
	public class LocalSilviaPassive : LocalSkillScript
	{
		private const string AnimFloatMoveAngle = "Angle";


		private static readonly int FloatMoveAngle = Animator.StringToHash("Angle");


		private float pastAngle;

		public override void Start()
		{
			pastAngle = 0f;
			StartCoroutine(UpdateRotateAngleAnimator());
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition)
		{
			if (action == 1)
			{
				PlayEffectChild(Self, "FX_BI_Silvia_Passive_New", "Root");
				PlaySoundPoint(Self, "Silvia_Passive_New", 15);
				return;
			}

			if (action == 2)
			{
				PlayEffectChild(Self, "FX_BI_Silvia_Passive_Fuel", "Root");
				PlaySoundPoint(Self, "Silvia_Passive_Fuel", 15);
				return;
			}

			if (action == 3)
			{
				PlayEffectChild(Self, "FX_BI_Silvia_Passive_Complete", "Root");
				PlaySoundPoint(Self, "Silvia_Passive_Complete", 15);
			}
		}


		public override void Finish(bool cancel)
		{
			StopCoroutines();
		}


		private IEnumerator UpdateRotateAngleAnimator()
		{
			for (;;)
			{
				yield return null;
				float num = 0f;
				if (!LocalPlayerCharacter.MoveAgent.IsLockRotation())
				{
					for (num = Self.transform.eulerAngles.y -
					           LocalPlayerCharacter.MoveAgent.TargetRotation.eulerAngles.y;
						num > 180f;
						num -= 360f) { }

					while (num < -180f)
					{
						num += 360f;
					}
				}

				if (pastAngle != num)
				{
					SetAnimation(LocalPlayerCharacter, FloatMoveAngle, num);
					pastAngle = num;
				}
			}
		}


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return GameDB.characterState
						.GetData(Singleton<SilviaSkillCommonData>.inst.PassiveAttackSpeedBuffStateCode[skillData.level])
						.statValue1 + "%";
				case 1:
					return Singleton<SilviaSkillHumanData>.inst.FillEpPeriod.ToString();
				case 2:
					return GameDB.characterState
						.GetData(Singleton<SilviaSkillCommonData>.inst.PassiveSkillDamageRatioStateCode).statValue1
						.ToString();
				case 3:
					return Singleton<SilviaSkillCommonData>.inst.PassiveEpAmount[skillData.level].ToString();
				case 4:
					return Singleton<SilviaSkillCommonData>.inst.PassiveSkillCoolTime.ToString();
				case 5:
					return Singleton<SilviaSkillHumanData>.inst.FillEpAmount.ToString();
				default:
					return "";
			}
		}


		public override string GetNextLevelTooltipParam(int index)
		{
			if (index == 0)
			{
				return "StatType/AttackSpeedRatio";
			}

			if (index != 1)
			{
				return "";
			}

			return "ToolTipType/EpMount";
		}


		public override string GetNextLevelTooltipValue(SkillData skillData, int level, int index)
		{
			if (index == 0)
			{
				return GameDB.characterState
					       .GetData(Singleton<SilviaSkillCommonData>.inst.PassiveAttackSpeedBuffStateCode[level])
					       .statValue1 +
				       "%";
			}

			if (index != 1)
			{
				return "";
			}

			return Singleton<SilviaSkillCommonData>.inst.PassiveEpAmount[level].ToString();
		}
	}
}