using System;
using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.SilviaHumanActive1)]
	public class LocalSilviaHumanActive1 : LocalSkillScript
	{
		public override void Start()
		{
			PlayAnimation(Self, TriggerSkill01);
			PlayEffectPoint(Self, "FX_BI_Silvia_Skill01_Ready");
			PlaySoundPoint(Self, "Silvia_Skill01_Ready", 15);
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition)
		{
			if (action == 1)
			{
				PlayEffectPoint(Self, "FX_BI_Silvia_Skill01_Fire");
				PlaySoundPoint(Self, "Silvia_Skill01_Fire", 15);
				return;
			}

			if (action == 2)
			{
				GameObject gameObject = PlayEffectPoint(target, "FX_BI_Silvia_Skill01_Hit_Enemy_Hit", "Bip001");
				if (gameObject != null)
				{
					gameObject.transform.LookAt(Self.GetPosition());
				}
			}
		}


		public override void Finish(bool cancel) { }


		public override void OnDisplaySkillIndicator(Splat indicator)
		{
			base.OnDisplaySkillIndicator(indicator);
			indicator.Length = Singleton<SilviaSkillHumanData>.inst.A1HitRange;
			indicator.SetLateUpdateAction(IndicatorLateUpdateAction);
		}


		private void IndicatorLateUpdateAction(Splat indicator)
		{
			LocalMovableCharacter localMovableCharacter =
				SingletonMonoBehaviour<PlayerController>.inst.mouseHitObject as LocalMovableCharacter;
			if (localMovableCharacter == null)
			{
				indicator.HideSelf();
				return;
			}

			indicator.ShowSelf();
			indicator.Direction = GameUtil.Direction(indicator.transform.position, localMovableCharacter.GetPosition());
		}


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return Singleton<SilviaSkillHumanData>.inst.A1BaseDamage[skillData.level].ToString();
				case 1:
					return ((int) (Singleton<SilviaSkillHumanData>.inst.A1ApDamage * SelfStat.AttackPower)).ToString();
				case 2:
					return Singleton<SilviaSkillHumanData>.inst.A1BaseHeal[skillData.level].ToString();
				case 3:
					return ((int) (Singleton<SilviaSkillHumanData>.inst.A1ApHeal * SelfStat.AttackPower)).ToString();
				case 4:
					return Math.Abs(Singleton<SilviaSkillHumanData>.inst.A1CooldownModifyMelee).ToString();
				case 5:
					return Math.Abs(Singleton<SilviaSkillHumanData>.inst.A1CooldownModifyRange).ToString();
				case 6:
					return Singleton<SilviaSkillHumanData>.inst.A1EpGainPerHit.ToString();
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
					return "ToolTipType/Heal";
				case 2:
					return "ToolTipType/CoolTime";
				case 3:
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
					return Singleton<SilviaSkillHumanData>.inst.A1BaseDamage[level].ToString();
				case 1:
					return Singleton<SilviaSkillHumanData>.inst.A1BaseHeal[level].ToString();
				case 2:
					return skillData.cooldown.ToString();
				case 3:
					return skillData.cost.ToString();
				default:
					return "";
			}
		}
	}
}