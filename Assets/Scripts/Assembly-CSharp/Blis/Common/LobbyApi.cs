using System;
using System.Collections.Generic;
using Blis.Client;
using Neptune.Http;

namespace Blis.Common
{
	public static class LobbyApi
	{
		public enum GiftMailType
		{
			NONE,
			ADMIN,
			EVENT_REWARD,
			NOTICE
		}

		public enum ItemType
		{
			ASSET = 1,
			CHARACTER,
			WEAPON_ROUTE_SLOT,
			SKIN,
			EMOTION
		}

		public enum NoticeType
		{
			NONE,
			LOBBY,
			BIG_BANNER,
			SMALL_BANNER,
			PLAY_BANNER
		}

		public static Func<HttpRequest> EnterLobby()
		{
			return HttpRequestFactory.Get(ApiConstants.Url("/lobby/enter", Array.Empty<object>()));
		}

		public static Func<HttpRequest> Maintenance()
		{
			return HttpRequestFactory.Get(ApiConstants.Url("/lobby/maintenance", Array.Empty<object>()));
		}

		public static Func<HttpRequest> authenticate(AuthParam param)
		{
			return HttpRequestFactory.Post(ApiConstants.Url("/users/authenticate", Array.Empty<object>()), param);
		}

		public static Func<HttpRequest> setupNickname(string nickname)
		{
			return HttpRequestFactory.Post(ApiConstants.Url("/users/nickname", Array.Empty<object>()), nickname);
		}

		public static Func<HttpRequest> GetLobbyNotice(SupportLanguage languageType)
		{
			return HttpRequestFactory.Get(ApiConstants.Url(
				string.Format("/notice/lobby?supportLanguage={0}", (int) languageType), Array.Empty<object>()));
		}

		public static Func<HttpRequest> GetGiftMail(SupportLanguage languageType)
		{
			return HttpRequestFactory.Get(ApiConstants.Url(
				string.Format("/giftMail/?supportLanguage={0}", (int) languageType), Array.Empty<object>()));
		}

		public static Func<HttpRequest> GetGiftMail(long giftMailId)
		{
			return HttpRequestFactory.Post(ApiConstants.Url("/giftMail/receive", Array.Empty<object>()), giftMailId);
		}


		public static Func<HttpRequest> GetLobbyBannerData(NoticeType noticeType, SupportLanguage languageType)
		{
			return HttpRequestFactory.Get(ApiConstants.Url(
				string.Format("/notice/?type={0}&supportLanguage={1}", noticeType, (int) languageType),
				Array.Empty<object>()));
		}


		public static Func<HttpRequest> GetBannerImage(string url)
		{
			return HttpRequestFactory.Get(url);
		}


		public static Func<HttpRequest> PlayerReport(ReportParam reportParam)
		{
			return HttpRequestFactory.Post(ApiConstants.Url("/users/report/", Array.Empty<object>()), reportParam);
		}


		public static Func<HttpRequest> GetTwitchDrops()
		{
			return HttpRequestFactory.Get(ApiConstants.Url("/twitch/", Array.Empty<object>()));
		}


		public static Func<HttpRequest> GetTwitchDropsReward(TwitchDropsRewardParam param)
		{
			return HttpRequestFactory.Post(ApiConstants.Url("/twitch/receive", Array.Empty<object>()), param);
		}


		public static Func<HttpRequest> GetBattleOverview(long userNum, int seasonId)
		{
			return HttpRequestFactory.Get(ApiConstants.Url(string.Format("/battle/overview/{0}/{1}", userNum, seasonId),
				Array.Empty<object>()));
		}


		public static Func<HttpRequest> GetBattleGames(long userNum)
		{
			return HttpRequestFactory.Get(ApiConstants.Url(string.Format("/battle/games/{0}", userNum),
				Array.Empty<object>()));
		}


		public static Func<HttpRequest> GetUserRankings(MatchingTeamMode matchingTeamMode)
		{
			return HttpRequestFactory.Get(ApiConstants.Url(
				string.Format("/ranking/userRankings/?matchingTeamMode={0}", matchingTeamMode), Array.Empty<object>()));
		}


		public static Func<HttpRequest> GetRankingSeasonTiers()
		{
			return HttpRequestFactory.Get(ApiConstants.Url("/ranking/seasonTiers", Array.Empty<object>()));
		}


