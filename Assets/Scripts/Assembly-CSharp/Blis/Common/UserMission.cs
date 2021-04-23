using Newtonsoft.Json;

namespace Blis.Common
{
	public class UserMission
	{
		[JsonProperty("cc")] public readonly int missionChangeCount;


		[JsonProperty("mc")] public readonly int missionCode;


		[JsonProperty("ms")] public readonly int missionSeq;


		[JsonProperty("pc")] public readonly int progressCount;

		public UserMission(int missionCode, int missionSeq, int progressCount, int missionChangeCount)
		{
			this.missionCode = missionCode;
			this.missionSeq = missionSeq;
			this.progressCount = progressCount;
			this.missionChangeCount = missionChangeCount;
		}
	}
}