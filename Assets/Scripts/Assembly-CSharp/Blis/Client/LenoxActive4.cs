using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.LenoxActive4)]
	public class LenoxActive4 : LocalSkillScript
	{
		private const string Lenox_Skill04_sfx = "Lenox_Skill04_start";


		private const string Lenox_Skill04_01sfx = "Lenox_Skill04_01";


		private const string Lenox_Skill04_02sfx = "Lenox_Skill04_02";


		private CrossIndicator crossIndicator;


		public override void Start()
		{
			PlayAnimation(Self, TriggerSkill04);
			SetAnimation(Self, BooleanSkill04, true);
			PlaySoundPoint(Self, "Lenox_Skill04_start", 15);
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition)
		{
			Vector3 vector = Singleton<LenoxSkillActive4Data>.inst.Active4ColisionBoxAngleY[action] *
			                 Self.transform.forward;
			Vector3 a = targetPosition != null ? targetPosition.Value : Vector3.zero;
			if (action == 1)
			{
				PlaySoundPoint(Self, "Lenox_Skill04_01", 15);
				Vector3 vector2 = a - Self.GetPosition();
				Self.PlayLocalEffectPoint("FX_BI_Lenox_Skill04a", vector2, GameUtil.LookRotation(vector.normalized));
				return;
			}

			if (action == 2)
			{
				PlaySoundPoint(Self, "Lenox_Skill04_02", 15);
				Vector3 vector3 = a - Self.GetPosition();
				Self.PlayLocalEffectPoint("FX_BI_Lenox_Skill04b", vector3, GameUtil.LookRotation(vector.normalized));
			}
		}


		public override void Finish(bool cancel)
		{
			SetAnimation(Self, BooleanSkill04, false);
		}


		public override float GetSkillInnerRange()
		{
			return SkillData.innerRange * 0.5f + 1f;
		}


		public override void OnDisplaySkillIndicator(Splat indicator)
		{
			base.OnDisplaySkillIndicator(indicator);
			indicator.SetLateUpdateAction(IndicatorLateUpdateAction);
			PointIndicator pointIndicator = indicator as PointIndicator;
			if (pointIndicator != null)
			{
				crossIndicator = pointIndicator.Inner as CrossIndicator;
			}
		}


		private void IndicatorLateUpdateAction(Splat indicator)
		{
			indicator.ShowSelf();
			if (crossIndicator == null)
			{
				return;
			}

			Vector3 dest;
			if (SingletonMonoBehaviour<PlayerController>.inst.GetMouseGroundHit(out dest))
			{
				Vector3 direction = GameUtil.DirectionOnPlane(indicator.transform.position, dest);
				crossIndicator.ResetRotation(direction);
			}
		}


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return Singleton<LenoxSkillActive4Data>.inst.DamageByLevel[skillData.level].ToString();
				case 1:
					return ((int) (Singleton<LenoxSkillActive4Data>.inst.SkillApCoef * SelfStat.AttackPower))
						.ToString();
				case 2:
					return Singleton<LenoxSkillActive4Data>.inst.Active4NormalDamageByLevel[skillData.level].ToString();
				case 3:
					return ((int) GameDB.characterState
						.GetData(Singleton<LenoxSkillActive4Data>.inst.Active4NormalDeBuffCode[skillData.level])
						.duration).ToString();
				case 4:
					return Singleton<LenoxSkillActive4Data>.inst.BlueSnakeActiveTime.ToString("N2");
				case 5:
					return Singleton<LenoxSkillActive4Data>.inst.Active4UpgradeDamageByLevel[skillData.level]
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
					return "ToolTipType/Damage";
				case 1:
					return "ToolTipType/TrueDamage";
				case 2:
					return "ToolTipType/Time";
				case 3:
					return "ToolTipType/CoolTime";
				case 4:
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
					return Singleton<LenoxSkillActive4Data>.inst.DamageByLevel[level].ToString();
				case 1:
					return Singleton<LenoxSkillActive4Data>.inst.Active4NormalDamageByLevel[level].ToString();
				case 2:
					return ((int) GameDB.characterState
							.GetData(Singleton<LenoxSkillActive4Data>.inst.Active4NormalDeBuffCode[level]).duration)
						.ToString();
				case 3:
					return skillData.cooldown.ToString();
				case 4:
					return skillData.cost.ToString();
				default:
					return "";
			}
		}
	}
}