		public class EnterLobbyResult
		{
			public List<BattleUser> battleUsers;
			public bool benefitByKakaoPcCafe;
			public List<Character> characterList;
			public List<UserEmotion> emotionList;
			public int freeNicknameChange;
			public List<int> freeRotation;
			public bool maintenance;
			public List<MatchingRegionSchedule> matchingRegionSchedule;
			public MatchingResult matchingResult;
			public DateTime normalMatchingPenaltyTime;
			public RankingSeason rankingSeason;
			public DateTime rankMatchingPenaltyTime;
			public DateTime serverTime;
			public List<Skin> skinList;
			public User user;

			public EnterLobbyResult(bool maintenance, List<Character> characterList, List<Skin> skinList,
				List<UserEmotion> emotionList, List<int> freeRotation,
				List<MatchingRegionSchedule> matchingRegionSchedule, List<BattleUser> battleUsers,
				RankingSeason rankingSeason, long normalMatchingPenaltyTime, long rankMatchingPenaltyTime,
				long serverTime, int freeNicknameChange, bool twitchAccountConnected, bool benefitByKakaoPcCafe)
			{
				this.maintenance = maintenance;
				this.characterList = characterList;
				this.skinList = skinList;
				this.emotionList = emotionList;
				this.freeRotation = freeRotation;
				this.matchingRegionSchedule = matchingRegionSchedule;
				this.battleUsers = battleUsers;
				this.rankingSeason = rankingSeason;
				this.normalMatchingPenaltyTime = GameUtil.ConvertFromUnixTimestamp(normalMatchingPenaltyTime / 1000L);
				this.rankMatchingPenaltyTime = GameUtil.ConvertFromUnixTimestamp(rankMatchingPenaltyTime / 1000L);
				this.serverTime = GameUtil.ConvertFromUnixTimestamp(serverTime);
				this.freeNicknameChange = freeNicknameChange;
				this.benefitByKakaoPcCafe = benefitByKakaoPcCafe;
			}
		}

		public class LobbyNoticeResult
		{
			public List<LobbyNotice> lobbyNoticeResults;
		}

		public class LobbyNotice
		{
			public string bannerUrl;
			public DateTime endDtm;
			public int id;
			public string message;
			public int order;
			public DateTime startDtm;
			public SupportLanguage supportLanguage;
			public string targetUrl;
			public string title;
			public NoticeType type;

			public LobbyNotice(int id, NoticeType type, int order, long startDtm, long endDtm,
				SupportLanguage supportLanguage, string title, string message, string bannerUrl, string targetUrl)
			{
				this.id = id;
				this.type = type;
				this.order = order;
				this.startDtm = GameUtil.ConvertFromUnixTimestamp(startDtm / 1000L);
				this.endDtm = GameUtil.ConvertFromUnixTimestamp(endDtm / 1000L);
				this.supportLanguage = supportLanguage;
				this.title = title;
				this.message = message;
				this.bannerUrl = bannerUrl;
				this.targetUrl = targetUrl;
			}
		}

		public class GiftMailResult
		{
			public List<UserGiftMail> userGiftMails;
		}


		public class UserGiftMail
		{
			public DateTime expireDtm;
			public long id;
			public string message;
			public string title;
			public GiftMailType type;

			public UserGiftMail(long id, GiftMailType type, string title, string message, long expireDtm)
			{
				this.id = id;
				this.type = type;
				this.title = title;
				this.message = message;
				this.expireDtm = GameUtil.ConvertFromUnixTimestamp(expireDtm / 1000L);
			}
		}
		
		public class GiftMailItemResult
		{
			public ItemContainer itemContainer;
		}

		public class ItemContainer
		{
			public List<Item> items;
		}
		
		public class Item
		{
			public int freeAmount;
			public int itemCode;
			public int paidAmount;

			public ItemType type;

			public AssetType GetAssetType()
			{
				if (type == ItemType.ASSET)
				{
					return (AssetType) itemCode;
				}

				return AssetType.NONE;
			}

			public int GetAssetItemAmount()
			{
				return freeAmount + paidAmount;
			}
		}

		public class TwitchAccountConnectedResult
		{
			public bool hasAuthToken;
		}
	}
}