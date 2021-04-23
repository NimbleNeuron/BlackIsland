using Newtonsoft.Json;

namespace Blis.Server
{
	
	public class BattleChatMember
	{
		
		[JsonProperty]
		public int team;

		
		[JsonProperty]
		public long userNum;

		
		[JsonProperty]
		public string nickname;
	}
}
