using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Blis.Common
{
	public class RecommendItem
	{
		public readonly int characterCode;


		public readonly int code;


		public readonly int itemCode;


		[JsonConverter(typeof(StringEnumConverter))]
		public readonly MasteryType mastery;


		[JsonConverter(typeof(StringEnumConverter))]
		public readonly RecommendItemType recommendItemType;

		public RecommendItem(int code, int characterCode, MasteryType mastery, RecommendItemType recommendItemType,
			int itemCode)
		{
			this.code = code;
			this.characterCode = characterCode;
			this.mastery = mastery;
			this.recommendItemType = recommendItemType;
			this.itemCode = itemCode;
		}
	}
}