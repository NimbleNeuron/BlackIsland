using System.Collections.Generic;
using System.Linq;

namespace Blis.Common
{
	public class MissionDB
	{
		private List<MissionData> userDailyMissionDatas;

		public void SetData<T>(List<T> data)
		{
			if (typeof(T) == typeof(MissionData))
			{
				userDailyMissionDatas = data.Cast<MissionData>().ToList<MissionData>();
			}
		}


		public MissionData GetMissionData(int key)
		{
			return userDailyMissionDatas.Find(x => x.key == key);
		}


		public MissionData GetMissionData(int code, int seq)
		{
			return userDailyMissionDatas.Find(x => x.code == code && x.seq == seq);
		}
	}
}