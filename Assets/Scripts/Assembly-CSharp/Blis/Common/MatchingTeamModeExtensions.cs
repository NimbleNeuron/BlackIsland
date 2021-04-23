using System;

namespace Blis.Common
{
	
	public static class MatchingTeamModeExtensions
	{
		
		public static int GetMemberCountPerTeam(this MatchingTeamMode matchingTeamMode)
		{
			switch (matchingTeamMode)
			{
				case MatchingTeamMode.Solo:
					return 1;
				case MatchingTeamMode.Duo:
					return 2;
				case MatchingTeamMode.Squad:
					return 3;
				default:
					throw new ArgumentOutOfRangeException("matchingTeamMode", matchingTeamMode, null);
			}
		}
	}
}