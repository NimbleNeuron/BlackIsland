using System;
using System.Collections.Generic;
using System.Text;
using Blis.Common;

namespace Blis.Client
{
	public static class LnUtil
	{
		private static readonly Dictionary<int, string> getCharacterNamePath = new Dictionary<int, string>();


		private static readonly Dictionary<int, string> getSkinNamePath = new Dictionary<int, string>();


		private static readonly Dictionary<int, string> getSkinTitlePath = new Dictionary<int, string>();


		private static readonly Dictionary<int, string> getSkinDescPath = new Dictionary<int, string>();


		private static readonly Dictionary<int, string> getMonsterNamePath = new Dictionary<int, string>();


		private static readonly Dictionary<int, string> getItemNamePath = new Dictionary<int, string>();


		private static readonly Dictionary<int, string> getAreaNamePath = new Dictionary<int, string>();


		private static readonly Dictionary<MasteryType, string> getMasteryNamePath =
			new Dictionary<MasteryType, string>(SingletonComparerEnum<MasteryTypeComparer, MasteryType>.Instance);


		private static readonly Dictionary<MasteryType, string> getMasteryDescPath =
			new Dictionary<MasteryType, string>(SingletonComparerEnum<MasteryTypeComparer, MasteryType>.Instance);


		private static readonly Dictionary<MasteryConditionType, string> getMasteryConditionPath =
			new Dictionary<MasteryConditionType, string>(
				SingletonComparerEnum<MasteryConditionTypeComparer, MasteryConditionType>.Instance);


		private static readonly Dictionary<CastingActionType, string> getCastingActionDescPath =
			new Dictionary<CastingActionType, string>(
				SingletonComparerEnum<CastingActionTypeComparer, CastingActionType>.Instance);


		private static readonly Dictionary<int, string> getLobbySkillDescPath = new Dictionary<int, string>();


		private static readonly Dictionary<int, string> getSkillDescKeyPath = new Dictionary<int, string>();


		private static readonly Dictionary<int, string> getSkillEvolutionNamePath = new Dictionary<int, string>();


		private static readonly Dictionary<int, string> getSkillEvolutionDescPath = new Dictionary<int, string>();


		private static readonly Dictionary<int, string> getCharacterStateNamePath = new Dictionary<int, string>();


		private static readonly Dictionary<int, string> getSkillNamePath = new Dictionary<int, string>();


		private static readonly Dictionary<int, string> getToastMessagePath = new Dictionary<int, string>();


		private static readonly Dictionary<SkillCostType, string> getSkillCostTypePath =
			new Dictionary<SkillCostType, string>(SingletonComparerEnum<SkillCostTypeComparer, SkillCostType>.Instance);

		public static string GetCharacterName(int code)
		{
			if (!getCharacterNamePath.ContainsKey(code))
			{
				getCharacterNamePath.Add(code, string.Format("Character/Name/{0}", code));
			}

			return Ln.Get(getCharacterNamePath[code]);
		}


		public static string GetSkinName(int code)
		{
			if (!getSkinNamePath.ContainsKey(code))
			{
				getSkinNamePath.Add(code, string.Format("Skin/Name/{0}", code));
			}

			return Ln.Get(getSkinNamePath[code]);
		}


		public static string GetSkinTitle(int code)
		{
			if (!getSkinTitlePath.ContainsKey(code))
			{
				getSkinTitlePath.Add(code, string.Format("Skin/Title/{0}", code));
			}

			return Ln.Get(getSkinTitlePath[code]);
		}


		public static string GetSkinDesc(int code)
		{
			if (!getSkinDescPath.ContainsKey(code))
			{
				getSkinDescPath.Add(code, string.Format("Skin/Desc/{0}", code));
			}

			return Ln.Get(getSkinDescPath[code]);
		}


		public static string GetMonsterName(int code)
		{
			if (!getMonsterNamePath.ContainsKey(code))
			{
				getMonsterNamePath.Add(code, string.Format("Monster/Name/{0}", code));
			}

			return Ln.Get(getMonsterNamePath[code]);
		}


