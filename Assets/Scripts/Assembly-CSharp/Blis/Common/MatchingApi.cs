using System;
using System.Collections.Generic;
using Neptune.Http;
using Newtonsoft.Json;

namespace Blis.Common
{
	public class MatchingApi
	{
		public static Func<HttpRequest> BeforeMatching(BeforeMatchingParam param)
		{
			return HttpRequestFactory.Post(ApiConstants.Url("/matching/before", Array.Empty<object>()), param);
		}


		public static Func<HttpRequest> Enter(MatchingParam matchingParam)
		{
			return HttpRequestFactory.Post(ApiConstants.Url("/matching/enter", Array.Empty<object>()), matchingParam);
		}


		public static Func<HttpRequest> EnterSingle()
		{
			return HttpRequestFactory.Post(ApiConstants.Url("/matching/enter/single", Array.Empty<object>()), null);
		}


		public static Func<HttpRequest> EnterCustom(MatchingParam matchingParam)
		{
			return HttpRequestFactory.Post(ApiConstants.Url("/matching/enter/custom", Array.Empty<object>()),
				matchingParam);
		}


		public static Func<HttpRequest> GetMatchingPenalty(MatchingMode matchingMode)
		{
			return HttpRequestFactory.Get(ApiConstants.Url(
				string.Format("/matching/matchingPenalty/?matchingMode={0}", matchingMode), Array.Empty<object>()));
		}


		public static Func<HttpRequest> GetMatchingDeclineCount(MatchingMode matchingMode)
		{
			return HttpRequestFactory.Get(ApiConstants.Url(
				string.Format("/matching/matchingDecline?matchingMode={0}", matchingMode), Array.Empty<object>()));
		}


		public class EnterResult
		{
			public string chinaMatchingHost;


			public string matchingHost;


			public string matchingTokenKey;

			public string GetMatchingHost()
			{
				if (!GlobalUserData.gaap)
				{
					return matchingHost;
				}

				return chinaMatchingHost;
			}
		}


		public class MatchingPenaltyInfo
		{
			public readonly string nickname;


			public readonly DateTime until;


			public readonly long userNum;

			[JsonConstructor]
			public MatchingPenaltyInfo(long userNum, string nickname, long until)
			{
				this.userNum = userNum;
				this.nickname = nickname;
				this.until = GameUtil.ConvertFromUnixTimestamp(until / 1000L).AddSeconds(1.0);
			}
		}


		public class BeforeMatchingResult
		{
			public List<MatchingPenaltyInfo> matchingPenaltyInfo;
		}


		public class EnterSingleResult
		{
			public string banCharacters;

			public string freeCharacters;
		}


		public class EnterCustomResult
		{
			public string chinaMatchingHost;


			public string customGameTokenKey;


			public string matchingHost;

			public string GetMatchingHost()
			{
				if (!GlobalUserData.gaap)
				{
					return matchingHost;
				}

				return chinaMatchingHost;
			}
		}


		public class GetMatchingPenaltyResult
		{
			public DateTime matchingPenaltyTime;

			public GetMatchingPenaltyResult(long matchingPenaltyTime)
			{
				this.matchingPenaltyTime =
					GameUtil.ConvertFromUnixTimestamp(matchingPenaltyTime / 1000L).AddSeconds(1.0);
			}
		}


		public class GetMatchingDeclineCountResult
		{
			public int matchingDeclineCount;


			public DateTime matchingPenaltyTime;

			public GetMatchingDeclineCountResult(int matchingDeclineCount, long matchingPenaltyTime)
			{
				this.matchingDeclineCount = matchingDeclineCount;
				this.matchingPenaltyTime =
					GameUtil.ConvertFromUnixTimestamp(matchingPenaltyTime / 1000L).AddSeconds(1.0);
			}
		}
	}
}