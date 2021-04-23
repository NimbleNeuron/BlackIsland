using System.Collections.Generic;
using Newtonsoft.Json;

namespace Blis.Common
{
	public class CustomGameRoom
	{
		[JsonProperty("bnn")] public readonly long botNameNum;
		[JsonProperty("cgk")] public readonly string customGameKey;
		[JsonProperty("ioa")] public readonly bool isOnAcceleration;
		[JsonProperty("mr")] public readonly MatchingRegion matchingRegion;
		[JsonProperty("ol")] public readonly List<CustomGameSlot> observerSlotList;
		[JsonProperty("onn")] public readonly long ownerUserNum;
		[JsonProperty("sl")] public readonly List<CustomGameSlot> slotList;
		[JsonProperty("tm")] public readonly MatchingTeamMode teamMode;
	}
}