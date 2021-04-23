using System;
using Neptune.Http;

namespace Blis.Common
{
	public static class IngameApi
	{
		public static Func<HttpRequest> RequestMatchingNormal(BattleOptionParam battleOptionParam)
		{
			return HttpRequestFactory.Post(ApiConstants.Url("/ingame/matching/normal", Array.Empty<object>()),
				battleOptionParam);
		}


		public static Func<HttpRequest> RequestMatchingRank(BattleOptionParam battleOptionParam)
		{
			return HttpRequestFactory.Post(ApiConstants.Url("/ingame/matching/rank", Array.Empty<object>()),
				battleOptionParam);
		}


		public static Func<HttpRequest> RequestCreatePrivateRoom(BattleOptionParam battleOptionParam, PlayMode playMode)
		{
			return HttpRequestFactory.Post(
				ApiConstants.Url(string.Format("/ingame/create/{0}", (int) playMode), Array.Empty<object>()),
				battleOptionParam);
		}


		public static Func<HttpRequest> RequestJoinPrivateRoom(BattleOptionParam battleOptionParam, string roomKey)
		{
			return HttpRequestFactory.Post(ApiConstants.Url("/ingame/join/" + roomKey, Array.Empty<object>()),
				battleOptionParam);
		}


		public class IngameRequestResult
		{
			public IngameServerInfo ingameServerInfo;

			public bool isChatRestricted;
		}
	}
}