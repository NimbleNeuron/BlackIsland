using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.YukiPassive)]
	public class YukiPassive : LocalSkillScript
	{
		private const string FX_BI_Yuki_Passive = "FX_BI_Yuki_Passive";


		private const string FX_BI_Yuki_Passive_Damaged = "FX_BI_Yuki_Passive_Damaged";


		private const string Yuki_Passive = "Yuki_Passive";


		private const string Yuki_Passive_Active = "Yuki_Passive_Active";


		private const string Yuki_Passive_Count_Increase = "Yuki_Passive_Count_Increase";


		public override void Start()
		{
			PlayEffectChildManual(Self, "Yuki_Passive", "FX_BI_Yuki_Passive");
		}


		public override void Play(int actionNo, LocalObject target, Vector3? targetPosition)
		{
			if (actionNo == 1)
			{
				PlayEffectChildManual(Self, "Yuki_Passive", "FX_BI_Yuki_Passive");
				if ((Self as LocalCharacter).Status.ExtraPoint < 1)
				{
					PlaySoundPoint(Self, "Yuki_Passive_Active");
					return;
				}

				PlaySoundPoint(Self, "Yuki_Passive_Count_Increase");
			}
			else
			{
				if (actionNo == 2)
				{
					PlayEffectPoint(Self, "FX_BI_Yuki_Passive_Damaged");
					return;
				}

				if (actionNo == 3)
				{
					StopEffectChildManual(Self, "Yuki_Passive", true);
				}
			}
		}


		public override void Finish(bool cancel)
		{
			StopEffectChildManual(Self, "Yuki_Passive", true);
		}


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return Singleton<YukiSkillPassiveData>.inst.Damage[skillData.level].ToString();
				case 1:
				{
					int num = Singleton<YukiSkillPassiveData>.inst.ReduceDamageRatio[skillData.level];
					return string.Format("{0}%", num);
				}
				case 2:
					return 1.ToString();
				case 3:
					return Singleton<YukiSkillPassiveData>.inst.RecoverySeconds.ToString();
				case 4:
					return 1.ToString();
				default:
					return "";
			}
		}


		public override string GetNextLevelTooltipParam(int index)
		{
			if (index == 0)
			{
				return "ToolTipType/TrueDamage";
			}

			return "";
		}


		public override string GetNextLevelTooltipValue(SkillData skillData, int level, int index)
		{
			if (index == 0)
			{
				return Singleton<YukiSkillPassiveData>.inst.Damage[level].ToString();
			}

			return "";
		}
	}
}