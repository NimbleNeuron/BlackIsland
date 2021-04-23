using System.Collections.Generic;

namespace Blis.Common
{
	public class HideNameSettingDic
	{
		private readonly Dictionary<long, HideNameSetting> hideNameSettings = new Dictionary<long, HideNameSetting>();


		public void SetHideName(Dictionary<int, MatchingTeamToken> teamMap)
		{
			if (hideNameSettings.Count > 0)
			{
				return;
			}

			foreach (KeyValuePair<int, MatchingTeamToken> keyValuePair in teamMap)
			{
				foreach (KeyValuePair<long, MatchingTeamMemberToken> keyValuePair2 in keyValuePair.Value.teamMembers)
				{
					hideNameSettings.Add(keyValuePair2.Key, new HideNameSetting(keyValuePair2.Value.hideNameFromEnemy));
				}
			}
		}


		public HideNameSetting GetUserHideNameSetting(long userId)
		{
			if (hideNameSettings.ContainsKey(userId))
			{
				return hideNameSettings[userId];
			}

			return new HideNameSetting(false);
		}
	}
}