using Newtonsoft.Json;

namespace Blis.Common
{
	public class CustomGameSlot
	{
		[JsonProperty("cc")] public readonly int characterCode;
		[JsonProperty("ib")] public readonly bool isBot;
		[JsonProperty("nn")] public readonly string nickname;
		[JsonProperty("si")] public readonly int slotIndex;
		[JsonProperty("s")] public readonly string steamId;
		[JsonProperty("tn")] public readonly int teamNumber;
		[JsonProperty("un")] public readonly long userNum;

		public CustomGameSlot(int slotIndex, int teamNumber, long userNum, string nickname, string steamId,
			int characterCode, bool isBot)
		{
			this.slotIndex = slotIndex;
			this.teamNumber = teamNumber;
			this.userNum = userNum;
			this.nickname = nickname;
			this.steamId = steamId;
			this.characterCode = characterCode;
			this.isBot = isBot;
		}


		public bool IsEmptySlot()
		{
			return userNum == 0L;
		}
	}
}