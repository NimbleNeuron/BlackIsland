using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Server
{
	
	public class BattleResultService : ServiceBase
	{
		
		public float GetMatchRating(int rank, MMRContext mmrContext)
		{
			int num = this.game.MatchingTeamMode.MaxTeamCount();
			int mmrBefore = mmrContext.mmrBefore;
			float avgMMR = this.game.PlayerCharacter.AvgMMR;
			float num2 = (float)num - BattleResultService.ADJUST_RANK_MAP[this.game.MatchingTeamMode][rank];
			Log.V("[MatchRating] Rank: {0}, TeamCount: {1}, AvgMMR: {2}, BeforeMMR: {3}", new object[]
			{
				rank.ToString(),
				num.ToString(),
				this.game.PlayerCharacter.AvgMMR.ToString(),
				mmrBefore.ToString()
			});
			float num3 = 1f / (1f + Mathf.Pow(10f, (avgMMR - (float)mmrBefore) / 800f));
			float result = (num2 - num3 * (float)(num - 1)) * BattleResultService.MATCH_RATING_WEIGHT[this.game.MatchingTeamMode];
			Log.V("[MatchRating] ScoreMatch: {0}, ExpectedScoreMatch: {1}, MatchRating: {2}", new object[]
			{
				num2.ToString(),
				num3.ToString(),
				result.ToString()
			});
			return result;
		}

		
		public string GetBattleResultTokenKey(long userNum)
		{
			if (MonoBehaviourInstance<GameService>.inst.MatchingMode == MatchingMode.Custom || MonoBehaviourInstance<GameService>.inst.MatchingMode.IsTutorialMode() || MonoBehaviourInstance<GameService>.inst.MatchingMode.GetGameMode() == GameMode.SINGLE)
			{
				return "";
			}
			return string.Join(":", new object[]
			{
				"BattleResult",
				userNum,
				this.game.BattleTokenKey
			});
		}

		
		private const string BATTLE_RESULT_KEY_PREFIX = "BattleResult";

		
		public const float RATING_COEFFCIENT = 800f;

		
		public static readonly Dictionary<MatchingTeamMode, float> MATCH_RATING_WEIGHT = new Dictionary<MatchingTeamMode, float>
		{
			{
				MatchingTeamMode.Solo,
				0.12f
			},
			{
				MatchingTeamMode.Duo,
				0.22f
			},
			{
				MatchingTeamMode.Squad,
				0.33f
			}
		};

		
		public static readonly Dictionary<MatchingTeamMode, float[]> ADJUST_RANK_MAP = new Dictionary<MatchingTeamMode, float[]>
		{
			{
				MatchingTeamMode.Solo,
				new float[]
				{
					0f,
					1f,
					2f,
					3f,
					4f,
					5f,
					6f,
					7f,
					8f,
					9f,
					10f,
					11f,
					12f,
					13f,
					14f,
					15f,
					16f,
					16.5f,
					17f
				}
			},
			{
				MatchingTeamMode.Duo,
				new float[]
				{
					0f,
					1f,
					2f,
					3f,
					4f,
					5f,
					6f,
					7f,
					7.5f,
					8f
				}
			},
			{
				MatchingTeamMode.Squad,
				new float[]
				{
					0f,
					1f,
					2f,
					3f,
					4f,
					4.5f,
					5f
				}
			}
		};
	}
}
