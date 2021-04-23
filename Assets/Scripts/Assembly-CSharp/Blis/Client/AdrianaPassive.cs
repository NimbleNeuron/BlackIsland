using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.AdrianaPassive)]
	public class AdrianaPassive : LocalSkillScript
	{
		private const string FX_BI_Adriana_Passive_Sprecovery = "FX_BI_Adriana_Passive_Sprecovery";


		private const string FX_BI_Adriana_Passive_Sprecovery_key = "FX_BI_Adriana_Passive_Sprecovery_key";


		public override void Start() { }


		public override void Play(int action, LocalObject target, Vector3? targetPosition)
		{
			if (action == 1)
			{
				PlayEffectChildManual(Self, "FX_BI_Adriana_Passive_Sprecovery_key", "FX_BI_Adriana_Passive_Sprecovery");
			}

			if (action == 2)
			{
				StopEffectChildManual(Self, "FX_BI_Adriana_Passive_Sprecovery_key", true);
			}
		}


		public override void Finish(bool cancel)
		{
			StopEffectChildManual(Self, "FX_BI_Adriana_Passive_Sprecovery_key", true);
		}


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return Singleton<AdrianaSkillPassiveData>.inst.FireFlameProjectileRefreshPeriod.ToString();
				case 1:
					return Singleton<AdrianaSkillPassiveData>.inst.BurnsDamageByLevel[skillData.level].ToString();
				case 2:
					return Mathf
						.RoundToInt(Singleton<AdrianaSkillPassiveData>.inst.BurnsDamageApCoef * SelfStat.AttackPower)
						.ToString();
				case 3:
					return Singleton<AdrianaSkillPassiveData>.inst.PyromaniacHealSPTerm.ToString();
				case 4:
				{
					float num = Singleton<AdrianaSkillPassiveData>.inst.RecoverySpRatio[skillData.level];
					return string.Format("{0}%", num);
				}
				case 5:
				{
					CharacterStateData data =
						GameDB.characterState.GetData(Singleton<AdrianaSkillPassiveData>.inst
							.BurnsMoveSpeedDownStateCode);
					return string.Format("{0}%", (int) Mathf.Abs(data.statValue1));
				}
				case 6:
				{
					float burnsDamageStackCoef = Singleton<AdrianaSkillPassiveData>.inst.BurnsDamageStackCoef;
					return string.Format("{0}%", burnsDamageStackCoef * 100f);
				}
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

			return "ToolTipType/SpHeal";
		}


		public override string GetNextLevelTooltipValue(SkillData skillData, int level, int index)
		{
			if (index == 0)
			{
				return Singleton<AdrianaSkillPassiveData>.inst.BurnsDamageByLevel[level].ToString();
			}

			if (index != 1)
			{
				return "";
			}

			float num = Singleton<AdrianaSkillPassiveData>.inst.RecoverySpRatio[level];
			return string.Format("{0}%", num);
		}
	}
}