using System;
using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.RozziActive4)]
	public class LocalRozziActive4 : LocalSkillScript
	{
		private const string Set_CharacterHit = "FX_BI_Rozzi_Skill04_Set_Character_Hit";


		private int attachStateMaxStack;

		public override void Start()
		{
			PlayAnimation(Self, TriggerReloadCancel);
			PlayAnimation(Self, TriggerSkill04);
			PlaySoundPoint(Self, "Rozzi_Skill04_Fire", 15);
			attachStateMaxStack = GameDB.characterState
				.GetData(Singleton<RozziSkillActive4Data>.inst.AttachDebuffStackStateCode).maxStack;
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition)
		{
			if (targetPosition != null)
			{
				if (action == 1)
				{
					target.SetRotation(GameUtil.LookRotation(targetPosition.Value));
					return;
				}

				if (action == 2)
				{
					GameObject gameObject = PlayEffectChild(target, "FX_BI_Rozzi_Skill04_Set_Character_Hit");
					if (gameObject != null)
					{
						gameObject.transform.rotation = GameUtil.LookRotation(targetPosition.Value);
						gameObject.transform.position += new Vector3(0f, 1.5f, 0f);
					}
				}
			}
		}


		public override void Finish(bool cancel)
		{
			ResetAnimatorTrigger(Self, TriggerReloadCancel);
		}


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return Singleton<RozziSkillActive4Data>.inst.BombTimerOnGround.ToString();
				case 1:
				{
					int num = (int) Math.Abs(GameDB.characterState
						.GetData(Singleton<RozziSkillActive4Data>.inst.AttachDebuffStateCodeByLevel[skillData.level])
						.statValue1);
					return string.Format("{0}%", num);
				}
				case 2:
					return Singleton<RozziSkillActive4Data>.inst.DamageActive4ByLevel[skillData.level].ToString();
				case 3:
					return ((int) (Singleton<RozziSkillActive4Data>.inst.DamageActive4ApCoef * SelfStat.AttackPower))
						.ToString();
				case 4:
					return GameDB.characterState
						.GetData(Singleton<RozziSkillActive4Data>.inst.AttachDebuffStackStateCode).maxStack.ToString();
				case 5:
				{
					int num2 = (int) Math.Abs(
						Singleton<RozziSkillActive4Data>.inst.AdditionalDamageRatioByLevel[skillData.level]);
					return string.Format("{0}%", num2);
				}
				case 6:
				{
					int num3 = (int) Math.Abs(Singleton<RozziSkillActive4Data>.inst.FullStackCoolDownValue * 100f);
					return string.Format("{0}%", num3);
				}
				case 7:
					return GameDB.characterState
						.GetData(Singleton<RozziSkillActive4Data>.inst.AttachFullStackBuffStateCode).duration
						.ToString();
				case 8:
				{
					int num4 = (int) Math.Abs(GameDB.characterState
						.GetData(Singleton<RozziSkillActive4Data>.inst.AttachFullStackBuffStateCode).statValue1);
					return string.Format("{0}%", num4);
				}
				case 9:
					return Singleton<RozziSkillActive4Data>.inst.AttachAfterSkillDamageStack.ToString();
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
					return "ToolTipType/MaxhpDamage";
				case 2:
					return "ToolTipType/DecreaseMoveRatio";
				case 3:
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
					return Singleton<RozziSkillActive4Data>.inst.DamageActive4ByLevel[level].ToString();
				case 1:
				{
					int num = (int) Math.Abs(Singleton<RozziSkillActive4Data>.inst.AdditionalDamageRatioByLevel[level]);
					return string.Format("{0}%", num);
				}
				case 2:
				{
					int num2 = (int) Math.Abs(GameDB.characterState
						.GetData(Singleton<RozziSkillActive4Data>.inst.AttachDebuffStateCodeByLevel[skillData.level])
						.statValue1);
					return string.Format("{0}%", num2);
				}
				case 3:
					return skillData.cooldown.ToString();
				default:
					return "";
			}
		}
	}
}