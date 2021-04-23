using System;
using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.EmmaPassive)]
	public class EmmaPassive : LocalEmmaSkillScript
	{
		private int playCount;


		public override void Start()
		{
			base.Start();
			if (3f < Time.time - lastNormalAttackTime)
			{
				playCount = 0;
			}
			else
			{
				playCount++;
			}

			lastNormalAttackTime = Time.time;
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition)
		{
			base.Play(action, target, targetPosition);
		}


		public override void Finish(bool cancel)
		{
			base.Finish(cancel);
		}


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return skillData.cooldown.ToString();
				case 1:
				{
					float num = Singleton<EmmaSkillPassiveData>.inst.CheerUpDamageMaxSpRatioByLevel[skillData.level] *
					            100f;
					return string.Format("{0}%", num);
				}
				case 2:
					return Singleton<EmmaSkillPassiveData>.inst.CheerUpShieldByLevel[skillData.level].ToString();
				case 3:
				{
					int num2 = (int) Math.Abs(
						Singleton<EmmaSkillPassiveData>.inst.CheerUpShieldAdditionalMaxSpRatio[skillData.level] * 100f);
					return string.Format("{0}%", num2);
				}
				case 4:
					return ((int) (Singleton<EmmaSkillPassiveData>.inst.CheerUpShieldAdditionalMaxSpRatio[
						skillData.level] * SelfStat.MaxSp)).ToString();
				default:
					return "";
			}
		}


		public override string GetNextLevelTooltipParam(int index)
		{
			switch (index)
			{
				case 0:
					return "ToolTipType/StaminaDamageRatio";
				case 1:
					return "ToolTipType/Shield";
				case 2:
					return "ToolTipType/StaminaShieldRatio";
				default:
					return "";
			}
		}


		public override string GetNextLevelTooltipValue(SkillData skillData, int level, int index)
		{
			switch (index)
			{
				case 0:
				{
					float num = Singleton<EmmaSkillPassiveData>.inst.CheerUpDamageMaxSpRatioByLevel[level] * 100f;
					return string.Format("{0}%", num);
				}
				case 1:
					return Singleton<EmmaSkillPassiveData>.inst.CheerUpShieldByLevel[level].ToString();
				case 2:
				{
					int num2 = (int) Math.Abs(
						Singleton<EmmaSkillPassiveData>.inst.CheerUpShieldAdditionalMaxSpRatio[level] * 100f);
					return string.Format("{0}%", num2);
				}
				default:
					return "";
			}
		}
	}
}