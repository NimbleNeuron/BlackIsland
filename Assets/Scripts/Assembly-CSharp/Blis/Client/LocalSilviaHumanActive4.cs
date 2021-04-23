using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.SilviaHumanActive4)]
	public class LocalSilviaHumanActive4 : LocalSkillScript
	{
		public override void Start() { }


		public override void Play(int action, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel) { }


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return GameDB.characterState
						.GetData(Singleton<SilviaSkillBikeData>.inst.BikeSpeedUpState[skillData.level]).statValue1
						.ToString();
				case 1:
					return Singleton<SilviaSkillHumanData>.inst.A4CanUseConditionEp.ToString();
				case 2:
					return Singleton<SilviaSkillBikeData>.inst.ConsumeEpAmount.ToString();
				case 3:
					return Singleton<SilviaSkillBikeData>.inst.ConsumeEpPeriod.ToString();
				case 4:
					return GameDB.characterState
						.GetData(Singleton<SilviaSkillBikeData>.inst.BikeSpeedUpState[skillData.level]).statValue2
						.ToString();
				default:
					return "";
			}
		}


		public override string GetNextLevelTooltipParam(int index)
		{
			if (index == 0)
			{
				return "ToolTipType/DefaultMoveSpeed";
			}

			if (index != 1)
			{
				return "";
			}

			return "StatType/Defense";
		}


		public override string GetNextLevelTooltipValue(SkillData skillData, int level, int index)
		{
			if (index == 0)
			{
				return GameDB.characterState.GetData(Singleton<SilviaSkillBikeData>.inst.BikeSpeedUpState[level])
					.statValue1.ToString();
			}

			if (index != 1)
			{
				return "";
			}

			return GameDB.characterState.GetData(Singleton<SilviaSkillBikeData>.inst.BikeSpeedUpState[level]).statValue2
				.ToString();
		}


		public override UseSkillErrorCode IsCanUseSkill(LocalObject hitTarget, Vector3? cursorPosition)
		{
			if (LocalCharacter.Status.ExtraPoint < Singleton<SilviaSkillHumanData>.inst.A4CanUseConditionEp)
			{
				return UseSkillErrorCode.NotEnoughExCost;
			}

			return UseSkillErrorCode.None;
		}


		public override UseSkillErrorCode IsEnableSkillSlot()
		{
			if (LocalCharacter.Status.ExtraPoint < Singleton<SilviaSkillHumanData>.inst.A4CanUseConditionEp)
			{
				return UseSkillErrorCode.NotEnoughExCost;
			}

			return UseSkillErrorCode.None;
		}
	}
}