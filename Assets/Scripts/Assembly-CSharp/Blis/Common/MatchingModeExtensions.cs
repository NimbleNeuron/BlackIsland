using System;

namespace Blis.Common
{
	
	public static class MatchingModeExtensions
	{
		
		public static bool IsStandaloneMode(this MatchingMode matchingMode)
		{
			return matchingMode == MatchingMode.Dev || matchingMode == MatchingMode.Test;
		}

		
		public static bool IsTutorialMode(this MatchingMode matchingMode)
		{
			// return matchingMode - MatchingMode.Tutorial1 <= 4;
			return (int)matchingMode > 4;
		}

		
		public static MatchingMode ConvertToMatchingMode(this TutorialType tutorialType)
		{
			switch (tutorialType)
			{
				case TutorialType.BasicGuide:
					return MatchingMode.Tutorial1;
				case TutorialType.Trace:
					return MatchingMode.Tutorial2;
				case TutorialType.Hunt:
					return MatchingMode.Tutorial3;
				case TutorialType.PowerUp:
					return MatchingMode.Tutorial4;
				case TutorialType.FinalSurvival:
					return MatchingMode.Tutorial5;
				default:
					throw new ArgumentOutOfRangeException("tutorialType", tutorialType, null);
			}
		}

		
		public static TutorialType ConvertToTutorialType(this MatchingMode matchingMode)
		{
			switch (matchingMode)
			{
				case MatchingMode.Tutorial1:
					return TutorialType.BasicGuide;
				case MatchingMode.Tutorial2:
					return TutorialType.Trace;
				case MatchingMode.Tutorial3:
					return TutorialType.Hunt;
				case MatchingMode.Tutorial4:
					return TutorialType.PowerUp;
				case MatchingMode.Tutorial5:
					return TutorialType.FinalSurvival;
				default:
					throw new ArgumentOutOfRangeException("matchingMode", matchingMode, null);
			}
		}

		
		public static GameMode GetGameMode(this MatchingMode matchingMode)
		{
			switch (matchingMode)
			{
				case MatchingMode.Dev:
					return GameMode.TEST;
				case MatchingMode.Test:
					return GameMode.SINGLE;
				case MatchingMode.Normal:
					return GameMode.NORMAL;
				case MatchingMode.Rank:
					return GameMode.RANKING;
				case MatchingMode.Custom:
					return GameMode.CUSTOM;
				case MatchingMode.Tutorial1:
					return GameMode.TUTORIAL;
				case MatchingMode.Tutorial2:
					return GameMode.TUTORIAL;
				case MatchingMode.Tutorial3:
					return GameMode.TUTORIAL;
				case MatchingMode.Tutorial4:
					return GameMode.TUTORIAL;
				case MatchingMode.Tutorial5:
					return GameMode.TUTORIAL;
				default:
					throw new ArgumentOutOfRangeException("matchingMode", matchingMode, null);
			}
		}

		
		public static GameModeSub GetGameModeSub(this MatchingMode matchingMode)
		{
			switch (matchingMode)
			{
				case MatchingMode.Dev:
				case MatchingMode.Test:
				case MatchingMode.Normal:
				case MatchingMode.Rank:
				case MatchingMode.Custom:
					return GameModeSub.NORMAL;
				case MatchingMode.Tutorial1:
					return GameModeSub.TUTORIAL1;
				case MatchingMode.Tutorial2:
					return GameModeSub.TUTORIAL2;
				case MatchingMode.Tutorial3:
					return GameModeSub.TUTORIAL3;
				case MatchingMode.Tutorial4:
					return GameModeSub.TUTORIAL4;
				case MatchingMode.Tutorial5:
					return GameModeSub.TUTORIAL5;
				default:
					throw new ArgumentOutOfRangeException("matchingMode", matchingMode, null);
			}
		}

		
		public static bool IsTeamMode(this MatchingTeamMode mode)
		{
			return mode - MatchingTeamMode.Duo <= 1;
		}

		
		public static int MaxUserPerTeam(this MatchingTeamMode mode)
		{
			switch (mode)
			{
				case MatchingTeamMode.None:
					return 0;
				case MatchingTeamMode.Solo:
					return 1;
				case MatchingTeamMode.Duo:
					return 2;
				case MatchingTeamMode.Squad:
					return 3;
				default:
					throw new ArgumentOutOfRangeException("mode", mode, null);
			}
		}

		
		public static int MaxUserCapacity(this MatchingTeamMode mode)
		{
			switch (mode)
			{
				case MatchingTeamMode.None:
					return 0;
				case MatchingTeamMode.Solo:
					return 18;
				case MatchingTeamMode.Duo:
					return 18;
				case MatchingTeamMode.Squad:
					return 18;
				default:
					throw new ArgumentOutOfRangeException("mode", mode, null);
			}
		}

		
		public static int MaxTeamCount(this MatchingTeamMode mode)
		{
			return mode.MaxUserCapacity() / mode.MaxUserPerTeam();
		}
	}
}