using System.Collections.Generic;
using Blis.Client;
using Blis.Client.UI;
using Blis.Common;


public static class GlobalUserData
{
	
	public static string GetStringMatchingModeDetail()
	{
		switch (GlobalUserData.matchingMode)
		{
		case MatchingMode.Test:
			switch (GlobalUserData.botDifficulty)
			{
			case BotDifficulty.EASY:
				return Ln.Get("싱글 대전") + "(" + Ln.Get("SingleEasy") + ")";
			case BotDifficulty.NORMAL:
				return Ln.Get("싱글 대전") + "(" + Ln.Get("SingleNormal") + ")";
			case BotDifficulty.HARD:
				return Ln.Get("싱글 대전") + "(" + Ln.Get("SingleHard") + ")";
			default:
				return string.Empty;
			}
		case MatchingMode.Normal:
			switch (GlobalUserData.matchingTeamMode)
			{
			case MatchingTeamMode.Solo:
				return Ln.Get("일반 대전(솔로)");
			case MatchingTeamMode.Duo:
				return Ln.Get("일반 대전(듀오)");
			case MatchingTeamMode.Squad:
				return Ln.Get("일반 대전(스쿼드)");
			default:
				return string.Empty;
			}
		case MatchingMode.Rank:
			switch (GlobalUserData.matchingTeamMode)
			{
			case MatchingTeamMode.Solo:
				return Ln.Get("랭크 대전(솔로)");
			case MatchingTeamMode.Duo:
				return Ln.Get("랭크 대전(듀오)");
			case MatchingTeamMode.Squad:
				return Ln.Get("랭크 대전(스쿼드)");
			default:
				return string.Empty;
			}
		case MatchingMode.Custom:
			return Ln.Get("커스텀 모드");
		default:
			return string.Empty;
		}
	}

	
	public static string GetStringMatchingMode()
	{
		switch (GlobalUserData.matchingMode)
		{
		case MatchingMode.Test:
			return Ln.Get("싱글 대전");
		case MatchingMode.Normal:
			return Ln.Get("일반 대전");
		case MatchingMode.Rank:
			return Ln.Get("랭크 대전");
		case MatchingMode.Custom:
			return Ln.Get("커스텀 모드");
		default:
			return string.Empty;
		}
	}

	
	public static bool IsStandaloneMode()
	{
		return GlobalUserData.matchingMode.IsStandaloneMode();
	}

	
	public static bool IsCustomMatching()
	{
		return GlobalUserData.matchingMode == MatchingMode.Custom;
	}

	
	public static bool IsTeamMode()
	{
		return GlobalUserData.matchingTeamMode == MatchingTeamMode.Duo || GlobalUserData.matchingTeamMode == MatchingTeamMode.Squad;
	}

	
	public static List<UserMission> userDailyMissions = new List<UserMission>();

	
	public static Dictionary<int, PlayerInfo> dicPlayerResults = new Dictionary<int, PlayerInfo>();

	
	public static int myObjectId;

	
	public static int myPlayTime;

	
	public static int myLevel;

	
	public static int myTeamNumber;

	
	public static bool IsPlayer;

	
	public static string battleResultKey = string.Empty;

	
	public static bool tutorialClearFlag = false;

	
	public static int tutorialClearCode = 0;

	
	public static MatchingRegion matchingRegion;

	
	public static MatchingMode matchingMode;

	
	public static MatchingTeamMode matchingTeamMode;

	
	public static BotDifficulty botDifficulty;

	
	public static string customGameRoomKey;

	
	public static List<string> finishedBattleTokenKeys = new List<string>();

	
	public static long lastGameId = 0L;

	
	public static bool gaap;

	
	public static bool showGrade;
}
