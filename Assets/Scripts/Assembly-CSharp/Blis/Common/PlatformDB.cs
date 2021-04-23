using System.Collections.Generic;
using System.Linq;

namespace Blis.Common
{
	public class PlatformDB
	{
		public List<ServerRegionData> serverRegions;

		public void SetData<T>(List<T> data)
		{
			if (typeof(T) == typeof(ServerRegionData))
			{
				serverRegions = data.Cast<ServerRegionData>().ToList<ServerRegionData>();
			}
		}


		public string GetIP(MatchingRegion region)
		{
			ServerRegionData serverRegionData = serverRegions.Find(x => x.region == region);
			if (serverRegionData != null)
			{
				return serverRegionData.pingIP;
			}

			Log.E(string.Format("PlatformDB.GetIP : {0} fail.", region));
			return null;
		}
	}
}