using System;
using System.Collections.Generic;
using System.Linq;
using Blis.Common;
using Blis.Common.Utils;

namespace Blis.Client
{
	public static class InfoService
	{
		private static BattleOverviewResult battleOverview;


		private static List<BattleUserGame> battleUserGames;


		private static BattleUser leagueBattleUser;


		private static Dictionary<RankingTierGrade, List<RankingUser>> leaguePlayerList;


		private static List<RankingTier> rankingTiers;


		private static Dictionary<RankingTierType, List<RankingTier>> rankInfos;

		public static void GetBattleOverview(Action callback, long userNum, int seasonId)
		{
			if (MonoBehaviourInstance<LobbyUI>.inst.ServerIndicator != null &&
			    !MonoBehaviourInstance<LobbyUI>.inst.ServerIndicator.gameObject.activeSelf)
			{
				MonoBehaviourInstance<LobbyUI>.inst.ServerIndicator.gameObject.SetActive(true);
			}

			RequestDelegate.request<BattleOverviewResult>(LobbyApi.GetBattleOverview(userNum, seasonId), false,
				delegate(RequestDelegateError err, BattleOverviewResult res)
				{
					if (err != null)
					{
						MonoBehaviourInstance<Popup>.inst.Error(err.errorType.ToString());
						return;
					}

					battleOverview = res;
					Action callback2 = callback;
					if (callback2 == null)
					{
						return;
					}

					callback2();
				});
		}


		public static void GetBattleGames(Action callback, long userNum)
		{
			if (MonoBehaviourInstance<LobbyUI>.inst.ServerIndicator != null &&
			    !MonoBehaviourInstance<LobbyUI>.inst.ServerIndicator.gameObject.activeSelf)
			{
				MonoBehaviourInstance<LobbyUI>.inst.ServerIndicator.gameObject.SetActive(true);
			}

			RequestDelegate.request<BattleUserGameResult>(LobbyApi.GetBattleGames(userNum), false,
				delegate(RequestDelegateError err, BattleUserGameResult res)
				{
					if (err != null)
					{
						MonoBehaviourInstance<Popup>.inst.Error(err.errorType.ToString());
						return;
					}

					battleUserGames = res.battleUserGames;
					Action callback2 = callback;
					if (callback2 == null)
					{
						return;
					}

					callback2();
				});
		}


		public static void GetUserRankings(Action callback, MatchingTeamMode matchingTeamMode)
		{
			if (MonoBehaviourInstance<LobbyUI>.inst.ServerIndicator != null &&
			    !MonoBehaviourInstance<LobbyUI>.inst.ServerIndicator.gameObject.activeSelf)
			{
				MonoBehaviourInstance<LobbyUI>.inst.ServerIndicator.gameObject.SetActive(true);
			}

			RequestDelegate.request<UserRankingsResult>(LobbyApi.GetUserRankings(matchingTeamMode), false,
				delegate(RequestDelegateError err, UserRankingsResult res)
				{
					if (err != null)
					{
						MonoBehaviourInstance<Popup>.inst.Error(err.errorType.ToString());
						return;
					}

					leagueBattleUser = res.battleUser;
					leaguePlayerList = res.tierRankingUsers;
					foreach (List<RankingUser> list in leaguePlayerList.Values)
					{
						if (list != null && list.Count > 0)
						{
							list.Sort((x, y) => y.mmr.CompareTo(x.mmr));
						}
					}

					Action callback2 = callback;
					if (callback2 == null)
					{
						return;
					}

					callback2();
				});
		}


		public static void GetRankingSeasonTiers(Action callback)
		{
			RequestDelegate.request<RankingSeasonTiersResult>(LobbyApi.GetRankingSeasonTiers(), false,
				delegate(RequestDelegateError err, RankingSeasonTiersResult res)
				{
					if (err != null)
					{
						MonoBehaviourInstance<Popup>.inst.Error(err.errorType.ToString());
						return;
					}

					rankingTiers = res.rankingTiers;
					SetRankingTier();
					Action callback2 = callback;
					if (callback2 == null)
					{
						return;
					}

					callback2();
				});
		}