		public static string GetItemName(int code)
		{
			if (!getItemNamePath.ContainsKey(code))
			{
				getItemNamePath.Add(code, string.Format("Item/Name/{0:000000}", code));
			}

			return Ln.Get(getItemNamePath[code]);
		}


		public static string GetAreaName(int code)
		{
			if (!getAreaNamePath.ContainsKey(code))
			{
				getAreaNamePath.Add(code, string.Format("Area/Name/{0}", code));
			}

			return Ln.Get(getAreaNamePath[code]);
		}


		public static string GetMasteryName(MasteryType masteryType)
		{
			if (!getMasteryNamePath.ContainsKey(masteryType))
			{
				getMasteryNamePath.Add(masteryType, string.Format("MasteryType/{0}", masteryType));
			}

			return Ln.Get(getMasteryNamePath[masteryType]);
		}


		public static string GetMasteryDesc(MasteryType masteryType, int level)
		{
			if (!getMasteryDescPath.ContainsKey(masteryType))
			{
				getMasteryDescPath.Add(masteryType, string.Format("MasteryType/{0}/Desc/", masteryType));
			}

			return Ln.Get(getMasteryDescPath[masteryType] + level);
		}


		public static string GetMasteryCondition(MasteryConditionType masteryConditionType)
		{
			if (!getMasteryConditionPath.ContainsKey(masteryConditionType))
			{
				getMasteryConditionPath.Add(masteryConditionType,
					string.Format("MasteryConditionType/{0}", masteryConditionType));
			}

			return Ln.Get(getMasteryConditionPath[masteryConditionType]);
		}


		public static string GetCastingActionDesc(CastingActionType castingActionType)
		{
			if (!getCastingActionDescPath.ContainsKey(castingActionType))
			{
				getCastingActionDescPath.Add(castingActionType,
					string.Format("CastingActionType/{0}", castingActionType));
			}

			return Ln.Get(getCastingActionDescPath[castingActionType]);
		}


		public static string GetLobbySkillDesc(int skillCode)
		{
			if (!getLobbySkillDescPath.ContainsKey(skillCode))
			{
				getLobbySkillDescPath.Add(skillCode, string.Format("Skill/LobbyDesc/{0}", skillCode));
			}

			return Ln.Get(getLobbySkillDescPath[skillCode]);
		}


		public static string GetSkillDescKey(int groupCode)
		{
			if (!getSkillDescKeyPath.ContainsKey(groupCode))
			{
				getSkillDescKeyPath.Add(groupCode, string.Format("Skill/Group/Desc/{0}", groupCode));
			}

			return getSkillDescKeyPath[groupCode];
		}


		public static string GetSkillEvolutionName(int groupCode)
		{
			if (!getSkillEvolutionNamePath.ContainsKey(groupCode))
			{
				getSkillEvolutionNamePath.Add(groupCode, string.Format("Skill/Group/Evolution/Name/{0}", groupCode));
			}

			return Ln.Get(getSkillEvolutionNamePath[groupCode]);
		}


		public static string GetSkillEvolutionDesc(int groupCode)
		{
			if (!getSkillEvolutionDescPath.ContainsKey(groupCode))
			{
				getSkillEvolutionDescPath.Add(groupCode, string.Format("Skill/Group/Evolution/Desc/{0}", groupCode));
			}

			return Ln.Get(getSkillEvolutionDescPath[groupCode]);
		}


		public static string GetCharacterStateName(int groupCode)
		{
			if (!getCharacterStateNamePath.ContainsKey(groupCode))
			{
				getCharacterStateNamePath.Add(groupCode, string.Format("CharacterState/Group/Name/{0}", groupCode));
			}

			return Ln.Get(getCharacterStateNamePath[groupCode]);
		}


		public static string GetSkillName(int groupCode)
		{
			if (!getSkillNamePath.ContainsKey(groupCode))
			{
				getSkillNamePath.Add(groupCode, string.Format("Skill/Group/Name/{0}", groupCode));
			}

			return Ln.Get(getSkillNamePath[groupCode]);
		}


