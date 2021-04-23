using System;
using System.Collections.Generic;
using Neptune.Http;

namespace Blis.Common
{
	public class MissionApi
	{
		public static Func<HttpRequest> GetDailyMissions()
		{
			return HttpRequestFactory.Get(ApiConstants.Url("/mission/userMissions", Array.Empty<object>()));
		}


		public static Func<HttpRequest> GetRefreshMission(MissionRefreshParam missionRefreshParam)
		{
			return HttpRequestFactory.Post(ApiConstants.Url("/mission/changeMission", Array.Empty<object>()),
				missionRefreshParam);
		}


		public static Func<HttpRequest> GetMissionReward(MissionRewardParam missionRewardParam)
		{
			return HttpRequestFactory.Post(ApiConstants.Url("/mission/receivedReward", Array.Empty<object>()),
				missionRewardParam);
		}


		public class DailyMissionsResult
		{
			public bool changeable;

			public List<UserMission> userMissions;
		}


		public class RefreshMission
		{
			public UserMission userMission;
		}


		public class RewardMission
		{
			public int userLevel;

			public UserMission userMission;


			public int userNeedExp;
		}
	}
}