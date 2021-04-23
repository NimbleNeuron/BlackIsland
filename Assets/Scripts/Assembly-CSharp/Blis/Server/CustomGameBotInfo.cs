using MessagePack;
using Newtonsoft.Json;

namespace Blis.Server
{
	
	[MessagePackObject(true)]
	public class CustomGameBotInfo
	{
		
		public CustomGameBotInfo(int teamNumber, int characterCode, string nickname)
		{
			this.teamNumber = teamNumber;
			this.characterCode = characterCode;
			this.nickname = nickname;
		}

		
		[JsonProperty("tn")]
		public int teamNumber;

		
		[JsonProperty("cc")]
		public int characterCode;

		
		[JsonProperty("nn")]
		public string nickname;
	}
}
