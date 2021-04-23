using Newtonsoft.Json;

namespace Blis.Common
{
	public class BotNickNameData
	{
		public readonly string botNickName;


		public readonly string serverRegion;

		[JsonConstructor]
		public BotNickNameData(string botNickName, string serverRegion)
		{
			this.botNickName = botNickName;
			this.serverRegion = serverRegion;
		}
	}
}