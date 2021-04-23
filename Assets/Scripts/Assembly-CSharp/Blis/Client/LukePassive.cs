using System.Collections.Generic;
using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.LukePassive)]
	public class LukePassive : LocalSkillScript
	{
		public override void Start() { }


		public override void Play(int actionNo, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel) { }


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
				{
					float num = Singleton<LukeSkillPassiveData>.inst.ReocveryLostHPRatioPlayerKill[skillData.level];
					return string.Format("{0}%", num * 100f);
				}
				case 1:
				{
					CharacterStateData data =
						GameDB.characterState.GetData(Singleton<LukeSkillPassiveData>.inst.BuffState);
					int stateStackByGroup = GetStateStackByGroup(Self, data.group, Self.ObjectId);
					int addRecoveryHpAmount =
						Singleton<LukeSkillPassiveData>.inst.GetAddRecoveryHpAmount(stateStackByGroup);
					return string.Format("{0}%", addRecoveryHpAmount);
				}
				case 2:
				{
					float num2 = Singleton<LukeSkillPassiveData>.inst.ReocveryLostHPRatioWicklineKill[skillData.level];
					return string.Format("{0}%", num2 * 100f);
				}
				case 3:
				case 4:
				case 5:
				{
					List<SkillEvolutionPointData> list =
						GameDB.skill.skillEvolutionPointMap[LocalCharacter.CharacterCode];
					int num3 = LocalCharacter.CharacterCode * 10 + index;
					foreach (SkillEvolutionPointData skillEvolutionPointData in list)
					{
						if (skillEvolutionPointData.code == num3 - 2)
						{
							return skillEvolutionPointData.conditionValue2;
						}
					}

					return "";
				}
				case 6:
				{
					float num4 = Singleton<LukeSkillPassiveData>.inst.CleaningCompletedStackMax;
					return string.Format("{0}", num4);
				}
				case 7:
				{
					float num5 = Singleton<LukeSkillPassiveData>.inst.AddRecoveryHpIncreaseStackUnit;
					return string.Format("{0}", num5);
				}
				case 8:
				{
					float num6 = Singleton<LukeSkillPassiveData>.inst.AddRecoveryHpIncrease;
					return string.Format("{0}%", num6);
				}
				case 9:
				{
					float num7 = Singleton<LukeSkillPassiveData>.inst.MonsterStack[MonsterType.Bat];
					return string.Format("{0}", num7);
				}
				case 10:
				{
					float num8 = Singleton<LukeSkillPassiveData>.inst.MonsterStack[MonsterType.Bear];
					return string.Format("{0}", num8);
				}
				case 11:
				{
					float num9 = Singleton<LukeSkillPassiveData>.inst.PlayerKillStack;
					return string.Format("{0}", num9);
				}
				case 12:
				{
					float num10 = Singleton<LukeSkillPassiveData>.inst.MonsterStack[MonsterType.Wickline];
					return string.Format("{0}", num10);
				}
				case 13:
				{
					float num11 = Singleton<LukeSkillPassiveData>.inst.AirSurpplyStack[ItemGrade.Uncommon];
					return string.Format("{0}", num11);
				}
				case 14:
				{
					float num12 = Singleton<LukeSkillPassiveData>.inst.AirSurpplyStack[ItemGrade.Rare];
					return string.Format("{0}", num12);
				}
				case 15:
				{
					float num13 = Singleton<LukeSkillPassiveData>.inst.AirSurpplyStack[ItemGrade.Epic];
					return string.Format("{0}", num13);
				}
				case 16:
				{
					float num14 = Singleton<LukeSkillPassiveData>.inst.AirSurpplyStack[ItemGrade.Legend];
					return string.Format("{0}", num14);
				}
				default:
					return "";
			}
		}


		public override string GetNextLevelTooltipParam(int index)
		{
			if (index == 0)
			{
				return "ToolTipType/HpRegenRatioPlayer";
			}

			if (index != 1)
			{
				return "";
			}

			return "ToolTipType/HpRegenRatioWickline";
		}


		public override string GetNextLevelTooltipValue(SkillData skillData, int level, int index)
		{
			if (index == 0)
			{
				float num = Singleton<LukeSkillPassiveData>.inst.ReocveryLostHPRatioPlayerKill[skillData.level];
				return string.Format("{0}%", num * 100f);
			}

			if (index != 1)
			{
				return "";
			}

			float num2 = Singleton<LukeSkillPassiveData>.inst.ReocveryLostHPRatioWicklineKill[skillData.level];
			return string.Format("{0}%", num2 * 100f);
		}
	}
}