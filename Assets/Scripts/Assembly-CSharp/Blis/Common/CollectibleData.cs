using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Blis.Common
{
	public class CollectibleData
	{
		[JsonConverter(typeof(StringEnumConverter))]
		public readonly CastingActionType castingActionType;


		public readonly int code;


		public readonly int cooldown;


		public readonly int dropCount;


		public readonly int itemCode;

		[JsonConstructor]
		public CollectibleData(int code, int itemCode, int dropCount, int cooldown, CastingActionType castingActionType)
		{
			this.code = code;
			this.itemCode = itemCode;
			this.dropCount = dropCount;
			this.cooldown = cooldown;
			this.castingActionType = castingActionType;
		}
	}
}