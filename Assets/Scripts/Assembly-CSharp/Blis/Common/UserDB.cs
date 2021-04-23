using System.Collections.Generic;
using System.Linq;

namespace Blis.Common
{
	public class UserDB
	{
		private List<UserLevelData> userLevelDatas;

		public void SetData<T>(List<T> data)
		{
			if (typeof(T) == typeof(UserLevelData))
			{
				userLevelDatas = data.Cast<UserLevelData>().ToList<UserLevelData>();
			}
		}

		public int GetRewardAP(int level)
		{
			UserLevelData userLevelData = userLevelDatas.Find(x => x.level == level);
			if (userLevelData != null)
			{
				return userLevelData.rewardAcoin;
			}

			return 0;
		}

		public int GetNeedXP(int level)
		{
			UserLevelData userLevelData = userLevelDatas.Find(x => x.level == level);
			if (userLevelData != null)
			{
				return userLevelData.needExp;
			}

			return 0;
		}

		public int GetAccmulateXP(int level)
		{
			UserLevelData userLevelData = userLevelDatas.Find(x => x.level == level);
			if (userLevelData != null)
			{
				return userLevelData.accumulateExp;
			}

			return 0;
		}
	}
}