		public static BattleUser GetOverviewBattleUser(MatchingTeamMode matchingTeamMode)
		{
			BattleUser result;
			if (battleOverview != null && battleOverview.battleUserInfo != null &&
			    battleOverview.battleUserInfo.TryGetValue(matchingTeamMode, out result))
			{
				return result;
			}

			return null;
		}


		public static List<string> GetRankingSeasonList()
		{
			if (battleOverview != null && battleOverview.seasons != null && battleOverview.seasons.Count > 0)
			{
				return (from x in battleOverview.seasons
					select Ln.Get(x.title)).ToList<string>();
			}

			return new List<string>();
		}


		public static int GetRankingSeasonId(int index)
		{
			if (battleOverview != null && battleOverview.seasons != null && battleOverview.seasons.Count > 0)
			{
				return battleOverview.seasons[index].id;
			}

			return 0;
		}


		public static string GetRemainRankingSeasonTime(int seasonId)
		{
			RankingSeason rankingSeason = battleOverview.seasons.Find(x => x.id == seasonId);
			return LnUtil.GetRankingSeasonRemainTime(DateTime.Now.ToUniversalTime(), rankingSeason.endDtm);
		}


		public static bool GetBattleUserGamesCache()
		{
			return battleUserGames != null && battleUserGames.Count > 0;
		}


		public static int GetBattleUserGamesCount()
		{
			if (GetBattleUserGamesCache())
			{
				return battleUserGames.Count;
			}

			return 0;
		}


		public static BattleUserGame GetBattleUserGame(int index)
		{
			if (GetBattleUserGamesCache())
			{
				return battleUserGames[index];
			}

			return null;
		}


		public static BattleUser GetLeagueBattleUser()
		{
			if (leagueBattleUser != null)
			{
				return leagueBattleUser;
			}

			return null;
		}


		public static bool IsLeaguePlayerList()
		{
			return leaguePlayerList != null && leaguePlayerList.Count > 0;
		}


		public static int GetLeaguePlayerListCount(RankingTierGrade tierGrade)
		{
			if (IsLeaguePlayerList() && leaguePlayerList.ContainsKey(tierGrade) && leaguePlayerList[tierGrade] != null)
			{
				return leaguePlayerList[tierGrade].Count;
			}

			return 0;
		}


		public static List<RankingTierGrade> GetLeaguePlayerListKeys()
		{
			if (IsLeaguePlayerList())
			{
				return (from x in leaguePlayerList.Keys
					orderby x
					select x).ToList<RankingTierGrade>();
			}

			return new List<RankingTierGrade>();
		}


		public static RankingUser GetRankingUser(RankingTierGrade tierGrade, int index)
		{
			if (IsLeaguePlayerList() && leaguePlayerList.ContainsKey(tierGrade) && leaguePlayerList[tierGrade] != null)
			{
				return leaguePlayerList[tierGrade][index];
			}

			return new RankingUser();
		}


		public static bool IsRankingTiers()
		{
			return rankingTiers != null && rankingTiers.Count > 0;
		}


		public static int GetRankingTiersCount()
		{
			if (IsRankingTiers())
			{
				return rankingTiers.Count;
			}

			return 0;
		}


		public static RankingTier GetRankingTier(int index)
		{
			if (IsRankingTiers())
			{
				return rankingTiers[index];
			}

			return new RankingTier();
		}


		public static void SetRankingTier()
		{
			rankInfos = new Dictionary<RankingTierType, List<RankingTier>>();
			for (int i = 0; i < GetRankingTiersCount(); i++)
			{
				RankingTier rankingTier = GetRankingTier(i);
				if (!rankInfos.ContainsKey(rankingTier.tierType))
				{
					rankInfos.Add(rankingTier.tierType, new List<RankingTier>());
				}

				rankInfos[rankingTier.tierType].Add(rankingTier);
			}
		}


		public static int GetRankingMinMMR(RankingTierType tier, RankingTierGrade grade)
		{
			if (rankInfos.ContainsKey(tier))
			{
				RankingTier rankingTier = rankInfos[tier].Find(p => p.tierGrade == grade);
				int? num = rankingTier != null ? new int?(rankingTier.minMmr) : null;
				if (num != null)
				{
					return num.Value;
				}
			}

			return 0;
		}
	}
}