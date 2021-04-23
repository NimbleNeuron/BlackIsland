using System;
using Newtonsoft.Json;

namespace Blis.Common
{
	public class Character
	{
		[JsonProperty("cc")] public readonly int characterCode;


		[JsonProperty("cd")] [JsonConverter(typeof(MicrosecondEpochConverter))]
		public readonly DateTime createDtm;


		[JsonProperty("lpd")] [JsonConverter(typeof(MicrosecondEpochConverter))]
		public readonly DateTime lastPlayDtm;


		[JsonProperty("lsc")] public readonly int lastSkinCode;


		[JsonProperty("un")] public readonly long userNum;

		public Character(long userNum, int characterCode, int lastSkinCode, DateTime dtm, DateTime lastPlayDtm)
		{
			this.userNum = userNum;
			this.characterCode = characterCode;
			this.lastSkinCode = lastSkinCode;
			createDtm = dtm;
			this.lastPlayDtm = lastPlayDtm;
		}
	}
}