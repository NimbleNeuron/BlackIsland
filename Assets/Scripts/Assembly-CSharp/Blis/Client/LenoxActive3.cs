using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.LenoxActive3)]
	public class LenoxActive3 : LocalSkillScript
	{
		private const string Lenox_Skill03_sfx = "Lenox_Skill03";


		public override void Start()
		{
			PlayAnimation(Self, TriggerSkill03);
			PlaySoundPoint(Self, "Lenox_Skill03", 15);
			Vector3 zero = Vector3.zero;
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel) { }


		public override void OnDisplaySkillIndicator(Splat indicator)
		{
			base.OnDisplaySkillIndicator(indicator);
			indicator.SetLateUpdateAction(IndicatorLateUpdateAction);
		}


		private void IndicatorLateUpdateAction(Splat indicator)
		{
			indicator.ShowSelf();
			Vector3 dest;
			if (SingletonMonoBehaviour<PlayerController>.inst.GetMouseGroundHit(out dest))
			{
				indicator.Direction = GameUtil.DirectionOnPlane(indicator.transform.position, dest);
			}
		}


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return Singleton<LenoxSkillActive3Data>.inst.DamageByLevel[skillData.level].ToString();
				case 1:
					return ((int) (Singleton<LenoxSkillActive3Data>.inst.SkillApCoef * SelfStat.AttackPower))
						.ToString();
				case 2:
					return GameDB.characterState
						.GetData(Singleton<LenoxSkillActive3Data>.inst.Active3SlowCode[skillData.level]).duration
						.ToString();
				case 3:
				{
					float num = Mathf.Abs(GameDB.characterState
						.GetData(Singleton<LenoxSkillActive3Data>.inst.Active3SlowCode[skillData.level]).statValue1);
					return string.Format("{0}%", (int) num);
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
					return "ToolTipType/DecreaseMoveRatio";
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
					return Singleton<LenoxSkillActive3Data>.inst.DamageByLevel[level].ToString();
				case 1:
				{
					float num = Mathf.Abs(GameDB.characterState
						.GetData(Singleton<LenoxSkillActive3Data>.inst.Active3SlowCode[level]).statValue1);
					return string.Format("{0}%", (int) num);
				}
				case 2:
					return skillData.cost.ToString();
				default:
					return "";
			}
		}
	}
}