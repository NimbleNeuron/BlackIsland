using System;
using Newtonsoft.Json;

namespace Blis.Common
{
	public class Collection
	{
		[JsonProperty("cc")] public readonly int collectionCode;

		[JsonProperty("cn")] public readonly long collectionNum;


		[JsonProperty("cd")] public readonly DateTime createDtm;


		[JsonProperty("ct")] public readonly GoodsType GoodsType;


		[JsonProperty("un")] public readonly long userNum;
	}
}