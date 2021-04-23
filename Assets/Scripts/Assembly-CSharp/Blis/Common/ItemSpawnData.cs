using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Blis.Common
{
	public class ItemSpawnData
	{
		public readonly int areaCode;


		public readonly int code;


		public readonly int dropCount;


		[JsonConverter(typeof(StringEnumConverter))]
		public readonly DropPoint dropPoint;


		public readonly int itemCode;


		public readonly int timing;

		[JsonConstructor]
		public ItemSpawnData(int code, int areaCode, int timing, int itemCode, DropPoint dropPoint, int dropCount)
		{
			this.code = code;
			this.areaCode = areaCode;
			this.timing = timing;
			this.itemCode = itemCode;
			this.dropPoint = dropPoint;
			this.dropCount = dropCount;
		}
	}
}