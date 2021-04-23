using System;
using Newtonsoft.Json;

namespace Blis.Common
{
	public class Goods
	{
		[JsonProperty("a")] public readonly int amount;


		[JsonProperty("gt")] public readonly GoodsType goodsType;


		[JsonProperty("st")] public readonly string subType;

		[JsonConstructor]
		public Goods(GoodsType goodsType, string subType, int amount)
		{
			this.goodsType = goodsType;
			this.subType = subType;
			this.amount = amount;
		}


		public int GetIntSubType()
		{
			return Convert.ToInt32(subType);
		}
	}
}