		public static string GetToastMessage(ToastMessageType toastMessageType)
		{
			if (!getToastMessagePath.ContainsKey((int) toastMessageType))
			{
				getToastMessagePath.Add((int) toastMessageType, string.Format("ToastMessage/{0}", toastMessageType));
			}

			return Ln.Get(getToastMessagePath[(int) toastMessageType]);
		}


		public static string GetSearchName(string str)
		{
			StringBuilder stringBuilder = new StringBuilder(str.Length);
			stringBuilder.Append(str.ToLowerInvariant());
			stringBuilder.Replace(" ", "");
			return stringBuilder.ToString();
		}


		public static string GetCostText(SkillCostType costType, int costKey, int cost)
		{
			StringBuilder stringBuilder = GameUtil.StringBuilder;
			stringBuilder.Clear();
			if (costType != SkillCostType.StateStack)
			{
				if (costType != SkillCostType.EquipWeaponStack)
				{
					if (!getSkillCostTypePath.ContainsKey(costType))
					{
						getSkillCostTypePath.Add(costType, string.Format("SkillCostType/{0}", costType));
					}

					stringBuilder.Append(Ln.Get(getSkillCostTypePath[costType]));
					if (cost > 0)
					{
						stringBuilder.Append(" ");
						stringBuilder.Append(cost);
					}
				}
				else
				{
					stringBuilder.Append(GetItemName(costKey));
					stringBuilder.Append(" ");
					stringBuilder.Append(cost);
				}
			}
			else
			{
				stringBuilder.Append(GetCharacterStateName(costKey));
				stringBuilder.Append(" ");
				stringBuilder.Append(cost);
			}

			return stringBuilder.ToString();
		}


		public static string GetServerTime(DateTime startTime, DateTime endTime)
		{
			startTime = startTime.ToLocalTime();
			endTime = endTime.ToLocalTime();
			string result;
			try
			{
				string[] array = Ln.Format("{0}년-{1}월-{2}일-{3:00}:{4:00}", startTime.Year, startTime.Month,
					startTime.Day, startTime.Hour, startTime.Minute).Split('-');
				string[] array2 = Ln.Format("{0}년-{1}월-{2}일-{3:00}:{4:00}", endTime.Year, endTime.Month, endTime.Day,
					endTime.Hour, endTime.Minute).Split('-');
				StringBuilder stringBuilder = GameUtil.StringBuilder;
				StringBuilder stringBuilder_ = GameUtil.StringBuilder_2;
				stringBuilder.Clear();
				stringBuilder_.Clear();
				bool flag = false;
				if (flag || startTime.Year != endTime.Year)
				{
					stringBuilder.Append(array[0]);
					stringBuilder.Append(" ");
					stringBuilder_.Append(array2[0]);
					stringBuilder_.Append(" ");
					flag = true;
				}

				if (flag || startTime.Month != endTime.Month || startTime.Day != endTime.Day)
				{
					stringBuilder.Append(array[1]);
					stringBuilder.Append(" ");
					stringBuilder_.Append(array2[1]);
					stringBuilder_.Append(" ");
					stringBuilder.Append(array[2]);
					stringBuilder.Append(" ");
					stringBuilder_.Append(array2[2]);
					stringBuilder_.Append(" ");
				}

				stringBuilder.Append(array[3]);
				stringBuilder.Append(" ");
				stringBuilder_.Append(array2[3]);
				stringBuilder_.Append(" ");
				result = string.Format("{0} ~ {1}", stringBuilder, stringBuilder_);
			}
			catch
			{
				result = string.Format("~ {0}-{1}-{2} {3:00}:{4:00}", endTime.Year, endTime.Month, endTime.Day,
					endTime.Hour, endTime.Minute);
			}

			return result;
		}


		public static string GetGiftMailExpireTimeText(DateTime currentTime, DateTime expireTime)
		{
			if (expireTime <= currentTime)
			{
				return "";
			}

			TimeSpan timeSpan = expireTime - currentTime;
			if (0 < (int) timeSpan.TotalDays)
			{
				return Ln.Format("선물함보관기간", (int) timeSpan.TotalDays, Ln.Get("일"));
			}

			if (0 < (int) timeSpan.TotalHours)
			{
				return Ln.Format("선물함보관기간", (int) timeSpan.TotalHours, Ln.Get("시"));
			}

			if (0 < (int) timeSpan.TotalMinutes)
			{
				return Ln.Format("선물함보관기간", (int) timeSpan.TotalMinutes, Ln.Get("분"));
			}

			return Ln.Format("선물함보관기간", 1, Ln.Get("분"));
		}


