using System.Collections.Generic;
using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.EmmaActive1Summon)]
	public class EmmaActive1Summon : LocalEmmaSkillScript
	{
		public override void Start()
		{
			PlayAnimation(Self, TriggerSkill01);
			PlaySoundPoint(Self, "Emma_Skill01_Fire", 15);
			base.Start();
			List<LocalSummonServant> pigeons = GetPigeons(LocalPlayerCharacter);
			if (pigeons != null)
			{
				HidePigeonsIndicator(pigeons);
			}
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition)
		{
			base.Play(action, target, targetPosition);
		}


		public override void Finish(bool cancel)
		{
			base.Finish(cancel);
		}


		public override void OnDisplaySkillIndicator(Splat indicator)
		{
			List<LocalSummonServant> pigeons = GetPigeons(LocalPlayerCharacter);
			if (pigeons != null)
			{
				ShowPigeonsIndicator(pigeons);
			}

			base.OnDisplaySkillIndicator(indicator);
		}


		public override void OnHideSkillIndicator(Splat indicator)
		{
			List<LocalSummonServant> pigeons = GetPigeons(LocalPlayerCharacter);
			if (pigeons != null)
			{
				HidePigeonsIndicator(pigeons);
			}

			base.OnHideSkillIndicator(indicator);
		}


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return Singleton<EmmaSkillActive1Data>.inst.DamageBySkillLevel[skillData.level].ToString();
				case 1:
					return ((int) (Singleton<EmmaSkillActive1Data>.inst.DamageApCoef * SelfStat.AttackPower))
						.ToString();
				case 2:
					return GameDB.character.GetSummonData(Singleton<EmmaSkillActive1Data>.inst.PigeonSummonCode)
						.createVisibleTime.ToString();
				case 3:
					return GameDB.character.GetSummonData(Singleton<EmmaSkillActive1Data>.inst.PigeonSummonCode)
						.duration.ToString();
				case 4:
					return Mathf.Abs(Singleton<EmmaSkillActive1Data>.inst.CooldownReduce).ToString();
				case 5:
					return GameDB.characterState
						.GetData(Singleton<EmmaSkillActive1Data>.inst.PigeonDealerMoveSpeedDownStateCode).duration
						.ToString();
				case 6:
					return Mathf.Abs(GameDB.characterState
						       .GetData(Singleton<EmmaSkillActive1Data>.inst.PigeonDealerMoveSpeedDownStateCode)
						       .statValue1) +
					       "%";
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

			return "ToolTipType/Cost";
		}


		public override string GetNextLevelTooltipValue(SkillData skillData, int level, int index)
		{
			if (index == 0)
			{
				return Singleton<EmmaSkillActive1Data>.inst.DamageBySkillLevel[level].ToString();
			}

			if (index != 1)
			{
				return "";
			}

			return skillData.cost.ToString();
		}
	}
}