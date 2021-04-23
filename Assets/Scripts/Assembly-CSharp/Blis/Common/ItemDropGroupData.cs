using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Blis.Common
{
	public class ItemDropGroupData
	{
		[JsonConverter(typeof(StringEnumConverter))]
		public readonly ItemDropType dropType;


		public readonly int groupCode;


		public readonly int itemCode;


		public readonly int max;


		public readonly int min;


		public readonly int probability;

		[JsonConstructor]
		public ItemDropGroupData(int groupCode, int itemCode, int min, int max, int probability, ItemDropType dropType)
		{
			this.groupCode = groupCode;
			this.itemCode = itemCode;
			this.min = min;
			this.max = max;
			this.probability = probability;
			this.dropType = dropType;
		}
	}
}