		public static string GetLobbyNoticeSlotTimeText(DateTime currentTime)
		{
			return Ln.Format("로비알림시간", currentTime.Year.ToString(), currentTime.Month.ToString("D2"),
				currentTime.Day.ToString("D2"));
		}


		public static string GetMaintenanceRemainTimeText(string key, DateTime now, DateTime endTime)
		{
			if (endTime <= now)
			{
				return Ln.Get("점검중");
			}

			TimeSpan timeSpan = endTime - now;
			if (0 < (int) timeSpan.TotalDays)
			{
				return Ln.Format(key, (int) timeSpan.TotalDays, Ln.Get("일"));
			}

			if (0 < (int) timeSpan.TotalHours)
			{
				return Ln.Format(key, (int) timeSpan.TotalHours, Ln.Get("시"));
			}

			if (0 < (int) timeSpan.TotalMinutes)
			{
				return Ln.Format(key, (int) timeSpan.TotalMinutes, Ln.Get("분"));
			}

			return Ln.Format(key, 1, Ln.Get("분"));
		}


		public static string GetRankRemainTimeText(int duration)
		{
			int num = 0;
			int num2;
			if (duration > 60)
			{
				num = duration / 60;
				num2 = duration % 60;
			}
			else
			{
				num2 = duration;
			}

			return string.Format("{0:D2}:{1:D2}", num, num2);
		}


		public static string GetBattleRecordPlayTime(string key, DateTime now, DateTime endTime)
		{
			if (endTime <= now)
			{
				return Ln.Get("점검중");
			}

			TimeSpan timeSpan = endTime - now;
			if (0 < (int) timeSpan.TotalDays)
			{
				return Ln.Format(key, (int) timeSpan.TotalDays, Ln.Get("일"));
			}

			if (0 < (int) timeSpan.TotalHours)
			{
				return Ln.Format(key, (int) timeSpan.TotalHours, Ln.Get("시"));
			}

			if (0 < (int) timeSpan.TotalMinutes)
			{
				return Ln.Format(key, (int) timeSpan.TotalMinutes, Ln.Get("분"));
			}

			return Ln.Format(key, 1, Ln.Get("분"));
		}


		public static string GetRankingSeasonRemainTime(DateTime now, DateTime endTime)
		{
			if (endTime <= now)
			{
				return "";
			}

			TimeSpan timeSpan = endTime - now;
			if (0 < (int) timeSpan.TotalDays)
			{
				return Ln.Format("일 시", timeSpan.Days, timeSpan.Hours);
			}

			if (0 < (int) timeSpan.TotalMinutes)
			{
				return Ln.Format("시 분", timeSpan.Hours, timeSpan.Minutes);
			}

			return Ln.Get("1분 미만");
		}


		public static string Get(MatchingMode matchingMode)
		{
			if (matchingMode == MatchingMode.Rank)
			{
				return Ln.Get("랭크 대전");
			}

			return Ln.Get("일반 대전");
		}


		public static string Get(MatchingTeamMode matchingTeamMode)
		{
			switch (matchingTeamMode)
			{
				case MatchingTeamMode.Solo:
					return Ln.Get("솔로");
				case MatchingTeamMode.Duo:
					return Ln.Get("듀오");
				case MatchingTeamMode.Squad:
					return Ln.Get("스쿼드");
				default:
					return Ln.Get("솔로");
			}
		}


		public static string GetMatchingTeamModeName(string mode)
		{
			if (mode == "Solo")
			{
				return Ln.Get("솔로");
			}

			if (mode == "Duo")
			{
				return Ln.Get("듀오");
			}

			if (!(mode == "Squad"))
			{
				return Ln.Get("솔로");
			}

			return Ln.Get("스쿼드");
		}
	}
}